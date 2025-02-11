---

title: IExecuteActivityFactory<TActivity, TArguments>

---

# IExecuteActivityFactory\<TActivity, TArguments\>

Namespace: MassTransit

A factory that creates an execute activity and then invokes the pipe for the activity context

```csharp
public interface IExecuteActivityFactory<TActivity, TArguments> : IProbeSite
```

#### Type Parameters

`TActivity`<br/>

`TArguments`<br/>

Implements [IProbeSite](../masstransit/iprobesite)

## Methods

### **Execute(ExecuteContext\<TArguments\>, IPipe\<ExecuteActivityContext\<TActivity, TArguments\>\>)**

Executes the activity context by passing it to the activity factory, which creates the activity
 and then invokes the next pipe with the combined activity context

```csharp
Task Execute(ExecuteContext<TArguments> context, IPipe<ExecuteActivityContext<TActivity, TArguments>> next)
```

#### Parameters

`context` [ExecuteContext\<TArguments\>](../masstransit/executecontext-1)<br/>

`next` [IPipe\<ExecuteActivityContext\<TActivity, TArguments\>\>](../masstransit/ipipe-1)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>
