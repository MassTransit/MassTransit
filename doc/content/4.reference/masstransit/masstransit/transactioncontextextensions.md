---

title: TransactionContextExtensions

---

# TransactionContextExtensions

Namespace: MassTransit

```csharp
public static class TransactionContextExtensions
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [TransactionContextExtensions](../masstransit/transactioncontextextensions)

## Methods

### **CreateTransactionScope(PipeContext)**

Create a transaction scope using the transaction context (added by the TransactionFilter),
 to ensure that any transactions are carried between any threads.

```csharp
public static TransactionScope CreateTransactionScope(PipeContext context)
```

#### Parameters

`context` [PipeContext](../../masstransit-abstractions/masstransit/pipecontext)<br/>

#### Returns

TransactionScope<br/>

### **CreateTransactionScope(PipeContext, TimeSpan)**

Create a transaction scope using the transaction context (added by the TransactionFilter),
 to ensure that any transactions are carried between any threads.

```csharp
public static TransactionScope CreateTransactionScope(PipeContext context, TimeSpan scopeTimeout)
```

#### Parameters

`context` [PipeContext](../../masstransit-abstractions/masstransit/pipecontext)<br/>

`scopeTimeout` [TimeSpan](https://learn.microsoft.com/en-us/dotnet/api/system.timespan)<br/>
The timespan after which the scope times out and aborts the transaction

#### Returns

TransactionScope<br/>

### **CreateTransactionScope(PipeContext, TimeSpan, TransactionScopeAsyncFlowOption)**

Create a transaction scope using the transaction context (added by the TransactionFilter),
 to ensure that any transactions are carried between any threads.

```csharp
public static TransactionScope CreateTransactionScope(PipeContext context, TimeSpan scopeTimeout, TransactionScopeAsyncFlowOption asyncFlowOptions)
```

#### Parameters

`context` [PipeContext](../../masstransit-abstractions/masstransit/pipecontext)<br/>

`scopeTimeout` [TimeSpan](https://learn.microsoft.com/en-us/dotnet/api/system.timespan)<br/>
The timespan after which the scope times out and aborts the transaction

`asyncFlowOptions` TransactionScopeAsyncFlowOption<br/>
Specifies whether transaction flow across thread continuations is enabled.

#### Returns

TransactionScope<br/>
