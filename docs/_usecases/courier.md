---
layout: default
title: orchestrate multiple services? (courier)
subtitle: Composable middleware for the Task Parallel Library
---

How to connect many services into one flow.

## Using Courier

Developing applications using a distributed, message-based architecture significantly increases the complexity of performing operations transactionally, where an end-to-end set of steps much be completed entirely, or not at all. In an application using an ACID database, this is typically done using SQL transactions, where partial operations are rolled back if the transaction cannot be completed. However, this doesn't scale when the steps being to include dependencies outside of a single database. And in the distributed, *micro-services* based architectures, the use of a single ACID database is shrinking to completely non-existent.

MassTransit Courier is a mechanism for creating and executing distributed transactions with fault compensation that can be used to meet the requirements previously within the domain of database transactions, but built to scale across a large system of distributed services. Courier also works well with MassTransit sagas, which add transaction monitoring and recoverability.

* ToC
{:toc}

## Using a Routing Slip

A routing slip specifies a sequence of processing steps called *activities* that are combined into a single transaction. As each activity completes, the routing slip is forwarded to the next activity in the itinerary. When all activities have completed, the routing slip is completed and the transaction is complete.

A key advantage to using a routing slip is it allows the activities to vary for each transaction. Depending upon the requirements for each transaction, which may differ based on things like payment methods, billing or shipping address, or customer preference ratings, the routing slip builder can selectively add activities to the routing slip. This dynamic behavior is in contrast to a more explicit behavior defined by a state machine or sequential workflow that is statically defined (either through the use of code, a DSL, or something like Windows Workflow).


## MassTransit Courier

