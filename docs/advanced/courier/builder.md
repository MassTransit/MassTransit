# Building a routing slip

A routing slip contains an itinerary, variables, and activity/compensation logs. It is defined by a message contract, which is used by the underlying Courier components to execute and compensate the transaction. The routing slip contract includes:

- A tracking number, which should be unique for each routing slip
- An itinerary, which is an ordered list of activities
- An activity log, containing an ordered list of previously executed activites
- A compensation log, containing an order list of previous executed activities which may be compensated if the routing slip faults
- A collection of variables, which can be mapped to activity arguments
- A collection of subscriptions, which can be added to notify consumers of routing slip events
- A collection of exceptions which may have occurred during routing slip execution

Developers are discouraged from directly implementing the *RoutingSlip* message type and should instead use a *RoutingSlipBuilder* to create a routing slip. The *RoutingSlipBuilder* encapsulates the creation of the routing slip and includes methods to add activities (and their arguments), activity logs, and variables to the routing slip. For example, to create a routing slip with two activities and an additional variable, a developer would write:

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

Each activity requires a name for display purposes and a URI specifying the execution address. The execution address is where the routing slip should be sent to execute the activity. For each activity, arguments can be specified that are stored and presented to the activity via the activity arguments interface type specify by the first argument of the *Activity* interface. The activities added to the routing slip are combined into an *Itinerary*, which is the list of activities to be executed, and stored in the routing slip.

> Managing the inventory of available activities, as well as their names and execution addresses, is the responsibility of the application and is not part of the MassTransit Courier. Since activities are application specific, and the business logic to determine which activities to execute and in what order is part of the application domain, the details are left to the application developer.

## Activity Arguments

Each activity declares an activity argument type, which must be an interface. When the routing slip is received by an activity host, the argument type is used to read data from the routing slip and deliver it to the activity.

The argument properties are mapped, by name, to the argument type from the routing slip using:

- Explictly declared arguments, added to the itinerary with the activity
- Implictly mapped arguments, added as variables to the routing slip

To specify an explicit activity argument, specify the argument value while adding the activity using the routing slip builder.

```csharp
var builder = new RoutingSlipBuilder(NewId.NextGuid());
builder.AddActivity("DownloadImage", "rabbitmq://localhost/execute_downloadimage", new
    {
        ImageUri = new Uri("http://images.google.com/someImage.jpg")
    });
```

To specify an implicit activity argument, add a variable to the routing slip with the same name/type as the activity argument.

```csharp
var builder = new RoutingSlipBuilder(NewId.NextGuid());
builder.AddActivity("DownloadImage", "rabbitmq://localhost/execute_downloadimage");
builder.AddVariable("ImageUri", "http://images.google.com/someImage.jpg");
```

If an activity argument is not specified when the routing slip is created, it may be added by an activity that executes prior to the activity that requires the argument. For instance, if the _DownloadImage_ activity stored the image in a local cache, that address could be added and used by another activity to access the cached image.

First, the routing slip would be built without the argument value.

```csharp
var builder = new RoutingSlipBuilder(NewId.NextGuid());
builder.AddActivity("DownloadImage", "rabbitmq://localhost/execute_downloadimage");
builder.AddActivity("ProcessImage", "rabbitmq://localhost/execute_processimage");
builder.AddVariable("ImageUri", "http://images.google.com/someImage.jpg");
```

Then, the first activity would add the variable to the routing slip on completion.

```csharp
Task Execute(ExecuteContext<DownloadImageArguments> context)
{
    ...
    return context.CompletedWithVariables(new { ImagePath = ...});
}
```

The process image activity would then use that variable as an argument value.

```csharp
Task Execute(ExecuteContext<ProcessImageArguments> context)
{
    var path = context.Arguments.ImagePath;
}
```


