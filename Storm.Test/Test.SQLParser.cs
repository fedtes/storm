using System;
using System.Collections.Generic;
using System.Text;
using SqlKata;
using SqlKata.Compilers;
using Storm.Schema;
using Xunit;
using Xunit.Sdk;

namespace Storm.Test.TestUnits
{
    public class TestingSQLParser
    {
        /*
         * Sample testing schema
         *  Model_1
         *    |
         *    +-----Model_2
         *    |
         *    +-----Model_3
         *            |
         *            +------Model_2
         *            |
         *            +------Model_4
         *                     |
         *                     +------Model_2
         */

        public const String Model_0 = "Model_0";
        public const String Model_1 = "Model_1";
        public const String Model_2 = "Model_2";
        public const String Model_3 = "Model_3";
        public const String Model_4 = "Model_4";
        public const String Model_5 = "Model_5";

        public SchemaEditor SampleSchema(SchemaEditor e)
        {
            return e.Add<Model_1>("Model_1", "Table_1")
                .Add<Model_2>("Model_2", "Table_2")
                .Add<Model_3>("Model_3", "Table_3")
                .Add<Model_4>("Model_4", "Table_4")
                .Add<Model_0>("Model_0", "Table_0")
                .Add<Model_5>("Model_5", "Table_5")
                .Connect("Child", "Model_0","Model_1", "Model1ID", "ID")
                .Connect("Child_2", "Model_1", "Model_2", "ID", "ParentID")
                .Connect("Child_3", "Model_1", "Model_3", "ID", "ParentID")
                .Connect("Child_2", "Model_3", "Model_2", "ID", "ParentID")
                .Connect("Child_4", "Model_3", "Model_4", "ID", "ParentID")
                .Connect("Child_2", "Model_4", "Model_2", "ID", "ParentID")
                .Connect("Child_5", "Model_5", "Model_2", ctx => ctx["source.ParentID"].EqualTo.Ref("target.ID") * ctx["source.ParentData"].Like.Ref("target.data"));
        }

        [Fact]
        public void Parse_GetObject_OnlyRoot()
        {
            Storm storm = new Storm();
            storm.EditSchema(SampleSchema);
            var con = storm.OpenConnection(new EmptyConnection());

            var cmd = con.Get(Model_1);

            cmd.ParseSQL();
            var compiler = new SqlServerCompiler();
            SqlResult result = compiler.Compile(cmd.query);
            string sql = result.Sql;

            // Previusly Calculated check sum integrity
            Assert.Equal("5DECAEBC7E86A73ABDEF2A9D0B9F9313", Helpers.Checksum(sql));
        }

        [Fact]
        public void Parse_GetObject_With_Relation_FullPathed()
        {
            Storm storm = new Storm();
            storm.EditSchema(SampleSchema);
            var con = storm.OpenConnection(new EmptyConnection());

            var cmd = con.Get(Model_1).With("Model_1.Child_2");

            cmd.ParseSQL();
            var compiler = new SqlServerCompiler();
            SqlResult result = compiler.Compile(cmd.query);
            string sql = result.Sql;

            // Previusly Calculated check sum integrity
            Assert.Equal("D5E28B7FC534053BA010A747D97002F1", Helpers.Checksum(sql));
        }

        [Fact]
        public void Parse_GetObject_With_Relation_ShortPathed()
        {
            Storm storm = new Storm();
            storm.EditSchema(SampleSchema);
            var con = storm.OpenConnection(new EmptyConnection());

            var cmd = con.Get(Model_1).With("Child_2");

            cmd.ParseSQL();
            var compiler = new SqlServerCompiler();
            SqlResult result = compiler.Compile(cmd.query);
            string sql = result.Sql;

            // Previusly Calculated check sum integrity
            Assert.Equal("D5E28B7FC534053BA010A747D97002F1", Helpers.Checksum(sql));
        }

        [Fact]
        public void Parse_GetObject_With_Second_Lev_Relation_FullPathed()
        {
            Storm storm = new Storm();
            storm.EditSchema(SampleSchema);
            var con = storm.OpenConnection(new EmptyConnection());

            var cmd = con.Get(Model_1).With("Model_1.Child_3").With("Model_1.Child_3.Child_2");

            cmd.ParseSQL();
            var compiler = new SqlServerCompiler();
            SqlResult result = compiler.Compile(cmd.query);
            string sql = result.Sql;

            // Previusly Calculated check sum integrity
            Assert.Equal("BEB883DD6AB5A3449718408384AAC684", Helpers.Checksum(sql));
        }

