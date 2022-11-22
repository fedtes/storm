# Storm
MultiPurpose SQL Orm


# Usage

## Scenario

Sample car selling db, where each customer have 0 to many cars purchased. Each car purchased has a reference with the salesman who sell the car.

- Car has relation with one Customer and one Salesman
- Customer has a relation with **many** cars purchased
- Salesman has a relation with **many** cars selled
- Customer and Salesmen don't have any direct relation, but should passa throught selled/purchased cars.


## Initialize Storm

First create a new instance of storm.

~~~ C#
Storm stormInstance = new Storm();
~~~

Then we need to create some model that describes our entities

~~~ C#
public class Car {
    [StormPrimaryKey]
    public int SerialNumber;
    public string Model;
    public string Color;
    public int CustomerID;
    public int SalesmanID;
}

public class Customer {
    [StormPrimaryKey]
    public int ID
    public string FirstName;
    public string LastName;
}

public class Salesman {
    [StormPrimaryKey]
    public int AgentID
    public string FirstName;
    public string LastName;
}
~~~

Each class will rappresent a table on the db and must has a field (one and only one) marked with **[StormPrimaryKey]** attribute. This is used by storm to identify which of the field is the primary key.

Now we need to register our models and map to tables in the Storm Schema

~~~ C#

stormInstance.EditSchema(x => {
    return x.Add<Car>("Car", "CarTable")
        .Add<Customer>("Customer", "CustomerTable")
        .Add<Salesman>("Salesman", "SalesmanTable");
});
~~~

Where **.Add<TModel>(string uniqueIdentifier,string tableName)** allow to register our entities with:
- TModel: class model that describe our entity
- uniqueIdentifier: string that identify this entity universally inside storm. Class name and uniqueIdentifier can differs.
- tableName: the fisical table where the data are stored on the db.

Then we need to connect our entities. This is done creating some "virtual properties" that extends the base models and link entities togheter.

~~~ C#

stormInstance.EditSchema(x => {
    return x.Add<Car>("Car", "CarTable")
        .Add<Customer>("Customer", "CustomerTable")
        .Add<Salesman>("Salesman", "SalesmanTable")
        .Connect("Owner", "Car", "Customer", "CustomerID", "ID")
        .Connect("Seller", "Car", "Salesman", "SalesmanID", "AgentID")
        .Connect("PurchasedCar", "Customer", "Car", "ID", "CustomerID")
        .Connect("SelledCar", "Salesman", "Car", "AgentID", "SalesmanID")
});
~~~

where **.Connect(string relationIdentifier, string sourceEntity, string targetEntity, string sourceField, string targetField)** connect two entity creating a relation that act as a virtual property of the sourceEntity that return the targetEntity using the specified fields to create the join.

It is possible to define more complex relation using expressions

~~~ C#
    .Connect("Owner", "Car", "Customer", ctx => ctx["source.CustomerID"].EqualTo.Ref("target.ID"))
~~~

## Queries

### Get

Getting some cars from db

~~~ C#
    using (var con = stormInstance.OpenConnection(new SQLConnection("myconnectionstring")))
    {
        var cars = con.Get("Car");
    }
~~~

In this example cars is an IEnumerable containing all the car from the db. Remember that each element is a **dynamic** so use late binding to access fields and properties.

~~~ C#
    string _model = cars.First().Model;
~~~


We can easly get Car and his related customer

~~~ C#
    using (var con = stormInstance.OpenConnection(new SQLConnection("myconnectionstring")))
    {
        var cars = con.Get("Car").With("Owner");
    }
~~~

Each element now is a car (dynamic) but also has a virtual property to access sub element

~~~ C#
    string _model = cars.First().Model;
    string _customerName = cars.First().Owner.First().FirstName;
~~~

Remember that related entities are always IEnumerable.

Going more complex:
~~~ C#
    using (var con = stormInstance.OpenConnection(new SQLConnection("myconnectionstring")))
    {
        var cars = con.Get("Car").With("Owner").With("Seller");
        string _model = cars.First().Model;
        string _customerName = cars.First().Owner.First().FirstName;
        string _agentName = cars.First().Seller.First().FirstName;
    }
~~~

And concatenating many relation:
~~~ C#
    using (var con = stormInstance.OpenConnection(new SQLConnection("myconnectionstring")))
    {
        var cars = con.Get("Car").With("Owner").With("Seller").With("Owner.PurchasedCar");
        string _model = cars.First().Model;
        string _customerName = cars.First().Owner.First().FirstName;
        string _agentName = cars.First().Seller.First().FirstName;
        int carOwned = cars.First().Owner.First().PurchasedCar.Count();
    }
~~~

Using where to filter:
~~~ C#
    using (var con = stormInstance.OpenConnection(new SQLConnection("myconnectionstring")))
    {
        var result = con.Get("Salesman")
            .Where(x => x("FirstName").IsNotNull * x("SelledCar.Color").NotEqualTo.Val("White"));
    }
~~~

Creating boolean expression using:
- **\*** for **And**
- **+** for **Or**
- **(** **)** for operation grouping  

### Projection

Access data in a punctual way creating a projection
~~~ C#
    using (var con = stormInstance.OpenConnection(new SQLConnection("myconnectionstring")))
    {
        var result = con.Projection("Car")
            .Select("Car.*")
            .Select("Owner.FirstName")
            .Select("Owner.LastName")
            .Select("Seller.{LastName, FirstName}");
    }
~~~

That is interpreted as

*Select all field from car, only FirstName and LastName from the Owner (Customer). Idem for the Seller(Salesman) but with more concise syntax*




...continues