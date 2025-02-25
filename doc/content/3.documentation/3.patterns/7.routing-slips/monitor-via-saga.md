---
title: Monitor via Saga
---

# How to Monitor a Routing Slip with a Saga

Because Routing Slips carry their state on the wire, it's not possible to query the state of a currently executing routing slip. Use this pattern when your solution requires tracking the progress of a Routing Slip that's in-flight.

## Create a Normal Routing Slip

```csharp
var builder = new RoutingSlipBuilder(NewId.NextGuid());

// add your activities as normal
builder.AddActivity("DownloadImage", new Uri("rabbitmq://localhost/execute_downloadimage"), 
    new
    {
        ImageUri = new Uri("http://images.google.com/someImage.jpg")
    });
builder.AddActivity("FilterImage", new Uri("rabbitmq://localhost/execute_filterimage"));
builder.AddVariable("WorkPath", @"\dfs\work");

var routingSlip = builder.Build();
```

## Build the Saga

```csharp
public class MonitorRoutingSlip :
  MassTransitStateMachine<MonitorState>
{
  public MonitorRoutingSlip()
  {
    InstanceState(x => x.CurrentState);
  }
}
```

## Add Subscription to Routing Slip

```csharp
var builder = new RoutingSlipBuilder(NewId.NextGuid());

// ... add activities and variables as normal

// ⭐️ KEY ITEM
builder.AddSubscription(new Uri("<the saga queue>"), RoutingSlipEvents.All);

var routingSlip = builder.Build();
```
