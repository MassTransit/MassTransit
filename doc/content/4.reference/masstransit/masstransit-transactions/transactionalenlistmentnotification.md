---

title: TransactionalEnlistmentNotification

---

# TransactionalEnlistmentNotification

Namespace: MassTransit.Transactions

```csharp
public class TransactionalEnlistmentNotification : IEnlistmentNotification
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [TransactionalEnlistmentNotification](../masstransit-transactions/transactionalenlistmentnotification)<br/>
Implements IEnlistmentNotification

## Constructors

### **TransactionalEnlistmentNotification()**

```csharp
public TransactionalEnlistmentNotification()
```

## Methods

### **Prepare(PreparingEnlistment)**

```csharp
public void Prepare(PreparingEnlistment preparingEnlistment)
```

#### Parameters

`preparingEnlistment` PreparingEnlistment<br/>

### **Commit(Enlistment)**

```csharp
public void Commit(Enlistment enlistment)
```

#### Parameters

`enlistment` Enlistment<br/>

### **Rollback(Enlistment)**

```csharp
public void Rollback(Enlistment enlistment)
```

#### Parameters

`enlistment` Enlistment<br/>

### **InDoubt(Enlistment)**

```csharp
public void InDoubt(Enlistment enlistment)
```

#### Parameters

`enlistment` Enlistment<br/>

### **Add(Func\<Task\>)**

```csharp
public void Add(Func<Task> method)
```

#### Parameters

`method` [Func\<Task\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-1)<br/>