        [Fact]
        public void Parse_GetObject_With_Second_Lev_Relation_ShortPathed()
        {
            Storm storm = new Storm();
            storm.EditSchema(SampleSchema);
            var con = storm.OpenConnection(new EmptyConnection());

            var cmd = con.Get(Model_1).With("Child_3").With("Child_3.Child_2");

            cmd.ParseSQL();
            var compiler = new SqlServerCompiler();
            SqlResult result = compiler.Compile(cmd.query);
            string sql = result.Sql;

            // Previusly Calculated check sum integrity
            Assert.Equal("BEB883DD6AB5A3449718408384AAC684", Helpers.Checksum(sql));
        }

        [Fact]
        public void Parse_GetObject_With_Many_Relations()
        {
            Storm storm = new Storm();
            storm.EditSchema(SampleSchema);
            var con = storm.OpenConnection(new EmptyConnection());

            var cmd = con.Get(Model_1)
                .With("Child_2")
                .With("Child_3")
                .With("Child_3.Child_2")
                .With("Child_3.Child_4")
                .With("Child_3.Child_4.Child_2");

            cmd.ParseSQL();
            var compiler = new SqlServerCompiler();
            SqlResult result = compiler.Compile(cmd.query);
            string sql = result.Sql;

            // Previusly Calculated check sum integrity
            Assert.Equal("C361DB9C4CA6C29056DCC324960F8A7F", Helpers.Checksum(sql));
        }

        [Fact]
        public void Parse_GetObject_With_Where()
        {
            Storm storm = new Storm();
            storm.EditSchema(SampleSchema);
            var con = storm.OpenConnection(new EmptyConnection());

            var cmd = con.Get(Model_1).Where(e => e["data"].EqualTo.Val("some string data"));
                

            cmd.ParseSQL();
            var compiler = new SqlServerCompiler();
            SqlResult result = compiler.Compile(cmd.query);
            string sql = result.Sql;

            // Previusly Calculated check sum integrity
            Assert.Equal("4E0A97FDD8410E7E2B981D76CE1FC8C9", Helpers.Checksum(sql));
        }

        [Fact]
        public void Parse_GetObject_With_Where_And_Relation()
        {
            Storm storm = new Storm();
            storm.EditSchema(SampleSchema);
            var con = storm.OpenConnection(new EmptyConnection());

            var cmd = con.Get(Model_1)
                .With("Child_2")
                .Where(e => e["Child_2.data"].EqualTo.Val("some string data"));


            cmd.ParseSQL();
            var compiler = new SqlServerCompiler();
            SqlResult result = compiler.Compile(cmd.query);
            string sql = result.Sql;

            // Previusly Calculated check sum integrity
            Assert.Equal("47E96191276E23F796A7FDE7A5BE83A2", Helpers.Checksum(sql));
        }

        [Fact]
        public void Should_AutoAdd_Relation_For_Where_Condition()
        {
            Storm storm = new Storm();
            storm.EditSchema(SampleSchema);
            var con = storm.OpenConnection(new EmptyConnection());

            var cmd = con.Get(Model_1)
                .Where(e => e["Child_2.data"].EqualTo.Val("some string data"));


            cmd.ParseSQL();
            var compiler = new SqlServerCompiler();
            SqlResult result = compiler.Compile(cmd.query);
            string sql = result.Sql;

            // Previusly Calculated check sum integrity
            Assert.Equal("775B02A69D3A485B5C529BC06C32C45E", Helpers.Checksum(sql));
        }


        [Fact]
        public void Parse_Filter_EqualTo()
        {
            Storm storm = new Storm();
            storm.EditSchema(SampleSchema);
            var compiler = new SqlServerCompiler();
            var con = storm.OpenConnection(new EmptyConnection());

            var cmd1 = con.Get(Model_1)
                .Where(e => e["data"].EqualTo.Val("data1"));

            cmd1.ParseSQL();
            SqlResult result1 = compiler.Compile(cmd1.query);
            string sql1 = result1.Sql;

            // Previusly Calculated check sum integrity
            Assert.Equal("4E0A97FDD8410E7E2B981D76CE1FC8C9", Helpers.Checksum(sql1));

            var cmd2 = con.Get(Model_1)
                .Where(e => e["data"].EqualTo.Ref("Child_2.data"));

            cmd2.ParseSQL();
            SqlResult result2 = compiler.Compile(cmd2.query);
            string sql2 = result2.Sql;

            // Previusly Calculated check sum integrity
            Assert.Equal("D44B3E75EA1EEAAC343E96981603CD43", Helpers.Checksum(sql2));
        }

        [Fact]
        public void Parse_Filter_NotEqualTo()
        {
            Storm storm = new Storm();
            storm.EditSchema(SampleSchema);
            var compiler = new SqlServerCompiler();
            var con = storm.OpenConnection(new EmptyConnection());

            var cmd1 = con.Get(Model_1)
                .Where(e => e["data"].NotEqualTo.Val("data1"));

            cmd1.ParseSQL();
            SqlResult result1 = compiler.Compile(cmd1.query);
            string sql1 = result1.Sql;

            // Previusly Calculated check sum integrity
            Assert.Equal("0E80502056208D5E9BD4A68A47059921", Helpers.Checksum(sql1));

            var cmd2 = con.Get(Model_1)
                .Where(e => e["data"].NotEqualTo.Ref("Child_2.data"));

            cmd2.ParseSQL();
            SqlResult result2 = compiler.Compile(cmd2.query);
            string sql2 = result2.Sql;

            // Previusly Calculated check sum integrity
            Assert.Equal("296C9F346FD332C7E5ECBA70C5FEAFFA", Helpers.Checksum(sql2));
        }

