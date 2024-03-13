using Storm.Execution;
using Storm.Origins;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Storm.Test
{
    public class MonoEntityMockDataSources
    {
        public static Storm StormDefine()
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
                    y.Add(new Schema.FieldConfig() { CodeName = "Address", DBName = "Address", CodeType = typeof(String), DBType = DbType.String });
                    return y;
                });

                x.Add("Test2", "TABTest2", y =>
                {
                    y.Add(new Schema.FieldConfig() { CodeName = "ID", DBName = "ID", CodeType = typeof(Int32), DBType = DbType.Int32, IsPrimary = true });
                    y.Add(new Schema.FieldConfig() { CodeName = "ParentID", DBName = "ParentID", CodeType = typeof(Int32), DBType = DbType.Int32 });
                    y.Add(new Schema.FieldConfig() { CodeName = "Info1", DBName = "Info1", CodeType = typeof(String), DBType = DbType.String });
                    y.Add(new Schema.FieldConfig() { CodeName = "Info2", DBName = "Info2", CodeType = typeof(String), DBType = DbType.String });
                    y.Add(new Schema.FieldConfig() { CodeName = "Info3", DBName = "Info3", CodeType = typeof(String), DBType = DbType.String });
                    return y;
                });

                x.Connect("ExtraInfos", "Test", "Test2", "ID", "ParentID");

                return x;
            });
            return s;
        }

        public static (Storm, Context, StormDataSet) PrepareDataSet()
        {
            Storm s = StormDefine();
            var ctx = s.CreateContext();
            var entity = ctx.Navigator.GetEntity("Test");
            Origin fromNode = new Origin()
            {
                Alias = "A1",
                children = new List<Origin>(),
                Entity = entity,
                Edge = null,
                FullPath = new Schema.EntityPath("Test", "")
            };
            var _idField = entity.SimpleProperties.First(x => x.CodeName == "ID");
            var _FirstName = entity.SimpleProperties.First(x => x.CodeName == "FirstName");
            var _LastName = entity.SimpleProperties.First(x => x.CodeName == "LastName");
            var _Email = entity.SimpleProperties.First(x => x.CodeName == "Email");
            var _Mobile = entity.SimpleProperties.First(x => x.CodeName == "Mobile");
            var _Phone = entity.SimpleProperties.First(x => x.CodeName == "Phone");
            var _Addre = entity.SimpleProperties.First(x => x.CodeName == "Address");

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


            data.ReadData(new MonoEntityMockDataSources().GetReader(), nodes);
            return (s, ctx, data);
        }



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
}
