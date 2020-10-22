﻿using Storm.Execution;
using Storm.Execution.Results;
using Storm.Helpers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Xunit;

namespace Storm.Test
{
    public class ReaderResult
    {

        [Fact]
        public void SelectResult_DataSetRead()
        {
            (Storm s, Schema.SchemaNavigator nav, StormDataSet data) = PrepareDataSet();

            Assert.Equal(18, data.Count());
            Assert.Equal(1, data.First()["ID"]);
            Assert.Equal("Mario", data.First()["FirstName"]);
            Assert.Equal(18, data.Last()["Test.ID"]);
            Assert.Equal("33333133331", data.Last()["Mobile"]);
            Assert.Equal(5, data.Where(x => x["FirstName"].ToString().StartsWith("Mar")).Count());
        }

        [Fact]
        public void GetResult_DataSetRead()
        {
            (Storm s, Schema.SchemaNavigator nav, StormDataSet data) = PrepareDataSet();
            GetCommand cmd = new GetCommand(nav, "Test");

            var res = GetCommandHelpers.ToResults(data, nav, cmd.requests, cmd.from);
            Assert.Equal(18, res.Count);
            Assert.Equal(1, res.First().Value.ID);
            Assert.Equal("Mario", res.First().Value.FirstName);
            Assert.Equal("Bianchi", res.First().Value.LastName);
            Assert.Equal("asd@mail.it", res.First().Value.Email);
            Assert.Equal(18, res.Last().Value.ID);
            Assert.Equal("33333133331", res.Last().Value.Mobile);
        }

        private static (Storm,Schema.SchemaNavigator,StormDataSet) PrepareDataSet()
        {
            Storm s = StormDefine();
            var nav = s.schema.GetNavigator();
            var entity = nav.GetEntity("Test");
            FromNode fromNode = new FromNode()
            {
                Alias = "A1",
                children = new List<FromNode>(),
                Entity = entity,
                Edge = null,
                FullPath = new Schema.EntityPath("Test", "")
            };
            var _idField = entity.entityFields.First(x => x.CodeName == "ID");
            var _FirstName = entity.entityFields.First(x => x.CodeName == "FirstName");
            var _LastName = entity.entityFields.First(x => x.CodeName == "LastName");
            var _Email = entity.entityFields.First(x => x.CodeName == "Email");
            var _Mobile = entity.entityFields.First(x => x.CodeName == "Mobile");
            var _Phone = entity.entityFields.First(x => x.CodeName == "Phone");
            var _Addre = entity.entityFields.First(x => x.CodeName == "Address");

            StormDataSet data = new StormDataSet("Test");
            List<SelectNode> nodes = new List<SelectNode>()
            {
                new SelectNode() {FromNode = fromNode, FullPath = new Schema.FieldPath("Test","","ID"), EntityField = _idField},
                new SelectNode() {FromNode = fromNode, FullPath = new Schema.FieldPath("Test","","FirstName"), EntityField = _FirstName},
                new SelectNode() {FromNode = fromNode, FullPath = new Schema.FieldPath("Test","","LastName"), EntityField = _LastName},
                new SelectNode() {FromNode = fromNode, FullPath = new Schema.FieldPath("Test","","Email"), EntityField = _Email},
                new SelectNode() {FromNode = fromNode, FullPath = new Schema.FieldPath("Test","","Mobile"), EntityField = _Mobile},
                new SelectNode() {FromNode = fromNode, FullPath = new Schema.FieldPath("Test","","Phone"), EntityField = _Phone},
                new SelectNode() {FromNode = fromNode, FullPath = new Schema.FieldPath("Test","","Address"), EntityField = _Addre}
            };


            data.ReadData(new MockDataSources().GetReader(), nodes);
            return (s,nav,data);
        }


        private static Storm StormDefine()
        {
            var s = new Storm();
            s.EditSchema(x =>
            {
                x.Add("Test", "TABTest", y =>
                {
                    y.Add(new Schema.FieldConfig() { CodeName = "ID", DBName = "ID", CodeType = typeof(Int32), DBType = DbType.Int32, IsPrimary = true });
                    y.Add(new Schema.FieldConfig() { CodeName = "FirstName", DBName = "FirstName", CodeType = typeof(String), DBType = DbType.String });
                    y.Add(new Schema.FieldConfig() { CodeName = "LastName", DBName = "LastName", CodeType = typeof(String), DBType = DbType.String });
                    y.Add(new Schema.FieldConfig() { CodeName = "Email", DBName = "Email", CodeType = typeof(String), DBType = DbType.String });
                    y.Add(new Schema.FieldConfig() { CodeName = "Mobile", DBName = "Mobile", CodeType = typeof(String), DBType = DbType.String });
                    y.Add(new Schema.FieldConfig() { CodeName = "Phone", DBName = "Phone", CodeType = typeof(String), DBType = DbType.String });
                    y.Add(new Schema.FieldConfig() { CodeName = "Address", DBName = "Address", CodeType = typeof(String), DBType = DbType.String});
                    return y;
                });
                return x;
            });
            return s;
        }

    }

