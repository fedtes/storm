using Storm.Helpers;
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

    public class StormResult : IEnumerable<StormResultRow>
    {
        internal Dictionary<string, int> columnMap = new Dictionary<string, int>();

        internal String root;

        internal IList<Object[]> data;

        public StormResult(String Root)
        {
            this.root = Root;
        }

        internal String NKey(String key)
        {
            return key.StartsWith($"{root}.") ? key : $"{root}.{key}";
        }

        public IEnumerator<StormResultRow> GetEnumerator()
        {
           return data.Select((r, i) => new StormResultRow(this, i)).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return (IEnumerator)this.GetEnumerator();
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
                        var c = new Column(dataReader.GetName(i));
                        var m = readerMetadata.FirstOrDefault(x => x.EntityField.DBName == c.Name && x.Alias == c.Alias);
                        columnMap.Add(m.FullPath, i);
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
