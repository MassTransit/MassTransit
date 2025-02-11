---

title: IActivityFactory

---

# IActivityFactory

Namespace: MassTransit

Should be implemented by containers that support generic object resolution in order to
 provide a common lifetime management policy for all activities

```csharp
public interface IActivityFactory : IProbeSite
```

Implements [IProbeSite](../masstransit/iprobesite)

## Methods

### **Execute\<TActivity, TArguments\>(ExecuteContext\<TArguments\>, IPipe\<ExecuteActivityContext\<TActivity, TArguments\>\>)**

Create and execute the activity

```csharp
Task Execute<TActivity, TArguments>(ExecuteContext<TArguments> context, IPipe<ExecuteActivityContext<TActivity, TArguments>> next)
```

#### Type Parameters

`TActivity`<br/>

`TArguments`<br/>

#### Parameters

`context` [ExecuteContext\<TArguments\>](../masstransit/executecontext-1)<br/>

`next` [IPipe\<ExecuteActivityContext\<TActivity, TArguments\>\>](../masstransit/ipipe-1)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **Compensate\<TActivity, TLog\>(CompensateContext\<TLog\>, IPipe\<CompensateActivityContext\<TActivity, TLog\>\>)**

Create and compensate the activity

```csharp
Task Compensate<TActivity, TLog>(CompensateContext<TLog> compensateContext, IPipe<CompensateActivityContext<TActivity, TLog>> next)
```

#### Type Parameters

`TActivity`<br/>

`TLog`<br/>

#### Parameters

`compensateContext` [CompensateContext\<TLog\>](../masstransit/compensatecontext-1)<br/>

`next` [IPipe\<CompensateActivityContext\<TActivity, TLog\>\>](../masstransit/ipipe-1)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>
