using Storm.Schema;
using Xunit;

namespace Storm.Test
{
    public class TestingNavigator
    {

        [Fact]
        public void GetEntity()
        {
            Storm storm = new Storm();
            storm.EditSchema(DynamicModelRealistic.SampleSchema);
            var nav = storm.schema.GetNavigator();
            Assert.IsType<Entity>(nav.GetEntity("Location"));
            Assert.Equal("LocationID", nav.GetEntity("Location").PrimaryKey.Id);
        }

        [Fact]
        public void GetNavigationProperty()
        {
            Storm storm = new Storm();
            storm.EditSchema(DynamicModelRealistic.SampleSchema);
            var nav = storm.schema.GetNavigator();
            Assert.IsType<NavigationProperty>(nav.GetNavigationProperty("Customer.Address"));
            Assert.Equal("Customer", nav.GetNavigationProperty("Customer.Address").OwnerEntityId);
        }


        [Fact]
        public void GetSimplePropertyByPath()
        {
            Storm storm = new Storm();
            storm.EditSchema(DynamicModelRealistic.SampleSchema);
            var nav = storm.schema.GetNavigator();
            Assert.IsType<SimpleProperty>(nav.Get(new Path("Customer.Locations.Address.City")));
            Assert.Equal("City", nav.Get(new Path("Customer.Locations.Address.City")).Id);
        }

        [Fact]
        public void GetSimplePropertyByStringPath()
        {
            Storm storm = new Storm();
            storm.EditSchema(DynamicModelRealistic.SampleSchema);
            var nav = storm.schema.GetNavigator();
            Assert.IsType<SimpleProperty>(nav.Get("Customer.Locations.Address.City"));
            Assert.Equal("City", nav.Get("Customer.Locations.Address.City").Id);
        }

        [Fact]
        public void GetNavigationPropertyByPath()
        {
            Storm storm = new Storm();
            storm.EditSchema(DynamicModelRealistic.SampleSchema);
            var nav = storm.schema.GetNavigator();
            Assert.IsType<NavigationProperty>(nav.Get(new Path("Customer.Locations.Address")));
            Assert.Equal("Address", nav.Get<NavigationProperty>(new Path("Customer.Locations.Address")).TargetEntity);
        }

        [Fact]
        public void GetNavigationPropertyByStringPath()
        {
            Storm storm = new Storm();
            storm.EditSchema(DynamicModelRealistic.SampleSchema);
            var nav = storm.schema.GetNavigator();
            Assert.IsType<NavigationProperty>(nav.Get("Customer.Locations.Address"));
            Assert.Equal("Address", nav.Get<NavigationProperty>("Customer.Locations.Address").TargetEntity);
        }

        
    }
}