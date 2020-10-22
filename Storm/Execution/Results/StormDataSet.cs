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
    internal class IndexRange
    {
        public int Start;
        public int End;
    }

    public class StormDataSet : IEnumerable<StormRow>
    {
        internal Dictionary<FieldPath, int> ColumnMap = new Dictionary<FieldPath, int>();

        internal Dictionary<SchemaNode, IndexRange> ObjectRanges = new Dictionary<SchemaNode, IndexRange>();

        internal Dictionary<EntityPath, FieldPath> IdentityIndexes = new Dictionary<EntityPath, FieldPath>();

        internal String root;

        internal IList<Object[]> data;

        public StormDataSet(String Root)
        {
            this.root = Root;
        }

        public override string ToString()
        {
            return $"Root: \"{root}\"; Rows: {this.Count()}; Cols: {ColumnMap.Count}; Objects: {ObjectRanges.Count};";
        }

        internal FieldPath NKey(String key)
        {
            //                 |a|b|c|.|e|f|g|.|h|i|
            // lastDot = 7      |           | ^ |
            // path = abc.efg   ^           ^   |
            // field = hi                       ^ 
            var lastDot = key.LastIndexOf('.');
            var path = lastDot == -1 ? "" : key.Substring(0, lastDot);
            var field = lastDot == -1 ? key : key.Substring(lastDot + 1);
            return new FieldPath(root, path, field);
        }

        internal int Map(String key)
        {
            return ColumnMap[NKey(key)];
        }

        internal int Map(FieldPath key)
        {
            return ColumnMap[key];
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

        internal void ReadData(IDataReader dataReader, IEnumerable<SelectNode> selectNodes)
        {
            int iteration = 0;
            var readerIdx_selectNode = new Dictionary<int, SelectNode>();
            this.data = new List<object[]>();
            while (dataReader.Read())
            {
                // Calculate columns
                if (0 == iteration)
                {
                    ComputeColumnMetadata(dataReader, selectNodes, readerIdx_selectNode);
                }

                // Calculate data
                var dataRow = new object[dataReader.FieldCount];

                for (int i = 0; i < dataReader.FieldCount; i++)
                {
                    var selectNode = readerIdx_selectNode[i];

                    if (dataReader.IsDBNull(i))
                    {
                        dataRow[i] = selectNode.FieldDefault;
                    }
                    else
                    {
                        var v = dataReader.GetValue(i);
                        if (v.GetType() == (selectNode.EntityField.CodeType))
                        {
                            dataRow[i] = v;
                        }
                        else
                        {
                            try
                            {
                                var x = Convert.ChangeType(dataReader.GetValue(i), selectNode.EntityField.CodeType);
                                dataRow[i] = x;
                            }
                            catch (InvalidCastException)
                            {
                                dataRow[i] = selectNode.FieldDefault;
                            }
                        }
                    }
                }

                this.data.Add(dataRow);
                iteration++;
            }
        }

        private void ComputeColumnMetadata(IDataReader dataReader, IEnumerable<SelectNode> selectNodes, Dictionary<int, SelectNode> tempMap)
        {
            SchemaNode currentEntity = null;
            for (int i = 0; i < dataReader.FieldCount; i++)
            {
                var (_alias, _name) = splitColumnName(dataReader.GetName(i));
                var m = selectNodes.FirstOrDefault(x => x.EntityField.CodeName == _name && x.Alias == _alias);

                if (m.OwnerEntity != currentEntity)
                {
                    if (!(currentEntity is null))
                    {
                        this.ObjectRanges[currentEntity].End = i - 1;
                    }
                    this.ObjectRanges.Add(m.OwnerEntity, new IndexRange() { Start = i });
                    currentEntity = m.OwnerEntity;
                }
                else if (i == dataReader.FieldCount - 1)
                {
                    this.ObjectRanges[currentEntity].End = i;
                }

                if (m.EntityField.IsPrimary)
                    this.IdentityIndexes.Add(m.FullPath.OwnerEntityPath, m.FullPath);

                ColumnMap.Add(m.FullPath, i);
                tempMap.Add(i, m);
            }
        }
    }   
}
