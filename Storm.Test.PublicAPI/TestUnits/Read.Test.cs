using Xunit;
using Storm.Execution;
using System.Linq;
using Storm.Test.PublicAPI.Helpers;

namespace Storm.Test.PublicAPI
{
    public class Read
    {

        
        [Fact]
        public void It_should_have_prefills_db()
        {
            var db = PrepMethods.PrepareDB();

            var cmd = db.CreateCommand();
            cmd.CommandText = "SELECT COUNT(*) FROM Users";
            int cnt = 0;
            using (var r = cmd.ExecuteReader())
            {
                while (r.Read())
                {
                    cnt = r.GetInt32(0);
                }
            }

            Assert.True(cnt > 0);

            cmd = db.CreateCommand();
            cmd.CommandText = "SELECT COUNT(*) FROM Tasks";
            cnt = 0;
            using (var r = cmd.ExecuteReader())
            {
                while (r.Read())
                {
                    cnt = r.GetInt32(0);
                }
            }

            Assert.True(cnt > 0);

            cmd = db.CreateCommand();
            cmd.CommandText = "SELECT COUNT(*) FROM TaskInfos";
            cnt = 0;
            using (var r = cmd.ExecuteReader())
            {
                while (r.Read())
                {
                    cnt = r.GetInt32(0);
                }
            }

            Assert.True(cnt > 0);

            db.Close();
        }


        [Fact]
        public async void It_should_get_users()
        {
            var s = PrepMethods.PrepareStorm();
            StormDataSet result;
            await using (var stormConnection = await s.OpenConnection(PrepMethods.PrepareDB()))
            {
                await using (var tran = stormConnection.BeginTransaction())
                {
                    result =await tran.Projection("User")
                        .Select("ID")
                        .Select("FirstName")
                        .Select("LastName")
                        .Execute();
                }
            }

            Assert.Equal(20, result.Count());
        }



    }
}
