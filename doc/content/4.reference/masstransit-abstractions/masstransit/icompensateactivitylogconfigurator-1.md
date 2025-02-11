---

title: ICompensateActivityLogConfigurator<TLog>

---

# ICompensateActivityLogConfigurator\<TLog\>

Namespace: MassTransit

Configure the execution of the activity and arguments with some tasty middleware.

```csharp
public interface ICompensateActivityLogConfigurator<TLog> : IPipeConfigurator<CompensateActivityContext<TLog>>, IConsumeConfigurator
```

#### Type Parameters

`TLog`<br/>

Implements [IPipeConfigurator\<CompensateActivityContext\<TLog\>\>](../masstransit/ipipeconfigurator-1), [IConsumeConfigurator](../masstransit/iconsumeconfigurator)