MassTransit Courier is a framework that implements the routing slip pattern. Leveraging a durable messaging transport and the advanced saga features of MassTransit, Courier provides a powerful set of components to simplify the use of routing slips in distributed applications. Combining the routing slip pattern with a [state machine such as Automatonymous](https://github.com/phatboyg/Automatonymous) results in a reliable, recoverable, and supportable approach for coordinating and monitoring message processing across multiple services.

In addition to the basic routing slip pattern, MassTransit Courier also supports [compensations](http://en.wikipedia.org/wiki/Compensation_%28engineering%29) which allow activities to store execution data so that reversible operations can be undone, using either a traditional rollback mechanism or by applying an offsetting operation. For example, an activity that holds a seat for a patron could release the held seat when compensated.

## Activities


In MassTransit Courier, an *Activity* refers to a processing step that can be added to a routing slip. To create an activity, create a class that implements the *Activity* interface.

{% highlight C# %}
public class DownloadImageActivity :
    Activity<DownloadImageArguments, DownloadImageLog>
{
    Task<ExecuteResult> Execute(ExecutionContext<DownloadImageArguments> context);
    Task<CompensationResult> Compensate(CompensateContext<DownloadImageLog> context);
}
{% endhighlight %}

The *Activity* interface is generic with two arguments. The first argument specifies the activity’s input type and the second argument specifies the activity’s log type. In the example shown above, *DownloadImageArguments* is the input type and *DownloadImageLog* is the log type. Both arguments must be interface types so that the implementations can be dynamically created.

### Execute Activities


An *Execute Activity* is an activity that only executes and does not support compensation. As such, the definition of a log type is not required.

{% highlight C# %}
public class ValidateImageActivity :
    ExecuteActivity<ValidateImageArguments>
{
    Task<ExecuteResult> Execute(ExecutionContext<DownloadImageArguments> context);
}
{% endhighlight %}

### Implementing an activity

An activity must implement two interface methods, *Execute* and *Compensate*. The *Execute* method is called while the routing slip is executing activities and the *Compensate* method is called when a routing slip faults and needs to be compensated.

When the *Execute* method is called, an *execution* argument is passed containing the activity arguments, the routing slip *TrackingNumber*, and methods to mark the activity as completed or faulted. The actual routing slip message, as well as any details of the underlying infrastructure, are excluded from the *execution* argument to prevent coupling between the activity and the implementation. An example *Execute* method is shown below.

{% highlight C# %}
  async Task<ExecutionResult> Execute(Execution<DownloadImageArguments> execution)
  {
      DownloadImageArguments args = execution.Arguments;
      string imageSavePath = Path.Combine(args.WorkPath,
          execution.TrackingNumber.ToString());

      await _httpClient.GetAndSave(args.ImageUri, imageSavePath);

      return await execution.Completed(new DownloadImageLogImpl(imageSavePath));
  }
{% endhighlight %}

Once activity processing is complete, the activity returns an *ExecutionResult* to the host. If the activity executes successfully, the activity can elect to store compensation data in an activity log which is passed to the *Completed* method on the *execution* argument. If the activity chooses not to store any compensation data, the activity log argument is not required. In addition to compensation data, the activity can add or modify variables stored in the routing slip for use by subsequent activities.


{% note %}
In the example above, the activity creates an instance of a private class that implements the *DownloadImageLog* interface and stores the log information in the object properties. The object is then passed to the *Completed* method for storage in the routing slip before sending the routing slip to the next activity.
{% endnote %}

### Compensating an activity

When an activity fails, the *Compensate* method is called for previously executed activities in the routing slip that stored compensation data. If an activity does not store any compensation data, the *Compensate* method is never called. The compensation method for the example above is shown below.

{% highlight C# %}
Task<CompensationResult> Compensate(Compensation<DownloadImageLog> compensation)
{
    DownloadImageLog log = compensation.Log;
    File.Delete(log.ImageSavePath);

    return compensation.Compensated();
}
{% endhighlight %}

Using the activity log data, the activity compensates by removing the downloaded image from the work directory. Once the activity has compensated the previous execution, it returns a *CompensationResult* by calling the *Compensated* method. If the compensating actions could not be performed (either via logic or an exception) and the inability to compensate results in a failure state, the *Failed* method can be used instead, optionally specifying an *Exception*.

## Building a routing slip

Developers are discouraged from directly implementing the *RoutingSlip* message type and should instead use a *RoutingSlipBuilder* to create a routing slip. The *RoutingSlipBuilder* encapsulates the creation of the routing slip and includes methods to add activities, activity logs, and variables to the routing slip. For example, to create a routing slip with two activities and an additional variable, a developer would write:

{% highlight C# %}
var builder = new RoutingSlipBuilder(NewId.NextGuid());
builder.AddActivity("DownloadImage", "rabbitmq://localhost/execute_downloadimage", new
    {
        ImageUri = new Uri("http://images.google.com/someImage.jpg")
    });
builder.AddActivity("FilterImage", "rabbitmq://localhost/execute_filterimage");
builder.AddVariable("WorkPath", "\\dfs\work");

var routingSlip = builder.Build();
{% endhighlight %}

Each activity requires a name for display purposes and a URI specifying the execution address. The execution address is where the routing slip should be sent to execute the activity. For each activity, arguments can be specified that are stored and presented to the activity via the activity arguments interface type specify by the first argument of the *Activity* interface. The activities added to the routing slip are combined into an *Itinerary*, which is the list of activities to be executed, and stored in the routing slip.

{% note %}
Managing the inventory of available activities, as well as their names and execution addresses, is the responsibility of the application and is not part of the MassTransit Courier. Since activities are application specific, and the business logic to determine which activities to execute and in what order is part of the application domain, the details are left to the application developer.
{% endnote %}

## Executing the routing slip

Once built, the routing slip is executed, which sends it to the first activity’s execute URI. To make it easy and to ensure that source information is included, an extension method on *IBus* is available, the usage of which is shown below.

{% highlight C# %}
await bus.Execute(routingSlip);
{% endhighlight %}

It should be pointed out that if the address for the first activity is invalid or cannot be reached, an exception will be thrown by the *Execute* method.

## Monitoring routing slip execution

During routing slip execution, events are published when the routing slip completes or faults. Every event message includes the *TrackingNumber* as well as a *Timestamp* (in UTC, of course) indicating when the event occurred:

  * RoutingSlipCompleted
  * RoutingSlipFaulted
  * RoutingSlipCompensationFailed

Additional events are published for each activity, including:

  * RoutingSlipActivityCompleted
  * RoutingSlipActivityFaulted
  * RoutingSlipActivityCompensated
  * RoutingSlipActivityCompensationFailed

By observing these events, an application can monitor and track the state of a routing slip. To maintain the current state, an Automatonymous state machine could be created. To maintain history, events could be stored in a database and then queried using the *TrackingNumber* of the routing slip.

## Subscriptions

By default, routing slip events are published -- which means that any subscribed consumers will receive the events. While this is useful getting started, it can quickly get out of control as applications grow and multiple unrelated routing slips are used. To handle this, subscriptions were added (yes, added, because they weren't though of until we experienced this ourselves).

Subscriptions are added to the routing slip at the time it is built using the `RoutingSlipBuilder`.

{% highlight C# %}
builder.AddSubscription(new Uri("rabbitmq://localhost/log-events"), RoutingSlipEvents.All);
{% endhighlight %}

This subscription would send all routing slip events to the specified endpoint. If the application only wanted specified events, the events can be selected by specifying the enumeration values for those events. For example, to only get the `RoutingSlipCompleted` and `RoutingSlipFaulted` events, the following code would be used.

{% highlight C# %}
builder.AddSubscription(new Uri("rabbitmq://localhost/log-events"), RoutingSlipEvents.Completed | RoutingSlipEvents.Faulted);
{% endhighlight %}

It is also possible to tweak the content of the events to cut down on message size. For instance, by default, the `RoutingSlipCompleted` event includes the variables from the routing slip. If the variables contained a large document, that document would be copied to the event. Eliminating the variables from the event would reduce the message size, thereby reducing the traffic on the message broker. To specify the contents of a routing slip event subscription, an additional argument is specified.

{% highlight C# %}
builder.AddSubscription(new Uri("rabbitmq://localhost/log-events"), RoutingSlipEvents.Completed, RoutingSlipEventContents.None);
{% endhighlight %}

This would send the `RoutingSlipCompleted` event to the endpoint, without any of the variables be included (only the main properties of the event would be present).

{% note %}
Once a subscription is added to a routing slip, events are no longer published -- they are only sent to the addresses specified in the subscriptions. However, multiple subscriptions can be specified -- the endpoints just need to be known at the time the routing slip is built.
{% endnote %}

### Custom events

It is also possible to specify a subscription with a custom event, a message that is created by the application developer. This makes it possible to create your own event types and publish them in response to routing slip events occurring. And this includes having the full context of a regular endpoint `Send` so that any headers or context settings can be applied.

To create a custom event subscription, use the overload shown below.

{% highlight C# %}
// first, define the event type in your assembly
public interface OrderProcessingCompleted
{
    Guid TrackingNumber { get; }
    DateTime Timestamp { get; }

    string OrderId { get; }
    string OrderApproval { get; }
}

// then, add the subscription with the custom properties
builder.AddSubscription(new Uri("rabbitmq://localhost/order-events"), RoutingSlipEvents.Completed,
    x => x.Send<OrderProcessingCompleted>(new
    {
        OrderId = "BFG-9000",
        OrderApproval = "ComeGetSome"
    }));
{% endhighlight %}

In the message contract above, there are four properties, but only two of them are specified. By default, the base `RoutingSlipCompleted` event is created, and then the content of that event is *merged* into the message created in the subscription. This ensures that the dynamic values, such as the `TrackingNumber` and the `Timestamp`, which are present in the default event, are available in the custom event.

Custom events can also select with contents are merged with the custom event, using an additional method overload.