        [Fact]
        public void Parse_Filter_GreaterTo()
        {
            Storm storm = new Storm();
            storm.EditSchema(SampleSchema);
            var compiler = new SqlServerCompiler();
            var con = storm.OpenConnection(new EmptyConnection());

            var cmd1 = con.Get(Model_1)
                .Where(e => e["data"].GreaterTo.Val("data1"));

            cmd1.ParseSQL();
            SqlResult result1 = compiler.Compile(cmd1.query);
            string sql1 = result1.Sql;

            // Previusly Calculated check sum integrity
            Assert.Equal("3AFF85B634EC75196A52F423A4308A30", Helpers.Checksum(sql1));

            var cmd2 = con.Get(Model_1)
                .Where(e => e["data"].GreaterTo.Ref("Child_2.data"));

            cmd2.ParseSQL();
            SqlResult result2 = compiler.Compile(cmd2.query);
            string sql2 = result2.Sql;

            // Previusly Calculated check sum integrity
            Assert.Equal("A69ECA081ECE81EF542FD3CF164F914E", Helpers.Checksum(sql2));
        }

        [Fact]
        public void Parse_Filter_GreaterOrEqualTo()
        {
            Storm storm = new Storm();
            storm.EditSchema(SampleSchema);
            var compiler = new SqlServerCompiler();
            var con = storm.OpenConnection(new EmptyConnection());

            var cmd1 = con.Get(Model_1)
                .Where(e => e["data"].GreaterOrEqualTo.Val("data1"));

            cmd1.ParseSQL();
            SqlResult result1 = compiler.Compile(cmd1.query);
            string sql1 = result1.Sql;

            // Previusly Calculated check sum integrity
            Assert.Equal("34AABBD1AE39782F004A906AE1B2AB72", Helpers.Checksum(sql1));

            var cmd2 = con.Get(Model_1)
                .Where(e => e["data"].GreaterOrEqualTo.Ref("Child_2.data"));

            cmd2.ParseSQL();
            SqlResult result2 = compiler.Compile(cmd2.query);
            string sql2 = result2.Sql;

            // Previusly Calculated check sum integrity
            Assert.Equal("B3F599E6750A4CEE5E467FA1A902F705", Helpers.Checksum(sql2));
        }

        [Fact]
        public void Parse_Filter_LessTo()
        {
            Storm storm = new Storm();
            storm.EditSchema(SampleSchema);
            var compiler = new SqlServerCompiler();
            var con = storm.OpenConnection(new EmptyConnection());

            var cmd1 = con.Get(Model_1)
                .Where(e => e["data"].LessTo.Val("data1"));

            cmd1.ParseSQL();
            SqlResult result1 = compiler.Compile(cmd1.query);
            string sql1 = result1.Sql;

            // Previusly Calculated check sum integrity
            Assert.Equal("8C816D6911AF6D3456AF68589502AFFE", Helpers.Checksum(sql1));

            var cmd2 = con.Get(Model_1)
                .Where(e => e["data"].LessTo.Ref("Child_2.data"));

            cmd2.ParseSQL();
            SqlResult result2 = compiler.Compile(cmd2.query);
            string sql2 = result2.Sql;

            // Previusly Calculated check sum integrity
            Assert.Equal("09864DD8F14F77517975BFDBBC9CE637", Helpers.Checksum(sql2));
        }

        [Fact]
        public void Parse_Filter_LessOrEqualTo()
        {
            Storm storm = new Storm();
            storm.EditSchema(SampleSchema);
            var compiler = new SqlServerCompiler();
            var con = storm.OpenConnection(new EmptyConnection());

            var cmd1 = con.Get(Model_1)
                .Where(e => e["data"].LessOrEqualTo.Val("data1"));

            cmd1.ParseSQL();
            SqlResult result1 = compiler.Compile(cmd1.query);
            string sql1 = result1.Sql;

            // Previusly Calculated check sum integrity
            Assert.Equal("5D7050823EE2EC6956E201D0400DA419", Helpers.Checksum(sql1));

            var cmd2 = con.Get(Model_1)
                .Where(e => e["data"].LessOrEqualTo.Ref("Child_2.data"));

            cmd2.ParseSQL();
            SqlResult result2 = compiler.Compile(cmd2.query);
            string sql2 = result2.Sql;

            // Previusly Calculated check sum integrity
            Assert.Equal("05069F883119759EB2718B153F573D1A", Helpers.Checksum(sql2));
        }

