---

title: SystemTransactionContext

---

# SystemTransactionContext

Namespace: MassTransit.Context

```csharp
public class SystemTransactionContext : TransactionContext, IDisposable
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [SystemTransactionContext](../masstransit-context/systemtransactioncontext)<br/>
Implements [TransactionContext](../masstransit/transactioncontext), [IDisposable](https://learn.microsoft.com/en-us/dotnet/api/system.idisposable)

## Properties

### **Transaction**

```csharp
public Transaction Transaction { get; }
```

#### Property Value

Transaction<br/>

## Constructors

### **SystemTransactionContext(TransactionOptions)**

```csharp
public SystemTransactionContext(TransactionOptions options)
```

#### Parameters

`options` TransactionOptions<br/>

## Methods

### **Dispose()**

```csharp
public void Dispose()
```

### **Commit()**

```csharp
public Task Commit()
```

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **Rollback()**

```csharp
public void Rollback()
```

### **Rollback(Exception)**

```csharp
public void Rollback(Exception exception)
```

#### Parameters

`exception` [Exception](https://learn.microsoft.com/en-us/dotnet/api/system.exception)<br/>
