using System.Linq;
using System.Runtime;
using Storm.Execution;
using Storm.Schema;
using Xunit;

namespace Storm.Test
{

    public class ReadCommandTest
    {
        [Fact]
        public void It_should_Init_TableTree()
        {
            Storm storm = new Storm();
            storm.EditSchema(DynamicModelRealistic.SampleSchema);
            
            TableTree tree = new TableTree(storm.CreateContext(), "Customer");
            Assert.Equal("Customer", tree.Root.Entity.Id);
            Assert.Equal("A0", tree.Root.Alias);
            Assert.Empty(tree.Root.Joins);
            Assert.Null(tree.Root.LookupProperty);
        }


        [Fact]
        public void It_should_Init_AddOneJoin()
        {
            Storm storm = new Storm();
            storm.EditSchema(DynamicModelRealistic.SampleSchema);
            
            TableTree tree = new TableTree(storm.CreateContext(), "Customer");
            tree.Resolve("Address");
            Assert.Equal("Customer", tree.Root.Entity.Id);
            Assert.Equal("A0", tree.Root.Alias);
            Assert.Single(tree.Root.Joins);
            var j1 = tree.Root.Joins.First();
            Assert.Equal("Address", j1.Entity.Id);
            Assert.Equal("A1", j1.Alias);
            Assert.Empty(j1.Joins);
            Assert.Equal("Customer.Address", j1.LookupProperty.Id);
            
        }
    }
}