        [Fact]
        public void Parse_Filter_Like()
        {
            Storm storm = new Storm();
            storm.EditSchema(SampleSchema);
            var compiler = new SqlServerCompiler();
            var con = storm.OpenConnection(new EmptyConnection());

            var cmd1 = con.Get(Model_1)
                .Where(e => e["data"].Like.Val("data1"));

            cmd1.ParseSQL();
            SqlResult result1 = compiler.Compile(cmd1.query);
            string sql1 = result1.Sql;

            // Previusly Calculated check sum integrity
            Assert.Equal("DE63F4F1988F9EE29ABD22F382A210D3", Helpers.Checksum(sql1));

            var cmd2 = con.Get(Model_1)
                .Where(e => e["data"].Like.Ref("Child_2.data"));

            cmd2.ParseSQL();
            SqlResult result2 = compiler.Compile(cmd2.query);
            string sql2 = result2.Sql;

            // Previusly Calculated check sum integrity
            Assert.Equal("461335C8133CF486EB72CC7A2B7B5606", Helpers.Checksum(sql2));
        }

        [Fact]
        public void Parse_Filter_NotLike()
        {
            Storm storm = new Storm();
            storm.EditSchema(SampleSchema);
            var compiler = new SqlServerCompiler();
            var con = storm.OpenConnection(new EmptyConnection());

            var cmd1 = con.Get(Model_1)
                .Where(e => e["data"].NotLike.Val("data1"));

            cmd1.ParseSQL();
            SqlResult result1 = compiler.Compile(cmd1.query);
            string sql1 = result1.Sql;

            // Previusly Calculated check sum integrity
            Assert.Equal("40DD56CBBD7FB9B8F6B8475CC9FECB26", Helpers.Checksum(sql1));

            var cmd2 = con.Get(Model_1)
                .Where(e => e["data"].NotLike.Ref("Child_2.data"));

            cmd2.ParseSQL();
            SqlResult result2 = compiler.Compile(cmd2.query);
            string sql2 = result2.Sql;

            // Previusly Calculated check sum integrity
            Assert.Equal("33D480C67D31B5769AA65E4CA87403C2", Helpers.Checksum(sql2));
        }

        [Fact]
        public void Parse_Filter_In_List()
        {
            Storm storm = new Storm();
            storm.EditSchema(SampleSchema);
            var compiler = new SqlServerCompiler();
            var con = storm.OpenConnection(new EmptyConnection());

            var cmd1 = con.Get(Model_1)
                .Where(e => e["data"].In.Val(new[] {1,2,3,4,5}));

            cmd1.ParseSQL();
            SqlResult result1 = compiler.Compile(cmd1.query);
            string sql1 = result1.Sql;

            // Previusly Calculated check sum integrity
            Assert.Equal("BAE98D2CF661558CB2CF1D6C26AB098A", Helpers.Checksum(sql1));
        }

        [Fact]
        public void Parse_Filter_In_SubQuery()
        {
            Storm storm = new Storm();
            storm.EditSchema(SampleSchema);
            var compiler = new SqlServerCompiler();
            var con = storm.OpenConnection(new EmptyConnection());

            var cmd1 = con.Get(Model_1)
                .Where(e => 
                    e["data"].In.SubQuery(q => 
                        q.From(Model_2).Where(x => x["ID"].GreaterTo.Val(-1) * x["ParentID"].IsNotNull).Select("data")
                    )
                );

            cmd1.ParseSQL();
            SqlResult result1 = compiler.Compile(cmd1.query);
            string sql1 = result1.Sql;

            // Previusly Calculated check sum integrity
            Assert.Equal("12E59D7E3E10AF08B9AA6C5AB1182099", Helpers.Checksum(sql1));
        }

        [Fact]
        public void Parse_Filter_NotIn_List()
        {
            Storm storm = new Storm();
            storm.EditSchema(SampleSchema);
            var compiler = new SqlServerCompiler();
            var con = storm.OpenConnection(new EmptyConnection());

            var cmd1 = con.Get(Model_1)
                .Where(e => e["data"].NotIn.Val(new[] { 1, 2, 3, 4, 5 }));

            cmd1.ParseSQL();
            SqlResult result1 = compiler.Compile(cmd1.query);
            string sql1 = result1.Sql;

            // Previusly Calculated check sum integrity
            Assert.Equal("893AEFFC92B73345339EEA3BA39EC54B", Helpers.Checksum(sql1));
        }

