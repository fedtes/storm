using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using SqlKata;
using SqlKata.Compilers;
using Storm.Schema;
using Xunit;

namespace Storm.Test.TestUnits 
{
    public class TestLinq 
    {

         [Fact]
        public async void Parse_GetObject_OnlyRoot()
        {
            Storm storm = new Storm();
            storm.EditSchema(SampleSchema);
            var con = await storm.OpenConnection(new EmptyConnection());

            var cmd = con.From("Customer").Where(x => x["Address.City"] == "TEST" ).ToArray();
            var cmd2 = con.From("Customer").Where(x => x["Address.City"] == x["RagSoc"] ).ToArray();
        }




        public SchemaEditor SampleSchema(SchemaEditor e)
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