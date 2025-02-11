---

title: TransactionConfiguratorExtensions

---

# TransactionConfiguratorExtensions

Namespace: MassTransit

```csharp
public static class TransactionConfiguratorExtensions
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [TransactionConfiguratorExtensions](../masstransit/transactionconfiguratorextensions)

## Methods

### **UseTransaction\<T\>(IPipeConfigurator\<T\>, Action\<ITransactionConfigurator\>)**

Encapsulate the pipe behavior in a transaction

```csharp
public static void UseTransaction<T>(IPipeConfigurator<T> configurator, Action<ITransactionConfigurator> configure)
```

#### Type Parameters

`T`<br/>
The pipe context type

#### Parameters

`configurator` [IPipeConfigurator\<T\>](../../masstransit-abstractions/masstransit/ipipeconfigurator-1)<br/>
The pipe configurator

`configure` [Action\<ITransactionConfigurator\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>
Configure the transaction pipe
