# Building a routing slip

Developers are discouraged from directly implementing the *RoutingSlip* message type and should instead use a 
*RoutingSlipBuilder* to create a routing slip. The *RoutingSlipBuilder* encapsulates the creation of the routing 
slip and includes methods to add activities, activity logs, and variables to the routing slip. For example, 
to create a routing slip with two activities and an additional variable, a developer would write:

```csharp
var builder = new RoutingSlipBuilder(NewId.NextGuid());
builder.AddActivity("DownloadImage", "rabbitmq://localhost/execute_downloadimage", 
    new
    {
        ImageUri = new Uri("http://images.google.com/someImage.jpg")
    });
builder.AddActivity("FilterImage", "rabbitmq://localhost/execute_filterimage");
builder.AddVariable("WorkPath", "\\dfs\work");

var routingSlip = builder.Build();
```

Each activity requires a name for display purposes and a URI specifying the execution address. The execution 
address is where the routing slip should be sent to execute the activity. For each activity, arguments can be 
specified that are stored and presented to the activity via the activity arguments interface type specify by 
the first argument of the *Activity* interface. The activities added to the routing slip are combined into an 
*Itinerary*, which is the list of activities to be executed, and stored in the routing slip.

> Managing the inventory of available activities, as well as their names and execution addresses, is the 
> responsibility of the application and is not part of the MassTransit Courier. Since activities are application 
> specific, and the business logic to determine which activities to execute and in what order is part of the 
> application domain, the details are left to the application developer.

