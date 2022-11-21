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
            (Storm s, Context nav, StormDataSet data) = MonoEntityMockDataSources.PrepareDataSet();

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
            (Storm s, Context ctx, StormDataSet data) = MonoEntityMockDataSources.PrepareDataSet();
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
            (Storm s, Context ctx, StormDataSet data) = MockResult_TwoEntity_1To1.PrepareDataSet();
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
            (Storm s, Context ctx, StormDataSet data) = MockResult_2Entities_1ToManyRelation.PrepareDataSet();
            GetCommand cmd = new GetCommand(ctx, "Test");
            cmd.With("ExtraInfos");

            var res = GetCommandHelpers.ToResults(data, ctx, cmd.requests, cmd.from).Cast<dynamic>();
            
            var _mario = res.First(x => x.Email == "cvb@mail.it");
            var extraInfo = _mario.ExtraInfos;
            Assert.Equal(5, extraInfo.Count);
            
        }

        [Fact]
        void GetResult_DataSetRead_3Entities_MultiRelation()
        {
            (Storm s, Context ctx, StormDataSet data) = MockResult_3Entities_1ToManyRelation.PrepareDataSet();
            GetCommand cmd = new GetCommand(ctx, "E1");
            cmd.With("Info1");
            cmd.With("Info2");
            cmd.With("Info1.Extra");

            var res = GetCommandHelpers.ToResults(data, ctx, cmd.requests, cmd.from).Cast<dynamic>();
            Assert.Equal("Mario", res.First().FirstName);
            Assert.Equal(1, res.First().Info1[0].InfoID);
            Assert.Equal(2, res.First().Info1[1].InfoID);
            Assert.Equal(3, res.First().Info1[2].InfoID);
            Assert.Equal(2, res.First().Info2[0].InfoID);
            Assert.Equal(1, res.First().Info1[0].Extra[0].ExtraID);
            Assert.Equal(2, res.First().Info1[0].Extra[1].ExtraID);
        }







    }
}
