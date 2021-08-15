# Activities

In MassTransit Courier, an *Activity* refers to a processing step that can be added to a routing slip.

To create an activity, create a class that implements the *IActivity* interface.

```csharp
public class DownloadImageActivity :
    IActivity<DownloadImageArguments, DownloadImageLog>
{
    Task<ExecutionResult> Execute(ExecuteContext<DownloadImageArguments> context);
    Task<CompensationResult> Compensate(CompensateContext<DownloadImageLog> context);
}
```

The *IActivity* interface is generic with two arguments. The first parameter specifies the activity’s argument type and the second parameter specifies the activity’s log type. In the example shown above, *DownloadImageArguments* is the argument type and *DownloadImageLog* is the log type. Both parameters may be interface, class or record types. Where the type is a class or a record, the proper accessors should be specified (i.e. `{ get; set; }` or `{ get; init; }`).

## Execute Activities

An *Execute Activity* is an activity that only executes and does not support compensation. As such, the declaration of a log type is not required.

```csharp
public class ValidateImageActivity :
    IExecuteActivity<ValidateImageArguments>
{
    Task<ExecutionResult> Execute(ExecuteContext<DownloadImageArguments> context);
}
```

## Implementing an activity

An activity must implement two interface methods, *Execute* and *Compensate*. The *Execute* method is called while the routing slip is executing activities and the *Compensate* method is called when a routing slip faults and needs to be compensated.

When the *Execute* method is called, an *execution* argument is passed containing the activity arguments, the routing slip *TrackingNumber*, and methods to mark the activity as completed or faulted. The actual routing slip message, as well as any details of the underlying infrastructure, are excluded from the *execution* argument to prevent coupling between the activity and the implementation. An example *Execute* method is shown below.

```csharp
async Task<ExecutionResult> Execute(ExecuteContext<DownloadImageArguments> execution)
{
    DownloadImageArguments args = execution.Arguments;
    string imageSavePath = Path.Combine(args.WorkPath, 
        execution.TrackingNumber.ToString());

    await _httpClient.GetAndSave(args.ImageUri, imageSavePath);

    return execution.Completed<DownloadImageLog>(new {ImageSavePath = imageSavePath});
}
```

## Completing an activity

Once activity processing is complete, the activity returns an *ExecutionResult* to the host. If the activity executes successfully, the activity can elect to store compensation data in an activity log which is passed to the *Completed* method on the *execution* argument. If the activity chooses not to store any compensation data, the activity log argument is not required. In addition to compensation data, the activity can add or modify variables stored in the routing slip for use by subsequent activities.

> In the example above, the activity specifies the *DownloadImageLog* interface and initializes the log using an anonymous object. The object is then passed to the *Completed* method for storage in the routing slip before sending the routing slip to the next activity.

## Terminating a routing slip

In some situations, it may make sense to terminate the routing slip without executing any of the subsequent activities in the itinerary. This might be due to a business rule, in which the routing slip shouldn't be faulted, but needs to end immediately.

To terminate a routing slip, call _Terminate_ as shown.

```csharp
// regular termination
return execution.Terminate();

// terminate and include additional variables in the event
return execution.Terminate(new { Reason = "Not a good time, dude."});
```

## Faulting a routing slip

By default, if an activity throws an exception, it will be _faulted_ and a `RoutingSlipFaulted` event will be published (unless a subscription changes the rules). An activity can also return _Faulted_ rather than throwing an exception.

## Compensating an activity

When an activity fails, the *Compensate* method is called for previously executed activities in the routing slip that stored compensation data. If an activity does not store any compensation data, the *Compensate* method is never called. The compensation method for the example above is shown below.

```csharp
Task<CompensationResult> Compensate(CompensateContext<DownloadImageLog> compensation)
{
    DownloadImageLog log = compensation.Log;
    File.Delete(log.ImageSavePath);

    return compensation.Compensated();
}
```

Using the activity log data, the activity compensates by removing the downloaded image from the work directory. Once the activity has compensated the previous execution, it returns a *CompensationResult* by calling the *Compensated* method. If the compensating actions could not be performed (either via logic or an exception) and the inability to compensate results in a failure state, the *Failed* method can be used instead, optionally specifying an *Exception*.


