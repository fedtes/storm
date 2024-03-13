using Storm.Execution;
using Storm.Origins;
using System;
using System.Collections.Generic;
using System.Linq;
using Storm.Schema;

namespace Storm.Test
{
    public class MockResult_TwoEntity_1To1
    {
        public static void DefineSchema(out Storm s, out Context ctx, out StormDataSet data, out List<SelectNode> nodes)
        {
            s = MonoEntityMockDataSources.StormDefine();
            ctx = s.CreateContext();
            var entity = ctx.Navigator.GetEntity("Test");
            var entity2 = ctx.Navigator.GetEntity("Test2");
            Origin fromNode1 = new Origin()
            {
                Alias = "A1",
                children = new List<Origin>(),
                Entity = entity,
                Edge = null,
                FullPath = new Schema.EntityPath("Test", "")
            };

            Origin fromNode2 = new Origin()
            {
                Alias = "A2",
                children = new List<Origin>(),
                Entity = entity2,
                Edge = null,
                FullPath = new Schema.EntityPath("Test", "ExtraInfos")
            };


            var _idField = entity.SimpleProperties.First(x => x.CodeName == "ID");
            var _FirstName = entity.SimpleProperties.First(x => x.CodeName == "FirstName");
            var _LastName = entity.SimpleProperties.First(x => x.CodeName == "LastName");
            var _Email = entity.SimpleProperties.First(x => x.CodeName == "Email");
            var _Mobile = entity.SimpleProperties.First(x => x.CodeName == "Mobile");
            var _Phone = entity.SimpleProperties.First(x => x.CodeName == "Phone");
            var _Addre = entity.SimpleProperties.First(x => x.CodeName == "Address");


            var _ID2 = entity2.SimpleProperties.First(x => x.CodeName == "ID");
            var _ParentID = entity2.SimpleProperties.First(x => x.CodeName == "ParentID");
            var _Info1 = entity2.SimpleProperties.First(x => x.CodeName == "Info1");
            var _Info2 = entity2.SimpleProperties.First(x => x.CodeName == "Info2");
            var _Info3 = entity2.SimpleProperties.First(x => x.CodeName == "Info3");

            data = new StormDataSet("Test");
            nodes = new List<SelectNode>()
            {
                new SelectNode() {FromNode = fromNode1, FullPath = new Schema.FieldPath("Test","","ID"), EntityField = _idField},
                new SelectNode() {FromNode = fromNode1, FullPath = new Schema.FieldPath("Test","","FirstName"), EntityField = _FirstName},
                new SelectNode() {FromNode = fromNode1, FullPath = new Schema.FieldPath("Test","","LastName"), EntityField = _LastName},
                new SelectNode() {FromNode = fromNode1, FullPath = new Schema.FieldPath("Test","","Email"), EntityField = _Email},
                new SelectNode() {FromNode = fromNode1, FullPath = new Schema.FieldPath("Test","","Mobile"), EntityField = _Mobile},
                new SelectNode() {FromNode = fromNode1, FullPath = new Schema.FieldPath("Test","","Phone"), EntityField = _Phone},
                new SelectNode() {FromNode = fromNode1, FullPath = new Schema.FieldPath("Test","","Address"), EntityField = _Addre},

                new SelectNode() {FromNode = fromNode2, FullPath = new Schema.FieldPath("Test","ExtraInfos","ID"), EntityField = _ID2},
                new SelectNode() {FromNode = fromNode2, FullPath = new Schema.FieldPath("Test","ExtraInfos","ParentID"), EntityField = _ParentID},
                new SelectNode() {FromNode = fromNode2, FullPath = new Schema.FieldPath("Test","ExtraInfos","Info1"), EntityField = _Info1},
                new SelectNode() {FromNode = fromNode2, FullPath = new Schema.FieldPath("Test","ExtraInfos","Info2"), EntityField = _Info2},
                new SelectNode() {FromNode = fromNode2, FullPath = new Schema.FieldPath("Test","ExtraInfos","Info3"), EntityField = _Info3}
            };
        }

        public static (Storm, Context, StormDataSet) PrepareDataSet()
        {
            Storm s;
            Context ctx;
            StormDataSet data;
            List<SelectNode> nodes;
            DefineSchema(out s, out ctx, out data, out nodes);

            data.ReadData(new MockResult_TwoEntity_1To1().GetReader(), nodes);
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
            "A1$Address",
            "A2$ID",
            "A2$Info1",
            "A2$Info2",
            "A2$Info3"
        };

