﻿using Storm.Helpers;
using Storm.Schema;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace Storm.Execution.Results
{
    internal class ReaderMetadata
    {
        public EntityField EntityField;
        public String FullPath;
        public String Alias;

        public object FieldDefault()
        {
            return EntityField.DefaultIfNull != null ? EntityField.DefaultIfNull : (EntityField.CodeType.IsValueType ? TypeHelper.DefValues[EntityField.CodeType] : null);
        }
    }

    public class StormDataSet : IEnumerable<StormRow>
    {
        internal Dictionary<string, int> ColumnMap = new Dictionary<string, int>();

        protected String root;

        internal IList<Object[]> data;

        public StormDataSet(String Root)
        {
            this.root = Root;
        }

        internal String NKey(String key)
        {
            return key.StartsWith($"{root}.") ? key : $"{root}.{key}";
        }

        internal int Map(String key)
        {
            return ColumnMap[NKey(key)];
        }

        public IEnumerator<StormRow> GetEnumerator()
        {
           return data.Select((r, i) => new StormRow(this, i)).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return (IEnumerator)this.GetEnumerator();
        }

        private (string, string) splitColumnName(string name)
        {
            var x = name.Split('$');
            return (x[0], x[1]);
        }

        internal void ReadData(IDataReader dataReader, IEnumerable<ReaderMetadata> readerMetadata)
        {
            int iteration = 0;
            var tempMap = new Dictionary<int, ReaderMetadata>();
            this.data = new List<object[]>();
            while (dataReader.Read())
            {
                // Calculate columns
                if (0 == iteration)
                {
                    for (int i = 0; i < dataReader.FieldCount; i++)
                    {
                        var (_alias, _name) = splitColumnName(dataReader.GetName(i));
                        var m = readerMetadata.FirstOrDefault(x => x.EntityField.DBName == _name && x.Alias == _alias);
                        ColumnMap.Add(NKey(m.FullPath), i);
                        tempMap.Add(i, m);
                    }
                }

                // Calculate data
                var dataRow = new object[dataReader.FieldCount];

                for (int i = 0; i < dataReader.FieldCount; i++)
                {
                    var metadata = tempMap[i];

                    if (dataReader.IsDBNull(i))
                    {
                        dataRow[i] = metadata.FieldDefault();
                    }
                    else
                    {
                        var v = dataReader.GetValue(i);
                        if (v.GetType() == (metadata.EntityField.CodeType))
                        {
                            dataRow[i] = v;
                        }
                        else
                        {
                            try
                            {
                                var x = Convert.ChangeType(dataReader.GetValue(i), metadata.EntityField.CodeType);
                                dataRow[i] = x;
                            }
                            catch (InvalidCastException)
                            {
                                dataRow[i] = metadata.FieldDefault();
                            }
                        }
                    }
                }

                this.data.Add(dataRow);
                iteration++;
            }
        }
    }   
}