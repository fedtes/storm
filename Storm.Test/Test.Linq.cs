using System.IO.Compression;
using System.Linq;
using Storm.Schema;
using Xunit;
using Storm.Linq;
using Microsoft.Data.Sqlite;

namespace Storm.Test.TestUnits
{
    public class TestLinq 
    {

        [Fact]
        public async void Parse_GetObject_OnlyRoot()
        {
            var db = new SqliteConnection("Data Source=TestDB;Mode=Memory;");
            db.Open();
            var cmd = db.CreateCommand();
            cmd.CommandText = @"
                CREATE TABLE TABCustomer (
                    CustomerID INTEGER PRIMARY KEY AUTOINCREMENT,
                    AddressID INTEGER NULL,
                    RagSoc TEXT
                );

                CREATE TABLE TABAddress (
                    AddressID INTEGER PRIMARY KEY AUTOINCREMENT,
                    City TEXT,
                    Street TEXT,
                    Number INTEGER
                );

                CREATE TABLE TABLocation (
                    LocationID INTEGER PRIMARY KEY AUTOINCREMENT,
                    CustomerID TEXT,
                    Name TEXT
                );

                INSERT INTO TABAddress (City, Street, Number) VALUES ('New York', 'Main street', 23);
                INSERT INTO TABCustomer (AddressID, RagSoc) VALUES (1, 'ACME SRL');
            ";
            cmd.ExecuteNonQuery();

            Storm storm = new Storm();
            storm.EditSchema(DynamicModelRealistic.SampleSchema);
            await using (var con = await storm.OpenConnection(db))
            {
                 var result = await con.From("Customer")
                    .Where(x => x["RagSoc"].Like("ACME") || (x["Address.City"] == "New York" && x["AddressID"].IsNotNull()))
                    // .OrderBy(x => x["RagSoc"])
                    // .Take(10)
                    // .Select("RagSoc")
                    .ToListAsync();
                
                Assert.Equal("ACME SRL", result.First().RagSoc.ToString());

            }
        }

        [Fact]
        public async void Parse_GetObject_OnlyRoot2()
        {

            Storm storm = new Storm();
            storm.EditSchema(DynamicModelRealistic.SampleSchema);
            await using (var con = await storm.OpenConnection(new EmptyConnection()))
            {
                 var result = (await con.From("Customer")
                    .Where(x => x["RagSoc"].Like("ACME") || (x["Address.City"] == "New York" && x["AddressID"].IsNotNull()))
                    // .OrderBy(x => x["RagSoc"])
                    // .Take(10)
                    .Select("RagSoc")
                    .ToListAsync())
                    .Select(x => new { b=x.RagSoc});
                
                Assert.Equal("ACME SRL", result.First().b.ToString());

            }
        }

    }
}