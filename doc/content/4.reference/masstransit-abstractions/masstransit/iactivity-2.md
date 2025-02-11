---

title: IActivity<TArguments, TLog>

---

# IActivity\<TArguments, TLog\>

Namespace: MassTransit

An Activity implements the execute and compensate methods for an activity

```csharp
public interface IActivity<TArguments, TLog> : IExecuteActivity<TArguments>, IExecuteActivity, ICompensateActivity<TLog>, ICompensateActivity, IActivity
```

#### Type Parameters

`TArguments`<br/>
The activity argument type

`TLog`<br/>
The activity log argument type

Implements [IExecuteActivity\<TArguments\>](../masstransit/iexecuteactivity-1), [IExecuteActivity](../masstransit/iexecuteactivity), [ICompensateActivity\<TLog\>](../masstransit/icompensateactivity-1), [ICompensateActivity](../masstransit/icompensateactivity), [IActivity](../masstransit/iactivity)
