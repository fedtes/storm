using System;
using System.Collections.Generic;
using System.Text;
using Storm.Schema;
using Xunit;
using Storm;
using Storm.Test.MockAndModels;
using System.Linq;
using System.Threading.Tasks;

namespace Storm.Test.TestUnits
{
    public class TestingSchema
    {
        [Fact]
        public void Assert_Editor_Creation()
        {
            Storm storm = new Storm();
            storm.EditSchema(e =>
            {
                Assert.NotNull(e);
                Assert.True(DateTime.Now.Ticks > e.ticks && e.ticks != 0);
                return e;
            });
        }

        [Fact]
        public void Assert_Add_TypedModel()
        {
            Storm storm = new Storm();
            storm.EditSchema(e => e.Add<TestModel>("TestModel", "TABTest"));
            var nav = storm.schema.GetNavigator();
            var x = nav.GetEntity("TestModel");
            Assert.Equal("TABTest", x.DBName);
            Assert.Equal("TestModel", x.ID);
            Assert.Equal("ID", x.PrimaryKey.CodeName);
            Assert.Equal(typeof(TestModel), x.TModel);
        }

        [Fact]
        public void Assert_It_Read_Property_Of_TypedModel()
        {
            Storm storm = new Storm();
            storm.EditSchema(e => e.Add<TestModel>("TestModel", "TABTest"));
            var nav = storm.schema.GetNavigator();
            var x = nav.GetEntity("TestModel");
            Assert.Contains(x.entityFields, y => y.CodeName == "SomeProperty");
        }

        [Fact]
        public void Assert_It_Read_Attr_StormIgnore_TypedModel()
        {
            Storm storm = new Storm();
            storm.EditSchema(e => e.Add<TestModel>("TestModel", "TABTest"));
            var nav = storm.schema.GetNavigator();
            var x = nav.GetEntity("TestModel");
            Assert.DoesNotContain(x.entityFields, y => y.CodeName == "IgnorableField");
        }

        [Fact]
        public void Assert_It_Read_Attr_StormColumnName_TypedModel()
        {
            Storm storm = new Storm();
            storm.EditSchema(e => e.Add<TestModel>("TestModel", "TABTest"));
            var nav = storm.schema.GetNavigator();
            var x = nav.GetEntity("TestModel");
            Assert.Equal("DifferentName", x.entityFields.First(y => y.CodeName == "SomeName").DBName);
        }

        [Fact]
        public void Assert_It_Read_Attr_StormDefaultIfNull_TypedModel()
        {
            Storm storm = new Storm();
            storm.EditSchema(e => e.Add<TestModel>("TestModel", "TABTest"));
            var nav = storm.schema.GetNavigator();
            var x = nav.GetEntity("TestModel");
            Assert.Equal("Some Default Value", x.entityFields.First(y => y.CodeName == "DefaultedField").DefaultIfNull);
        }

        [Fact]
        public void Assert_It_Read_Attr_StormColumnType_TypedModel()
        {
            Storm storm = new Storm();
            storm.EditSchema(e => e.Add<TestModel>("TestModel", "TABTest"));
            var nav = storm.schema.GetNavigator();
            var x = nav.GetEntity("TestModel");
            Assert.Equal(System.Data.DbType.String, x.entityFields.First(y => y.CodeName == "SpecifiedType").DBType);
            Assert.Equal(50, x.entityFields.First(y => y.CodeName == "SpecifiedType").Size);
        }

        [Fact]
        public void Assert_It_Read_Attr_StormColumnAccess_TypedModel()
        {
            Storm storm = new Storm();
            storm.EditSchema(e => e.Add<TestModel>("TestModel", "TABTest"));
            var nav = storm.schema.GetNavigator();
            var x = nav.GetEntity("TestModel");
            Assert.Equal(ColumnAccess.ReadOnly, x.entityFields.First(y => y.CodeName == "ReadOnlyField").ColumnAccess);
            Assert.Equal(ColumnAccess.CanInsert, x.entityFields.First(y => y.CodeName == "InsertOnly").ColumnAccess);
            Assert.Equal(ColumnAccess.Full, x.entityFields.First(y => y.CodeName == "FullControl").ColumnAccess);
        }

