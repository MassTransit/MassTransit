---

title: TransactionContext

---

# TransactionContext

Namespace: MassTransit

```csharp
public interface TransactionContext
```

## Properties

### **Transaction**

Returns the current transaction scope, creating a dependent scope if a thread switch
 occurred

```csharp
public abstract Transaction Transaction { get; }
```

#### Property Value

Transaction<br/>

## Methods

### **Commit()**

Complete the transaction scope

```csharp
Task Commit()
```

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **Rollback()**

Rollback the transaction

```csharp
void Rollback()
```

### **Rollback(Exception)**

Rollback the transaction

```csharp
void Rollback(Exception exception)
```

#### Parameters

`exception` [Exception](https://learn.microsoft.com/en-us/dotnet/api/system.exception)<br/>
The exception that caused the rollback
