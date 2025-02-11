---

title: ICompensateLogConfigurator<TLog>

---

# ICompensateLogConfigurator\<TLog\>

Namespace: MassTransit

Configure the execution of the activity and arguments with some tasty middleware.

```csharp
public interface ICompensateLogConfigurator<TLog> : IPipeConfigurator<CompensateContext<TLog>>, IConsumeConfigurator
```

#### Type Parameters

`TLog`<br/>

Implements [IPipeConfigurator\<CompensateContext\<TLog\>\>](../masstransit/ipipeconfigurator-1), [IConsumeConfigurator](../masstransit/iconsumeconfigurator)