        public object[][] source = new object[][]
        {
            new object[] {1, "Mario", "Bianchi", "asd@mail.it", "3331313131", "04512364", "street 1", 1, "Pet", "Food", "Sport" },
            new object[] {2, "Maria", "Rima", "fgh@mail.it", "66565565", "04512364", "street 2", 2, "Movies", "Food", "Games"  },
            new object[] {3, "Andre", "Est", "ajkl@mail.it", "5545458786", "04512364", "street 12", 3, "Games", "Pet", "Sport"  },
            new object[] {4, "Luca", "Verdi", "jkl@mail.it", "54555656", "04512364", "street 13", 4, "Travels", "Food", "Games"  },
            new object[] {5, "Gino", "Russo", "zxc@mail.it", "7898787987", "04512364", "street 11", 5, "Travels", "Food", "Sport"  },
            new object[] {6, "Mario", "Amer", "acvb@mail.it", "788798998", "04512364", "street 2", 6, "", "", ""  },
            new object[] {7, "Paride", "Paris", "bnm@mail.it", "54566569665", "04512364", "street 31", 7, "", "", ""  },
            new object[] {8, "Fede", "Rico", "qwe@mail.it", "124554878", "04512364", "street 51", 8, "", "", ""  },
            new object[] {9, "Luca", "Serio", "ert@mail.it", "21385438384", "04512364", "street 671", 9, "", "", ""  },
            new object[] {10, "Lucia", "Merlo", "tyu@mail.it", "21245683531", "04512364", "street 231", 10, "", "", ""  },
            new object[] {11, "Doria", "Galli", "yui@mail.it", "2135465926", "04512364", "street 41", 11, "", "", ""  },
            new object[] {12, "Mattia", "Polo", "uio@mail.it", "1213843483", "04512364", "street 2341", 12, "", "", ""  },
            new object[] {13, "Mattia", "Denim", "aoip@mail.it", "2323456684", "04512364", "street 41", 13, "", "", ""  },
            new object[] {14, "Mara", "Ciano", "dfg@mail.it", "23235648468", "04512364", "street 451", 14, "", "", ""  },
            new object[] {15, "Iole", "Amer", "ghj@mail.it", "35454684", "04512364", "street 156", 15, "", "", ""  },
            new object[] {16, "Laura", "Giallo", "kjl@mail.it", "5435135", "04512364", "street 123", 16, "", "", ""  },
            new object[] {17, "Lucio", "Verdi", "zxc@mail.it", "65989896565", "04512364", "street 14", 17, "", "", ""  },
            new object[] {18, "Mario", "Rossi", "cvb@mail.it", "33333133331", "04512364", "street 12", 18, "Pet", "Food", "Travels" },
            new object[] {18, "Mario", "Rossi", "cvb@mail.it", "33333133331", "04512364", "street 12", 18, "Game", "Sleep", "Running" },
            new object[] {18, "Mario", "Rossi", "cvb@mail.it", "33333133331", "04512364", "street 12", 18, "Food", "Drinks", "Movie" },
            new object[] {18, "Mario", "Rossi", "cvb@mail.it", "33333133331", "04512364", "street 12", 18, "Music", "Game", "Coding" },
            new object[] {18, "Mario", "Rossi", "cvb@mail.it", "33333133331", "04512364", "street 12", 18, "Cat", "Dog", "Snake" }
        };

    }

    public class MockResult_2Entities_1ToManyRelation
    {

        public static (Storm, Context, StormDataSet) PrepareDataSet()
        {
            Storm s;
            Context ctx;
            StormDataSet data;
            List<SelectNode> nodes;
            MockResult_TwoEntity_1To1.DefineSchema(out s, out ctx, out data, out nodes);

            data.ReadData(new MockResult_2Entities_1ToManyRelation().GetReader(), nodes);
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
            "A1$Address",
            "A2$ID",
            "A2$ParentID",
            "A2$Info1",
            "A2$Info2",
            "A2$Info3"
        };

        public object[][] source = new object[][]
        {
            new object[] {18, "Mario", "Rossi", "cvb@mail.it", "33333133331", "04512364", "street 12", 1, 18, "Pet", "Food", "Travels" },
            new object[] {18, "Mario", "Rossi", "cvb@mail.it", "33333133331", "04512364", "street 12", 2, 18, "Game", "Sleep", "Running" },
            new object[] {18, "Mario", "Rossi", "cvb@mail.it", "33333133331", "04512364", "street 12", 3, 18, "Food", "Drinks", "Movie" },
            new object[] {18, "Mario", "Rossi", "cvb@mail.it", "33333133331", "04512364", "street 12", 4, 18, "Music", "Game", "Coding" },
            new object[] {18, "Mario", "Rossi", "cvb@mail.it", "33333133331", "04512364", "street 12", 5, 18, "Cat", "Dog", "Snake" },
            new object[] {17, "Lucio", "Verdi", "zxc@mail.it", "65989896565", "04512364", "street 14", 6, 17, "", "", ""  }
        };

    }

