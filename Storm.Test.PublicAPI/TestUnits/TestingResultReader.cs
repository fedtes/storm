using Storm.Execution.Results;
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
                    .Execute()
                    .Cast<dynamic>();

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
                    .Execute()
                    .Cast<dynamic>();

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
                    .Execute()
                    .Cast<dynamic>();

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



    }
}