        [Fact]
        public void Parse_Filter_NotIn_SubQuery()
        {
            Storm storm = new Storm();
            storm.EditSchema(SampleSchema);
            var compiler = new SqlServerCompiler();
            var con = storm.OpenConnection(new EmptyConnection());

            var cmd1 = con.Get(Model_1)
                .Where(e =>
                    e["data"].NotIn.SubQuery(q =>
                        q.From(Model_2).Where(x => x["ID"].GreaterTo.Val(-1) * x["ParentID"].IsNotNull).Select("data")
                    )
                );

            cmd1.ParseSQL();
            SqlResult result1 = compiler.Compile(cmd1.query);
            string sql1 = result1.Sql;

            // Previusly Calculated check sum integrity
            Assert.Equal("5B0285F0689036D3E033E9FF3910EC66", Helpers.Checksum(sql1));
        }

        [Fact]
        public void Parse_Filter_IsNull()
        {
            Storm storm = new Storm();
            storm.EditSchema(SampleSchema);
            var compiler = new SqlServerCompiler();
            var con = storm.OpenConnection(new EmptyConnection());

            var cmd1 = con.Get(Model_1)
                .Where(e => e["data"].IsNull);

            cmd1.ParseSQL();
            SqlResult result1 = compiler.Compile(cmd1.query);
            string sql1 = result1.Sql;

            // Previusly Calculated check sum integrity
            Assert.Equal("F1CCB7FFAC683AB0DCF69B7F78D6EDB1", Helpers.Checksum(sql1));
        }

        [Fact]
        public void Parse_Filter_IsNotNull()
        {
            Storm storm = new Storm();
            storm.EditSchema(SampleSchema);
            var compiler = new SqlServerCompiler();
            var con = storm.OpenConnection(new EmptyConnection());

            var cmd1 = con.Get(Model_1)
                .Where(e => e["data"].IsNotNull);

            cmd1.ParseSQL();
            SqlResult result1 = compiler.Compile(cmd1.query);
            string sql1 = result1.Sql;

            // Previusly Calculated check sum integrity
            Assert.Equal("E46DAD691FD356860076C865C1E55BEC", Helpers.Checksum(sql1));
        }

        [Fact]
        public void Parse_Filter_And()
        {
            Storm storm = new Storm();
            storm.EditSchema(SampleSchema);
            var compiler = new SqlServerCompiler();
            var con = storm.OpenConnection(new EmptyConnection());

            var cmd1 = con.Get(Model_1)
                .Where(e => e["data"].NotEqualTo.Val("data1") * e["data"].NotEqualTo.Val("data2"));

            cmd1.ParseSQL();
            SqlResult result1 = compiler.Compile(cmd1.query);
            string sql1 = result1.Sql;

            // Previusly Calculated check sum integrity
            Assert.Equal("974D688381F9BC2CA380785805D3E408", Helpers.Checksum(sql1));
        }


        [Fact]
        public void Parse_Filter_Or()
        {
            Storm storm = new Storm();
            storm.EditSchema(SampleSchema);
            var compiler = new SqlServerCompiler();
            var con = storm.OpenConnection(new EmptyConnection());

            var cmd1 = con.Get(Model_1)
                .Where(e => e["data"].EqualTo.Val("data1") + e["data"].EqualTo.Val("data2"));

            cmd1.ParseSQL();
            SqlResult result1 = compiler.Compile(cmd1.query);
            string sql1 = result1.Sql;

            // Previusly Calculated check sum integrity
            Assert.Equal("1D43CA3B01D03F5E9373F71407EBD7BB", Helpers.Checksum(sql1));
        }

        [Fact]
        public void Parse_Filter_And_Or_Mixed()
        {
            Storm storm = new Storm();
            storm.EditSchema(SampleSchema);
            var compiler = new SqlServerCompiler();
            var con = storm.OpenConnection(new EmptyConnection());

            var cmd1 = con.Get(Model_1)
                .Where(e => (e["Child_2.data"].EqualTo.Val("some data") * e["data"].EqualTo.Val("data1")) + e["data"].EqualTo.Val("data2") * e["Child_3.Child_2.data"].EqualTo.Val("some data"));

            cmd1.ParseSQL();
            SqlResult result1 = compiler.Compile(cmd1.query);
            string sql1 = result1.Sql;

            // Previusly Calculated check sum integrity
            Assert.Equal("23066D56628CA30E68972E8F1EEAACB9", Helpers.Checksum(sql1));
        }

        [Fact]
        public void Parse_GetProjection()
        {
            Storm storm = new Storm();
            storm.EditSchema(SampleSchema);
            var compiler = new SqlServerCompiler();
            var con = storm.OpenConnection(new EmptyConnection());

            var cmd1 = con.Projection(Model_0).Select("Model_0.Field1").Select("Model_0.Child.data");

            cmd1.ParseSQL();
            SqlResult result1 = compiler.Compile(cmd1.query);
            string sql1 = result1.Sql;

            // Previusly Calculated check sum integrity
            Assert.Equal("D8401076EA9FCB0D614815BC14AE4531", Helpers.Checksum(sql1));
        }