    public class MockResult_3Entities_1ToManyRelation
    {
        public static (Storm, Context, StormDataSet) PrepareDataSet()
        {
            var s = new Storm(SQLEngine.SQLite);
            s.EditSchema(x =>
            {
                return x.Add("E1", "E1", y => y.AddPrimary("ID", typeof(int)).Add("FirstName", typeof(string)))
                .Add("E2", "E2", y => y.AddPrimary("InfoID", typeof(int)).Add("ParentID", typeof(int)))
                .Add("E3", "E3", y => y.AddPrimary("InfoID", typeof(int)).Add("ParentID", typeof(int)))
                .Add("E4", "E4", y => y.AddPrimary("ExtraID", typeof(int)).Add("ParentID", typeof(int)))
                .Connect("Info1", "E1", "E2", "ID", "ParentID")
                .Connect("Info2", "E1", "E3", "ID", "ParentID")
                .Connect("Extra", "E2", "E4", "InfoID", "ParentID");
            });
            var data = new StormDataSet("E1");
            var ctx = s.CreateContext();
            Origin n1 = new Origin()
            {
                Alias = "A1",
                children = new List<Origin>(),
                Entity = ctx.Navigator.GetEntity("E1"),
                Edge = null,
                FullPath = new Schema.EntityPath("E1", "")
            };

            Origin n2 = new Origin()
            {
                Alias = "A2",
                children = new List<Origin>(),
                Entity = ctx.Navigator.GetEntity("E2"),
                Edge = null,
                FullPath = new Schema.EntityPath("E1", "Info1")
            };

            Origin n3 = new Origin()
            {
                Alias = "A3",
                children = new List<Origin>(),
                Entity = ctx.Navigator.GetEntity("E3"),
                Edge = null,
                FullPath = new Schema.EntityPath("E1", "Info2")
            };

            Origin n4 = new Origin()
            {
                Alias = "A4",
                children = new List<Origin>(),
                Entity = ctx.Navigator.GetEntity("E4"),
                Edge = null,
                FullPath = new Schema.EntityPath("E1", "Info1.Extra")
            };

            var nodes = new List<SelectNode>()
            {
                new SelectNode()
                {
                    FromNode = n1,
                    FullPath = new FieldPath("E1","","ID"),
                    EntityField = n1.Entity.SimpleProperties.First(x => x.CodeName == "ID")
                },
                new SelectNode()
                {
                    FromNode = n1,
                    FullPath = new FieldPath("E1","","FirstName"),
                    EntityField = n1.Entity.SimpleProperties.First(x => x.CodeName == "FirstName")
                },
                new SelectNode()
                {
                    FromNode = n2,
                    FullPath = new FieldPath("E1","Info1","InfoID"),
                    EntityField = n2.Entity.SimpleProperties.First(x => x.CodeName == "InfoID")
                },
                new SelectNode()
                {
                    FromNode = n2,
                    FullPath = new FieldPath("E1","Info1","ParentID"),
                    EntityField = n2.Entity.SimpleProperties.First(x => x.CodeName == "ParentID")
                },
                new SelectNode()
                {
                    FromNode = n3,
                    FullPath = new FieldPath("E1","Info2","InfoID"),
                    EntityField = n3.Entity.SimpleProperties.First(x => x.CodeName == "InfoID")
                },
                new SelectNode()
                {
                    FromNode = n3,
                    FullPath = new FieldPath("E1","Info2","ParentID"),
                    EntityField = n3.Entity.SimpleProperties.First(x => x.CodeName == "ParentID")
                },
                new SelectNode()
                {
                    FromNode = n4,
                    FullPath = new FieldPath("E1","Info1.Extra","ExtraID"),
                    EntityField = n4.Entity.SimpleProperties.First(x => x.CodeName == "ExtraID")
                },
                new SelectNode()
                {
                    FromNode = n4,
                    FullPath = new FieldPath("E1","Info1.Extra","ParentID"),
                    EntityField = n4.Entity.SimpleProperties.First(x => x.CodeName == "ParentID")
                },
            };

            data.ReadData(new MockResult_3Entities_1ToManyRelation().GetReader(), nodes);
            return (s, ctx, data);
        }

        public MockReader GetReader() => new MockReader(source, names);

        public string[] names = new string[]
        {
            "A1$ID",
            "A1$FirstName",
            "A2$InfoID",
            "A2$ParentID",
            "A3$InfoID",
            "A3$ParentID",
            "A4$ExtraID",
            "A4$ParentID"
        };

        public object[][] source = new object[][] 
        {                                 // 1, 2, 1, 2, 1, 2
            new object[] {1,"Mario", 1, 1, null, null, 1, 1},
            new object[] {1,"Mario", 1, 1, null, null, 2, 1},
            new object[] {1,"Mario", 2, 1, 2, 1, 3, 2},
            new object[] {1,"Mario", 3, 1, 2, 1, null, null }
        }; 
    }


}
