---

title: DelegateConfigurationExtensions

---

# DelegateConfigurationExtensions

Namespace: MassTransit

```csharp
public static class DelegateConfigurationExtensions
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [DelegateConfigurationExtensions](../masstransit/delegateconfigurationextensions)

## Methods

### **UseExecute\<TContext\>(IPipeConfigurator\<TContext\>, Action\<TContext\>)**

Executes a synchronous method on the pipe

```csharp
public static void UseExecute<TContext>(IPipeConfigurator<TContext> configurator, Action<TContext> callback)
```

#### Type Parameters

`TContext`<br/>
The context type

#### Parameters

`configurator` [IPipeConfigurator\<TContext\>](../masstransit/ipipeconfigurator-1)<br/>
The pipe configurator

`callback` [Action\<TContext\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>
The callback to invoke

### **UseExecuteAsync\<TContext\>(IPipeConfigurator\<TContext\>, Func\<TContext, Task\>)**

Executes an asynchronous method on the pipe

```csharp
public static void UseExecuteAsync<TContext>(IPipeConfigurator<TContext> configurator, Func<TContext, Task> callback)
```

#### Type Parameters

`TContext`<br/>
The context type

#### Parameters

`configurator` [IPipeConfigurator\<TContext\>](../masstransit/ipipeconfigurator-1)<br/>
The pipe configurator

`callback` [Func\<TContext, Task\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>
The callback to invoke