        [Fact]
        public void Parse_GetProjection_ShortPathed()
        {
            Storm storm = new Storm();
            storm.EditSchema(SampleSchema);
            var compiler = new SqlServerCompiler();
            var con = storm.OpenConnection(new EmptyConnection());

            var cmd1 = con.Projection(Model_0).Select("Field1").Select("Child.data");

            cmd1.ParseSQL();
            SqlResult result1 = compiler.Compile(cmd1.query);
            string sql1 = result1.Sql;

            // Previusly Calculated check sum integrity
            Assert.Equal("D8401076EA9FCB0D614815BC14AE4531", Helpers.Checksum(sql1));
        }

        [Fact]
        public void Parse_GetProjection_Use_WildCard()
        {
            Storm storm = new Storm();
            storm.EditSchema(SampleSchema);
            var compiler = new SqlServerCompiler();
            var con = storm.OpenConnection(new EmptyConnection());

            var cmd1 = con.Projection(Model_0).Select("Model_0.*");

            cmd1.ParseSQL();
            SqlResult result1 = compiler.Compile(cmd1.query);
            string sql1 = result1.Sql;

            // Previusly Calculated check sum integrity
            Assert.Equal("B03BF0BC3009D5065961498447A558C0", Helpers.Checksum(sql1));
        }

        [Fact]
        public void Parse_GetProjection_Use_WildCard_ShortPathed()
        {
            Storm storm = new Storm();
            storm.EditSchema(SampleSchema);
            var compiler = new SqlServerCompiler();
            var con = storm.OpenConnection(new EmptyConnection());

            var cmd1 = con.Projection(Model_0).Select("*");

            cmd1.ParseSQL();
            SqlResult result1 = compiler.Compile(cmd1.query);
            string sql1 = result1.Sql;

            // Previusly Calculated check sum integrity
            Assert.Equal("B03BF0BC3009D5065961498447A558C0", Helpers.Checksum(sql1));
        }

        [Fact]
        public void Parse_GetProjection_Use_Bracket_List()
        {
            Storm storm = new Storm();
            storm.EditSchema(SampleSchema);
            var compiler = new SqlServerCompiler();
            var con = storm.OpenConnection(new EmptyConnection());

            var cmd1 = con.Projection(Model_0).Select("Model_0.{Field1, Field4,Field3}");

            cmd1.ParseSQL();
            SqlResult result1 = compiler.Compile(cmd1.query);
            string sql1 = result1.Sql;

            // Previusly Calculated check sum integrity
            Assert.Equal("AB1631108D6E3CDFFC67CBF51BE38286", Helpers.Checksum(sql1));
        }

        [Fact]
        public void Parse_GetProjection_Use_Bracket_List_ShortPathed()
        {
            Storm storm = new Storm();
            storm.EditSchema(SampleSchema);
            var compiler = new SqlServerCompiler();
            var con = storm.OpenConnection(new EmptyConnection());

            var cmd1 = con.Projection(Model_0).Select("{Field1, Field4,Field3}");

            cmd1.ParseSQL();
            SqlResult result1 = compiler.Compile(cmd1.query);
            string sql1 = result1.Sql;

            // Previusly Calculated check sum integrity
            Assert.Equal("AB1631108D6E3CDFFC67CBF51BE38286", Helpers.Checksum(sql1));
        } 

        [Fact]
        public void Parse_GetProjection_With_Relation_Use_WildCard()
        {
            Storm storm = new Storm();
            storm.EditSchema(SampleSchema);
            var compiler = new SqlServerCompiler();
            var con = storm.OpenConnection(new EmptyConnection());

            var cmd1 = con.Projection(Model_0).Select("Child.*");

            cmd1.ParseSQL();
            SqlResult result1 = compiler.Compile(cmd1.query);
            string sql1 = result1.Sql;

            // Previusly Calculated check sum integrity
            Assert.Equal("6CAA11070463C8B00411B8FC7A721F44", Helpers.Checksum(sql1));
        }

        [Fact]
        public void Parse_GetProjection_With_Relation_Use_Bracket_List()
        {
            Storm storm = new Storm();
            storm.EditSchema(SampleSchema);
            var compiler = new SqlServerCompiler();
            var con = storm.OpenConnection(new EmptyConnection());

            var cmd1 = con.Projection(Model_0).Select("Child.{ data }");

            cmd1.ParseSQL();
            SqlResult result1 = compiler.Compile(cmd1.query);
            string sql1 = result1.Sql;

            // Previusly Calculated check sum integrity
            Assert.Equal("2BA8729405B8B935A99702A4B14DDEB8", Helpers.Checksum(sql1));
        }

