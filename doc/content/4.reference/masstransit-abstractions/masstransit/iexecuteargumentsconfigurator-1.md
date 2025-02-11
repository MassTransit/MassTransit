---

title: IExecuteArgumentsConfigurator<TArguments>

---

# IExecuteArgumentsConfigurator\<TArguments\>

Namespace: MassTransit

Configure the execution of the activity and arguments with some tasty middleware.

```csharp
public interface IExecuteArgumentsConfigurator<TArguments> : IPipeConfigurator<ExecuteContext<TArguments>>, IConsumeConfigurator
```

#### Type Parameters

`TArguments`<br/>

Implements [IPipeConfigurator\<ExecuteContext\<TArguments\>\>](../masstransit/ipipeconfigurator-1), [IConsumeConfigurator](../masstransit/iconsumeconfigurator)
