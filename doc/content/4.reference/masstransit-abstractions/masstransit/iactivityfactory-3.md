---

title: IActivityFactory<TActivity, TArguments, TLog>

---

# IActivityFactory\<TActivity, TArguments, TLog\>

Namespace: MassTransit

```csharp
public interface IActivityFactory<TActivity, TArguments, TLog> : IExecuteActivityFactory<TActivity, TArguments>, IProbeSite, ICompensateActivityFactory<TActivity, TLog>
```

#### Type Parameters

`TActivity`<br/>

`TArguments`<br/>

`TLog`<br/>

Implements [IExecuteActivityFactory\<TActivity, TArguments\>](../masstransit/iexecuteactivityfactory-2), [IProbeSite](../masstransit/iprobesite), [ICompensateActivityFactory\<TActivity, TLog\>](../masstransit/icompensateactivityfactory-2)