        [Fact]
        public void Parse_GetProjection_With_Where_And_Relation()
        {
            Storm storm = new Storm();
            storm.EditSchema(SampleSchema);
            var compiler = new SqlServerCompiler();
            var con = storm.OpenConnection(new EmptyConnection());

            var cmd1 = con.Projection(Model_0).Select("Child.Child_2.data");

            cmd1.ParseSQL();
            SqlResult result1 = compiler.Compile(cmd1.query);
            string sql1 = result1.Sql;

            // Previusly Calculated check sum integrity
            Assert.Equal("C1303A7DB6A3824E53A935606DD4F2B5", Helpers.Checksum(sql1));
        }


        [Fact]
        public void Parse_GetProjection_Using_JoinExpressions()
        {
            Storm storm = new Storm();
            storm.EditSchema(SampleSchema);
            var compiler = new SqlServerCompiler();
            var con = storm.OpenConnection(new EmptyConnection());


            var cmd1 = con.Projection(Model_5).Select("Child_5.data");

            cmd1.ParseSQL();
            SqlResult result1 = compiler.Compile(cmd1.query);
            string sql1 = result1.Sql;

            // Previusly Calculated check sum integrity
            Assert.Equal("C5E2C68D6792A7C0CD4C3CDC768A1825", Helpers.Checksum(sql1));

        }

        [Fact]
        public void Parse_GetProjection_Using_Pagination1()
        {
            Storm storm = new Storm();
            storm.EditSchema(SampleSchema);
            var compiler = new SqlServerCompiler();
            var con = storm.OpenConnection(new EmptyConnection());


            var cmd1 = con.Projection(Model_1)
                .Select("ID")
                .Select("Child_2.{data, ParentID}")
                .ForPage(1, 500);

            cmd1.ParseSQL();
            SqlResult result1 = compiler.Compile(cmd1.query);
            var fake_cmd = new System.Data.SqlClient.SqlCommand();
            cmd1.PopulateCommandText(fake_cmd, result1);
            string sql1 = fake_cmd.CommandText;
            // Previusly Calculated check sum integrity
            Assert.Equal("A44F4C1925EB57D7A5568B1B7491CDDD", Helpers.Checksum(sql1));
        }

        [Fact]
        public void Parse_GetProjection_Using_Pagination2()
        {
            Storm storm = new Storm();
            storm.EditSchema(SampleSchema);
            var compiler = new SqlServerCompiler();
            var con = storm.OpenConnection(new EmptyConnection());


            var cmd1 = con.Projection(Model_1)
                .Select("ID")
                .Select("Child_2.{data, ParentID}")
                .ForPage(2, 500);

            cmd1.ParseSQL();
            SqlResult result1 = compiler.Compile(cmd1.query);
            var fake_cmd = new System.Data.SqlClient.SqlCommand();
            cmd1.PopulateCommandText(fake_cmd, result1);
            string sql1 = fake_cmd.CommandText;
            // Previusly Calculated check sum integrity
            Assert.Equal("F4E725C4C7225A7115FB9272A4727F29", Helpers.Checksum(sql1));
        }

        [Fact]
        public void Parse_GetProjection_Using_Pagination3()
        {
            Storm storm = new Storm();
            storm.EditSchema(SampleSchema);
            var compiler = new SqlServerCompiler();
            var con = storm.OpenConnection(new EmptyConnection());


            var cmd1 = con.Projection(Model_1)
                .Select("ID")
                .Select("Child_2.{data, ParentID}")
                .OrderBy("Child_2.ID")
                .ForPage(2, 500);

            cmd1.ParseSQL();
            SqlResult result1 = compiler.Compile(cmd1.query);
            var fake_cmd = new System.Data.SqlClient.SqlCommand();
            cmd1.PopulateCommandText(fake_cmd, result1);
            string sql1 = fake_cmd.CommandText;
            // Previusly Calculated check sum integrity
            Assert.Equal("23662B1C3314AA9ED4805B976CDE1E45", Helpers.Checksum(sql1));
        }

        [Fact]
        public void Parse_SetCommand_Insert()
        {
            Storm storm = new Storm();
            storm.EditSchema(SampleSchema);
            var compiler = new SqlServerCompiler();
            var con = storm.OpenConnection(new EmptyConnection());

            var cmd1 = con.Insert(Model_0)
                .Value(new Dictionary<string, object>() {
                    {"ID","asd"}, //<= this shold not take in consideration because it's primary key of the entity
                    {"Model1ID","1" },
                    {"Field1","value1" },
                    {"Field2","value2" },
                    {"Field3","value3" },
                    {"Field4","value4" },
                    {"Field5","value5" },
                    {"NotExistingField","some value" } // <= this shold not take in consideration because it's not a field of the entity
                });

            cmd1.ParseSQL();
            SqlResult result1 = compiler.Compile(cmd1.query);
            string sql1 = result1.Sql;

            // Previusly Calculated check sum integrity
            Assert.Equal("EBF6145BC25366C39C939E8B13B9DDA6", Helpers.Checksum(sql1));
        }

