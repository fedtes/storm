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
            storm.EditSchema(SampleSchema);
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
            storm.EditSchema(SampleSchema);
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




        public SchemaModelBuilder SampleSchema(SchemaModelBuilder e)
        {
            return e.Add("Customer", "TABCustomer", eb => {
                return eb.AddPrimary("CustomerID", typeof(int))
                    .Add("AddressID", typeof(int))
                    .Add("RagSoc", typeof(string));
            })
            .Add("Address", "TABAddress", eb => {
                return eb.AddPrimary("AddressID", typeof(int))
                    .Add("City", typeof(string))
                    .Add("Street", typeof(string))
                    .Add("Number", typeof(int));
            })
            .Add("Location", "TABLocation", eb => {
                return eb.AddPrimary("LocationID", typeof(int))
                    .Add("CustomerID", typeof(int))
                    .Add("AddressID", typeof(int))
                    .Add("Name", typeof(string));
            })
            .Connect("Address", "Customer", "Address", "AddressID", "AddressID")
            .Connect("Locations", "Customer", "Location", "CustomerID", "CustomerID")
            .Connect("Address", "Location", "Address", "AddressID", "AddressID");
        }
    }
}