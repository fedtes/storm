using Storm.Schema;

namespace Storm.Test
{
    public static class DynamicModelRealistic 
    {
        public static SchemaModelBuilder SampleSchema(SchemaModelBuilder e)
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