        [Fact]
        public void Parse_SetCommand_Insert_UsingModel()
        {
            Storm storm = new Storm();
            storm.EditSchema(SampleSchema);
            var compiler = new SqlServerCompiler();
            var con = storm.OpenConnection(new EmptyConnection());

            var cmd1 = con.Insert(Model_0)
                .Value(new Model_0() {
                    ID= 12, //<= this shold not take in consideration because it's primary key of the entity
                    Model1ID =1,
                    Field1= "value1",
                    Field2= "value2",
                    Field3= "value3",
                    Field4= "value4",
                    Field5= "value5",
                });


            cmd1.ParseSQL();
            SqlResult result1 = compiler.Compile(cmd1.query);
            string sql1 = result1.Sql;

            // Previusly Calculated check sum integrity
            Assert.Equal("EBF6145BC25366C39C939E8B13B9DDA6", Helpers.Checksum(sql1));
        }


        [Fact]
        public void Parse_SetCommand_Update()
        {
            Storm storm = new Storm();
            storm.EditSchema(SampleSchema);
            var compiler = new SqlServerCompiler();
            var con = storm.OpenConnection(new EmptyConnection());

            var cmd1 = con.Update(Model_0, 12)
                .Value(new Dictionary<string, object>() {
                    {"ID","asd"}, //<= this shold not take in consideration because it's primary key of the entity
                    {"Model1ID","1" },
                    {"Field1","value1" },
                    {"Field2","value2" },
                    {"Field3","value3" },
                    {"Field4","value4" },
                    {"Field5","value5" },
                    {"NotExistingField","some value" } // <= this shold not take in consideration because it's not a field of the entity
                });

            cmd1.ParseSQL();
            SqlResult result1 = compiler.Compile(cmd1.query);
            string sql1 = result1.Sql;

            // Previusly Calculated check sum integrity
            Assert.Equal("ADD301250F02D5AF0240ACE77079032E", Helpers.Checksum(sql1));
        }

        [Fact]
        public void Parse_SetCommand_Update_UsingModel()
        {
            Storm storm = new Storm();
            storm.EditSchema(SampleSchema);
            var compiler = new SqlServerCompiler();
            var con = storm.OpenConnection(new EmptyConnection());

            var cmd1 = con.Update(Model_0, 12)
                .Value(new Model_0()
                {
                    ID = 12, //<= this shold not take in consideration because it's primary key of the entity
                    Model1ID = 1,
                    Field1 = "value1",
                    Field2 = "value2",
                    Field3 = "value3",
                    Field4 = "value4",
                    Field5 = "value5",
                });


            cmd1.ParseSQL();
            SqlResult result1 = compiler.Compile(cmd1.query);
            string sql1 = result1.Sql;

            // Previusly Calculated check sum integrity
            Assert.Equal("ADD301250F02D5AF0240ACE77079032E", Helpers.Checksum(sql1));
        }


        [Fact]
        public void Parse_DeleteCommand_NoFilter()
        {
            Storm storm = new Storm();
            storm.EditSchema(SampleSchema);
            var compiler = new SqlServerCompiler();
            var con = storm.OpenConnection(new EmptyConnection());

            var cmd1 = con.Delete(Model_0);

            cmd1.ParseSQL();
            SqlResult result1 = compiler.Compile(cmd1.query);
            string sql1 = result1.Sql;

            // Previusly Calculated check sum integrity
            Assert.Equal("3671297C80C511F80B64CF061D91CA6F", Helpers.Checksum(sql1));

        }

        [Fact]
        public void Parse_DeleteCommand_SimpleFilter()
        {
            Storm storm = new Storm();
            storm.EditSchema(SampleSchema);
            var compiler = new SqlServerCompiler();
            var con = storm.OpenConnection(new EmptyConnection());

            var cmd1 = con.Delete(Model_0).Where(x => x["Field1"].IsNotNull * x["Field2"].GreaterTo.Val(100));

            cmd1.ParseSQL();
            SqlResult result1 = compiler.Compile(cmd1.query);
            string sql1 = result1.Sql;

            // Previusly Calculated check sum integrity
            Assert.Equal("4EAAE38C3FD27E0728FE85D172227FD7", Helpers.Checksum(sql1));
        }

        [Fact]
        public void Parse_DeleteCommand_FilterInSecondaryEntity()
        {
            Storm storm = new Storm();
            storm.EditSchema(SampleSchema);
            var compiler = new SqlServerCompiler();
            var con = storm.OpenConnection(new EmptyConnection());

            var cmd1 = con.Delete(Model_0).Where(x => x["Child.Child_2.data"].IsNotNull * x["Field2"].GreaterTo.Val(100));

            cmd1.ParseSQL();
            SqlResult result1 = compiler.Compile(cmd1.query);
            string sql1 = result1.Sql;

            // Previusly Calculated check sum integrity
            Assert.Equal("3E640C0F9D2EAF88BA7050CE7B8C1374", Helpers.Checksum(sql1));
        }

    }
}
