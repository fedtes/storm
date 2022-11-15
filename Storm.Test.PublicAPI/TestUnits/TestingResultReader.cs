using Storm.Execution;
using Storm.Test.PublicAPI.Helpers;
using Storm.Test.PublicAPI.MockAndModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace Storm.Test.PublicAPI
{
    public class TestingResultReader
    {
        public Storm storm;

        public TestingResultReader()
        {
            storm = PrepMethods.PrepareStorm();
        }

        [Fact]
        public void Should_Read_Projection_Result()
        {
            using (var con = storm.OpenConnection(PrepMethods.PrepareDB()))
            {
                StormDataSet r = con.Projection("Task")
                    .Select("User.{FirstName, LastName}")
                    .Select("TaskType")
                    .Select("Completed")
                    .Select("{DateStart, DateEnd}")
                    .Select("Info.Field2")
                    .Where(x => x["User.Email"].EqualTo.Val("wprestwich2@deviantart.com"))
                    .Execute();

                Assert.Equal(8, r.Count());
                Assert.Equal(2, r.Count(x => (bool)x["Completed"] == true));

            }
        }

        [Fact]
        public void Should_Read_Paginated_Projection_Result()
        {
            using (var con = storm.OpenConnection(PrepMethods.PrepareDB()))
            {
                StormDataSet r = con.Projection("User")
                    .Select("{FirstName, LastName}")
                    .ForPage(1,5)
                    .Execute();

                Assert.Equal(5, r.Count());
                Assert.Equal("Kaila", r.Last()["FirstName"]);

            }
        }

        [Fact]
        public void Should_Read_Paginated_Projection_Result_2()
        {
            using (var con = storm.OpenConnection(PrepMethods.PrepareDB()))
            {
                StormDataSet r = con.Projection("User")
                    .Select("{FirstName, LastName}")
                    .ForPage(2, 5)
                    .Execute();

                Assert.Equal(5, r.Count());
                Assert.Equal("Katerine", r.Last()["FirstName"]);

            }
        }

        [Fact]
        public void Should_Read_Paginated_Projection_Result_3()
        {
            using (var con = storm.OpenConnection(PrepMethods.PrepareDB()))
            {
                StormDataSet r = con.Projection("User")
                    .Select("{FirstName, LastName}")
                    .ForPage(2, 5)
                    .OrderBy("ID",false)
                    .Where(x => x["ID"].LessOrEqualTo.Val(10))
                    .Execute();

                Assert.Equal(5, r.Count());
                Assert.Equal("Goldarina", r.Last()["FirstName"]);

            }
        }

        [Fact]
        public void Should_Read_Get_Result_OneModel_Single()
        {
            using (var con = storm.OpenConnection(PrepMethods.PrepareDB()))
            {
                var r = con.Get("Task")
                    .Where(x => x["ID"].EqualTo.Val("10"))
                    .Execute();

                Assert.Single(r);

                var task = r.First();
                Assert.Equal(10, task.ID);
                Assert.Equal(15, task.UserID);
                Assert.Equal("Todo", task.TaskType);
                Assert.Equal("augue a suscipit nulla elit ac nulla sed vel enim", task.Subject);
                Assert.Equal(new DateTime(2021,6,22), task.DateStart);
                Assert.Equal(new DateTime(2021,3,28), task.DateEnd);

            }
        }

        [Fact]
        public void Should_Read_Get_Result_OneModel_Single_TypedModel()
        {
            using (var con = storm.OpenConnection(PrepMethods.PrepareDB()))
            {
                var r = con.Get("Task")
                    .Where(x => x["ID"].EqualTo.Val("10"))
                    .Execute();

                Assert.Single(r);

                var task = r.First().GetModel<Task>();
                Assert.Equal(10, task.ID);
                Assert.Equal(15, task.UserID);
                Assert.Equal("Todo", task.TaskType);
                Assert.Equal("augue a suscipit nulla elit ac nulla sed vel enim", task.Subject);
                Assert.Equal(new DateTime(2021, 6, 22), task.DateStart);
                Assert.Equal(new DateTime(2021, 3, 28), task.DateEnd);
            }
        }

        [Fact]
        public void Should_Read_Get_Result_2Model_Single()
        {
            using (var con = storm.OpenConnection(PrepMethods.PrepareDB()))
            {
                var r = con.Get("Task")
                    .With("User")
                    .Where(x => x["ID"].EqualTo.Val("10"))
                    .Execute();

                Assert.Single(r);
                var user = r.First().User[0];
                Assert.Equal(15, user.ID);
                Assert.Equal("Jannelle", user.FirstName);
                Assert.Equal("Lawles", user.LastName);
                Assert.Equal("jlawlese@state.gov", user.Email);
            }
        }


        [Fact]
        public void Should_Read_Get_Result_3Model_Single()
        {
            using (var con = storm.OpenConnection(PrepMethods.PrepareDB()))
            {
                var r = con.Get("Task")
                    .With("User")
                    .With("Info")
                    .Where(x => x["Info.ID"].EqualTo.Val("10"))
                    .Execute();

                Assert.Single(r);
                var user = r.First().GetRelation("User")[0].GetModel<User>();
                Assert.Equal(15, user.ID);
                Assert.Equal("Jannelle", user.FirstName);
                Assert.Equal("Lawles", user.LastName);
                Assert.Equal("jlawlese@state.gov", user.Email);

                var info = r.First().Info[0];
                Assert.Equal("cubilia", info.Field1);
                Assert.Equal(911, info.Field2);
                Assert.Equal(true, info.Field3);

            }
        }


        [Fact]
        public void Should_Read_Get_Result_3Model_Multiple()
        {
            using (var con = storm.OpenConnection(PrepMethods.PrepareDB()))
            {
                var r = con.Get("Task")
                    .With("User")
                    .With("Info")
                    .Where(x => x["Info.ID"].EqualTo.Val("10") + x["ID"].EqualTo.Val("11"))
                    .Execute();

                Assert.Equal(2, r.Count());

                var res1 = r.First(x => x.ID == 10);

                var task1 = res1.GetModel<Task>();
                var user1 = res1.User[0].GetModel<User>();
                var info1 = res1.GetRelation("Info")[0];

                Assert.Equal("augue a suscipit nulla elit ac nulla sed vel enim", task1.Subject);
                Assert.Equal("jlawlese@state.gov", user1.Email);
                Assert.Equal("cubilia", info1.Field1);

                var res2 = r.First(x => x.ID == 11);

                var task2 = res2.GetModel<Task>();
                var user2 = res2.GetRelation("User")[0].GetModel<User>();
                var info2 = res2.Info[0];

                Assert.Equal("aliquet maecenas leo odio condimentum id luctus nec molestie sed justo", task2.Subject);
                Assert.Equal("mmorgand@pbs.org", user2.Email);
                Assert.Equal("nullam porttitor lacus at turpis donec posuere metus vitae", info2.Field1);
            }
        }


        [Fact]
        public void Should_Add_NewElement()
        {
            using (var con = storm.OpenConnection(PrepMethods.PrepareDB()))
            {
                var r = con.Insert("Task")
                    .Value(new Task()
                    {
                        Completed = false,
                        Subject = "New Task element",
                        TaskType = "test",
                        UserID = 1,
                        DateStart = new DateTime(2021, 04, 21, 14, 00, 00),
                        DateEnd = new DateTime(2021, 04, 21, 15, 00, 00),
                        ID = 999999 /// should be ignored
                    }).Execute();

                var rows = r.RowsAffected;
                var id = r.ObjectId;

                Assert.NotEqual(999999, id);
                Assert.Equal(1, rows);


                var task = con.Get("Task").Where(f => f["ID"].EqualTo.Val(id)).Execute().First();

                Assert.False(task.Completed);
                Assert.Equal("New Task element", task.Subject);
                Assert.Equal("test", task.TaskType);
                Assert.Equal(1, task.UserID);
                Assert.Equal(new DateTime(2021, 04, 21, 14, 00, 00), task.DateStart);
                Assert.Equal(new DateTime(2021, 04, 21, 15, 00, 00), task.DateEnd);
                Assert.Equal(id.ToString(), task.ID.ToString());

            }
        }

        [Fact]
        public void Should_Update_Element()
        {
            using (var con = storm.OpenConnection(PrepMethods.PrepareDB()))
            {

                var task = con.Get("Task").Where(f => f["ID"].EqualTo.Val(1)).Execute().First();

                var r = con.Update("Task", task.ID)
                    .Value(new Task()
                    {
                        Completed = task.Completed,
                        Subject = task.Subject,
                        TaskType = task.TaskType,
                        UserID = task.UserID,
                        DateStart = ((DateTime)task.DateStart).AddDays(1),
                        DateEnd = ((DateTime)task.DateEnd).AddDays(2),
                    }).Execute();

                var rows = r.RowsAffected;
                var id = r.ObjectId;

                Assert.Equal(task.ID, id);
                Assert.Equal(1, rows);


                var task2 = con.Get("Task").Where(f => f["ID"].EqualTo.Val(id)).Execute().First();

                Assert.Equal(task.Completed, task2.Completed);
                Assert.Equal(task.Subject, task2.Subject);
                Assert.Equal(task.TaskType, task2.TaskType);
                Assert.Equal(task.UserID, task2.UserID);
                Assert.Equal(((DateTime)task.DateStart).AddDays(1), task2.DateStart);
                Assert.Equal(((DateTime)task.DateEnd).AddDays(2), task2.DateEnd);

            }
        }


        [Fact]
        public void Should_NotUpdate_Element()
        {
            using (var con = storm.OpenConnection(PrepMethods.PrepareDB()))
            {

                var r = con.Update("Task", 99999) // id not exists!!
                    .Value(new Task()
                    {
                        Completed = false,
                        Subject = "New Task element",
                        TaskType = "test",
                        UserID = 1,
                        DateStart = new DateTime(2021, 04, 21, 14, 00, 00),
                        DateEnd = new DateTime(2021, 04, 21, 15, 00, 00)
                    }).Execute();

                var rows = r.RowsAffected;
                var id = r.ObjectId;

                Assert.Equal(0, rows);

                var elements = con.Get("Task").Where(f => f["ID"].EqualTo.Val(99999)).Execute();
                Assert.Empty(elements);

            }
        }


        /* ----  Testing Nullable values ---- */

        public class NullableTask
        {
            [Schema.StormPrimaryKey]
            public int ID;
            public int? UserID;
            public string TaskType;
            public bool? Completed;
            [Schema.StormColumnName("ShortDesc")]
            public string Subject;
            public DateTime? DateStart;
            public DateTime? DateEnd;
        }

        [Fact]
        public void It_ShouldReadNullable_Types()
        {
            var s = new Storm(SQLEngine.SQLite);
            s.EditSchema(e => e.Add<NullableTask>("Task", "Tasks"));
            using (var con = s.OpenConnection(PrepMethods.PrepareDB()))
            {
                NullableTask nt =  con.Get("Task").ForPage(1, 1).Execute().First().GetModel<NullableTask>();
                Assert.NotNull(nt.UserID);
                Assert.NotNull(nt.Completed);
                Assert.NotNull(nt.DateStart);
                Assert.NotNull(nt.DateEnd);
            }
        }


    }
}