        [Fact]
        public void Assert_Add_DynamicModel()
        {
            Storm storm = new Storm();
            storm.EditSchema(e =>
            {
                e.Add("SomeDynModel", "Table", b => b.Add(new FieldConfig() { CodeName = "ID", IsPrimary = true, CodeType = typeof(int) }));
                return e;
            });
            var nav = storm.schema.GetNavigator();
            var x = nav.GetEntity("SomeDynModel");
            Assert.Equal("Table", x.DBName);
            Assert.Equal("SomeDynModel", x.ID);
            Assert.Equal("ID", x.PrimaryKey.CodeName);
        }

        [Fact]
        public void Throw_If_No_PrimaryKey_In_TypedModel()
        {
            Storm storm = new Storm();
            Assert.Throws<NoPrimaryKeySpecifiedException>(()=> storm.EditSchema(e => e.Add<NoPrimaryKeyTestModel>("TestModel", "TABTest")));
        }

        [Fact]
        public void Throw_If_No_PrimaryKey_In_DynamicModel()
        {
            Storm storm = new Storm();
            Assert.Throws<NoPrimaryKeySpecifiedException>(() =>
            {
                storm.EditSchema(e =>
                {
                    e.Add("SomeDynModel", "Table", b => b.Add(new FieldConfig() { CodeName = "ID", CodeType = typeof(int) }));
                    return e;
                });
            });
        }


        [Fact]
        public void Assert_Should_Handle_Schema_Versioning()
        {
            Storm storm = new Storm();
            storm.EditSchema(e => e.Add<TestModel>("TestModel1", "TABTest"));
            var nav = storm.schema.GetNavigator();
            var x = nav.GetEntity("TestModel1");
            var y = nav.GetEntity("TestModel2");
            Assert.NotNull(x);
            Assert.Null(y);

            storm.EditSchema(e => e.Add<TestModel>("TestModel2", "TABTest"));
            nav = storm.schema.GetNavigator();
            x = nav.GetEntity("TestModel1");
            y = nav.GetEntity("TestModel2");
            Assert.NotNull(x);
            Assert.NotNull(y);
        }

        [Fact]
        public void Assert_Navigation_Keep_Schema_Version()
        {
            Storm storm = new Storm();
            storm.EditSchema(e => e.Add<TestModel>("TestModel1", "TABTest"));

            var nav = storm.schema.GetNavigator();

            storm.EditSchema(e => e.Add<TestModel>("TestModel2", "TABTest"));
            
            var x = nav.GetEntity("TestModel1");
            var y = nav.GetEntity("TestModel2");
            Assert.NotNull(x);
            Assert.Null(y);

        }

        [Fact]
        public void Throw_If_Editor_OutOfDate()
        {
            Storm storm = new Storm();
            Task.Factory.StartNew(() =>
            {
                Assert.Throws<SchemaOutOfDateException>(() =>
                {
                    storm.EditSchema(e =>
                    {
                        System.Threading.Thread.Sleep(10);
                        return e;
                    });
                });
            });

            storm.EditSchema(e =>
            {
                return e;
            });
        }

        [Fact]
        public void Assert_Schema_Handle_Multiple_Entitiy()
        {
            Storm storm = new Storm();
            storm.EditSchema(e =>
            {
                e.Add<TestModel>("TestModel", "TABTest");
                e.Add("SomeDynModel", "Table", b => b.Add(new FieldConfig() { CodeName = "ID", IsPrimary = true, CodeType = typeof(int) }));
                return e;
            });

            var nav = storm.schema.GetNavigator();
            var x = nav.GetEntity("TestModel");
            var y = nav.GetEntity("SomeDynModel");
            Assert.NotNull(x);
            Assert.NotNull(y);
        }

