---

title: IExecuteActivityArgumentsConfigurator<TArguments>

---

# IExecuteActivityArgumentsConfigurator\<TArguments\>

Namespace: MassTransit

Configure the execution of the activity and arguments with some tasty middleware.

```csharp
public interface IExecuteActivityArgumentsConfigurator<TArguments> : IPipeConfigurator<ExecuteActivityContext<TArguments>>, IConsumeConfigurator
```

#### Type Parameters

`TArguments`<br/>

Implements [IPipeConfigurator\<ExecuteActivityContext\<TArguments\>\>](../masstransit/ipipeconfigurator-1), [IConsumeConfigurator](../masstransit/iconsumeconfigurator)