    public class MockDataSources
    {
        public MockReader GetReader() => new MockReader(source, names);

        public String[] names = new string[]
        {
            "A1$ID",
            "A1$FirstName",
            "A1$LastName",
            "A1$Email",
            "A1$Mobile",
            "A1$Phone",
            "A1$Address"
        };

        public object[][] source = new object[][]
        {
            new object[] {1, "Mario", "Bianchi", "asd@mail.it", "3331313131", "04512364", "street 1" },
            new object[] {2, "Maria", "Rima", "fgh@mail.it", "66565565", "04512364", "street 2" },
            new object[] {3, "Andre", "Est", "ajkl@mail.it", "5545458786", "04512364", "street 12" },
            new object[] {4, "Luca", "Verdi", "jkl@mail.it", "54555656", "04512364", "street 13" },
            new object[] {5, "Gino", "Russo", "zxc@mail.it", "7898787987", "04512364", "street 11" },
            new object[] {6, "Mario", "Amer", "acvb@mail.it", "788798998", "04512364", "street 2" },
            new object[] {7, "Paride", "Paris", "bnm@mail.it", "54566569665", "04512364", "street 31" },
            new object[] {8, "Fede", "Rico", "qwe@mail.it", "124554878", "04512364", "street 51" },
            new object[] {9, "Luca", "Serio", "ert@mail.it", "21385438384", "04512364", "street 671" },
            new object[] {10, "Lucia", "Merlo", "tyu@mail.it", "21245683531", "04512364", "street 231" },
            new object[] {11, "Doria", "Galli", "yui@mail.it", "2135465926", "04512364", "street 41" },
            new object[] {12, "Mattia", "Polo", "uio@mail.it", "1213843483", "04512364", "street 2341" },
            new object[] {13, "Mattia", "Denim", "aoip@mail.it", "2323456684", "04512364", "street 41" },
            new object[] {14, "Mara", "Ciano", "dfg@mail.it", "23235648468", "04512364", "street 451" },
            new object[] {15, "Iole", "Amer", "ghj@mail.it", "35454684", "04512364", "street 156" },
            new object[] {16, "Laura", "Giallo", "kjl@mail.it", "5435135", "04512364", "street 123" },
            new object[] {17, "Lucio", "Verdi", "zxc@mail.it", "65989896565", "04512364", "street 14" },
            new object[] {18, "Mario", "Rossi", "cvb@mail.it", "33333133331", "04512364", "street 12" }
        };

    }

    public class MockReader : IDataReader
    {
        private readonly object[][] source;
        private readonly string[] names;
        private int row = -1;
        private bool isOpen = true;


        public MockReader(object[][] source, string[] names)
        {
            this.source = source;
            this.names = names;
        }

        public object this[int i] => source[row][i];

        public object this[string name] => source[row][this.GetOrdinal(name)];

        public bool IsClosed => isOpen;

        public int RecordsAffected => source.Length;

        public int FieldCount => names.Length;

        public void Close() { isOpen = false; }

        public void Dispose() { }

        public bool GetBoolean(int i) => (bool)source[row][i];

        public byte GetByte(int i) => (byte)source[row][i];

        public char GetChar(int i) => (char)source[row][i];

        public string GetDataTypeName(int i) => source[row][i].GetType().Name;

        public DateTime GetDateTime(int i) => (DateTime)source[row][i];

        public decimal GetDecimal(int i) => (decimal) source[row][i];

        public double GetDouble(int i) => (double)source[row][i];

        public Type GetFieldType(int i) => source[row][i].GetType();

        public float GetFloat(int i) => (float)source[row][i];

        public Guid GetGuid(int i) => (Guid)source[row][i];

        public short GetInt16(int i) => (short)source[row][i];

        public int GetInt32(int i) => (int) source[row][i];

        public long GetInt64(int i) => (long)source[row][i];

        public string GetName(int i) => names[i];

        public int GetOrdinal(string name) => names.Select((x, i) => (x == name, i)).First(x => x.Item1).i;

        public string GetString(int i) => (string)source[row][i];

        public object GetValue(int i) => source[row][i];

        public bool IsDBNull(int i) => false;

        public bool Read()
        {
            if (row + 1 < source.Length)
            {
                row++;
                return true;
            }
            else
            {
                return false;
            }        
        }

        public bool NextResult()
        {
            throw new NotImplementedException();
        }

        public int GetValues(object[] values)
        {
            throw new NotImplementedException();
        }

        public DataTable GetSchemaTable()
        {
            throw new NotImplementedException();
        }

        public long GetChars(int i, long fieldoffset, char[] buffer, int bufferoffset, int length)
        {
            throw new NotImplementedException();
        }

        public IDataReader GetData(int i)
        {
            throw new NotImplementedException();
        }

        public long GetBytes(int i, long fieldOffset, byte[] buffer, int bufferoffset, int length)
        {
            throw new NotImplementedException();
        }

        public int Depth => throw new NotImplementedException();

    }
}