        [Fact]
        public void Assert_Add_Connection_Between_Enties()
        {
            Storm storm = new Storm();
            storm.EditSchema(e =>
            {
                e.Add<TestModel>("TestModel", "TABTest");
                e.Add("SomeDynModel", "Table", b => 
                {
                    b.Add(new FieldConfig() { CodeName = "ID", IsPrimary = true, CodeType = typeof(int) });
                    b.Add(new FieldConfig() { CodeName = "TestModelID", CodeType = typeof(int) });
                    return b;
                });

                e.Connect("TestModel", "SomeDynModel", "TestModel", "TestModelID", "ID");
                return e;
            });

            var nav = storm.schema.GetNavigator();
            var s = nav.GetEntity("SomeDynModel");
            var t = nav.GetEntity("TestModel");
            var x = nav.GetEdge("SomeDynModel.TestModel");
            Assert.Equal(s.ID, x.SourceID);
            Assert.Equal(t.ID, x.TargetID);
            Assert.Equal("TestModelID", x.On.Item1);
            Assert.Equal("ID", x.On.Item2);
        }

        [Fact]
        public void Assert_Add_Connection_Using_Expression()
        {
            Storm storm = new Storm();
            storm.EditSchema(e =>
            {
                e.Add<TestModel>("TestModel", "TABTest");
                e.Add("SomeDynModel", "Table", b =>
                {
                    b.Add(new FieldConfig() { CodeName = "ID", IsPrimary = true, CodeType = typeof(int) });
                    b.Add(new FieldConfig() { CodeName = "TestModelID", CodeType = typeof(int) });
                    return b;
                });

                e.Connect("TestModel", "SomeDynModel", "TestModel", ctx => ctx["source.TestModelID"].EqualTo.Ref("target.ID"));
                return e;
            });

            var nav = storm.schema.GetNavigator();
            var s = nav.GetEntity("SomeDynModel");
            var t = nav.GetEntity("TestModel");
            var x = nav.GetEdge("SomeDynModel.TestModel");
            Assert.Equal(s.ID, x.SourceID);
            Assert.Equal(t.ID, x.TargetID);
            Assert.NotNull(x.OnExpression);
            var f = Assert.IsType<Filters.EqualToFilter>(x.OnExpression);
            Assert.Equal("source.TestModelID", f.Left.Path);
            Assert.Equal("target.ID", ((Filters.ReferenceFilterValue)f.Right).Path);
        }

        [Fact]
        public void Throw_If_Entity_With_Same_ID_Exists()
        {
            Storm storm = new Storm();
            Assert.Throws<ArgumentException>(() =>
            {
                storm.EditSchema(e =>
                {
                    e.Add<TestModel>("TestModel", "TABTest");
                    e.Add<TestModel>("TestModel", "TABTest2");
                    return e;
                });
            });
        }

        [Fact]
        public void Throw_If_Connection_With_Same_ID_Exists()
        {
            Storm storm = new Storm();
            Assert.Throws<ArgumentException>(() =>
            {
                storm.EditSchema(e =>
                {
                    e.Add<TestModel>("TestModel", "TABTest");
                    e.Add("SomeDynModel", "Table", b =>
                    {
                        b.Add(new FieldConfig() { CodeName = "ID", IsPrimary = true, CodeType = typeof(int) });
                        b.Add(new FieldConfig() { CodeName = "TestModelID", CodeType = typeof(int) });
                        return b;
                    });

                    e.Connect("TestModel", "SomeDynModel", "TestModel", "TestModelID", "ID");
                    e.Connect("TestModel", "SomeDynModel", "TestModel", "TestModelID", "ID");
                    return e;
                });
            });
        }

        [Fact]
        public void Throw_If_Connected_Entity_Not_Exists()
        {
            Storm storm = new Storm();
            Assert.Throws<ArgumentException>(() =>
            {
                storm.EditSchema(e =>
                {
                    e.Add<TestModel>("TestModel", "TABTest");
                    e.Connect("TestModel", "SomeDynModel", "TestModel", "TestModelID", "ID");
                    return e;
                });
            });
        }

        

    }
}
