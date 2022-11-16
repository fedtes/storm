using Storm.Execution;
using Storm.Helpers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Storm.Origins;
using Xunit;

namespace Storm.Test
{
    public class ReaderResult
    {

        [Fact]
        public void SelectResult_DataSetRead()
        {
            (Storm s, Context nav, StormDataSet data) = PrepareDataSet();

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
            (Storm s, Context ctx, StormDataSet data) = PrepareDataSet();
            GetCommand cmd = new GetCommand(ctx, "Test");

            var res = GetCommandHelpers.ToResults(data, ctx, cmd.requests, cmd.from).Cast<dynamic>();
            Assert.Equal(18, res.Count());
            Assert.Equal(1, res.First().ID);
            Assert.Equal("Mario", res.First().FirstName);
            Assert.Equal("Bianchi", res.First().LastName);
            Assert.Equal("asd@mail.it", res.First().Email);
            Assert.Equal(18, res.Last().ID);
            Assert.Equal("33333133331", res.Last().Mobile);
        }

        [Fact]
        void GetResult_DataSetRead_2Entities()
        {
            (Storm s, Context ctx, StormDataSet data) = PrepareDataSet2();
            GetCommand cmd = new GetCommand(ctx, "Test");
            cmd.With("ExtraInfos");

            var res = GetCommandHelpers.ToResults(data, ctx, cmd.requests, cmd.from).Cast<dynamic>();
            Assert.Equal(18, res.Count());
            var _mario = res.First();
            var extraInfo = _mario.ExtraInfos[0];
            Assert.Equal(1, extraInfo.ID);
            Assert.Equal("Pet", extraInfo.Info1);
            Assert.Equal("Food", extraInfo.Info2);
            Assert.Equal("Sport", extraInfo.Info3);


            var _maria = res.Skip(1).First();
            extraInfo = _maria.ExtraInfos[0];
            Assert.Equal(2, extraInfo.ID);
            Assert.Equal("Movies", extraInfo.Info1);
            Assert.Equal("Food", extraInfo.Info2);
            Assert.Equal("Games", extraInfo.Info3);
        }


        [Fact]
        void GetResult_DataSetRead_2Entities_MultiRelation()
        {
            (Storm s, Context ctx, StormDataSet data) = PrepareDataSet3();
            GetCommand cmd = new GetCommand(ctx, "Test");
            cmd.With("ExtraInfos");

            var res = GetCommandHelpers.ToResults(data, ctx, cmd.requests, cmd.from).Cast<dynamic>();
            
            var _mario = res.First(x => x.Email == "cvb@mail.it");
            var extraInfo = _mario.ExtraInfos;
            Assert.Equal(5, extraInfo.Count);
            
        }




        private static (Storm,Context,StormDataSet) PrepareDataSet()
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


            data.ReadData(new MonoEntityMockDataSources().GetReader(), nodes);
            return (s,ctx,data);
        }

        private static (Storm, Context, StormDataSet) PrepareDataSet2()
        {
            Storm s;
            Context ctx;
            StormDataSet data;
            List<SelectNode> nodes;
            DefineSchema(out s, out ctx, out data, out nodes);

            data.ReadData(new TwoEntityMockDataSources().GetReader(), nodes);
            return (s, ctx, data);
        }

        private static (Storm, Context, StormDataSet) PrepareDataSet3()
        {
            Storm s;
            Context ctx;
            StormDataSet data;
            List<SelectNode> nodes;
            DefineSchema(out s, out ctx, out data, out nodes);

            data.ReadData(new TwoEntityMockDataSources2().GetReader(), nodes);
            return (s, ctx, data);
        }

        private static void DefineSchema(out Storm s, out Context ctx, out StormDataSet data, out List<SelectNode> nodes)
        {
            s = StormDefine();
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


            var _idField = entity.entityFields.First(x => x.CodeName == "ID");
            var _FirstName = entity.entityFields.First(x => x.CodeName == "FirstName");
            var _LastName = entity.entityFields.First(x => x.CodeName == "LastName");
            var _Email = entity.entityFields.First(x => x.CodeName == "Email");
            var _Mobile = entity.entityFields.First(x => x.CodeName == "Mobile");
            var _Phone = entity.entityFields.First(x => x.CodeName == "Phone");
            var _Addre = entity.entityFields.First(x => x.CodeName == "Address");


            var _ID2 = entity2.entityFields.First(x => x.CodeName == "ID");
            var _ParentID = entity2.entityFields.First(x => x.CodeName == "ParentID");
            var _Info1 = entity2.entityFields.First(x => x.CodeName == "Info1");
            var _Info2 = entity2.entityFields.First(x => x.CodeName == "Info2");
            var _Info3 = entity2.entityFields.First(x => x.CodeName == "Info3");

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

    }
}
