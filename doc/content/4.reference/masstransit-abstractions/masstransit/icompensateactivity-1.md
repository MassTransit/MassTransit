---

title: ICompensateActivity<TLog>

---

# ICompensateActivity\<TLog\>

Namespace: MassTransit

```csharp
public interface ICompensateActivity<TLog> : ICompensateActivity
```

#### Type Parameters

`TLog`<br/>

Implements [ICompensateActivity](../masstransit/icompensateactivity)

## Methods

### **Compensate(CompensateContext\<TLog\>)**

Compensate the activity and return the remaining compensation items

```csharp
Task<CompensationResult> Compensate(CompensateContext<TLog> context)
```

#### Parameters

`context` [CompensateContext\<TLog\>](../masstransit/compensatecontext-1)<br/>
The compensation information for the activity

#### Returns

[Task\<CompensationResult\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>
