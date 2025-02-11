---

title: IBaseTestHarness

---

# IBaseTestHarness

Namespace: MassTransit.Testing

```csharp
public interface IBaseTestHarness
```

## Properties

### **TestTimeout**

```csharp
public abstract TimeSpan TestTimeout { get; set; }
```

#### Property Value

[TimeSpan](https://learn.microsoft.com/en-us/dotnet/api/system.timespan)<br/>

### **TestInactivityTimeout**

```csharp
public abstract TimeSpan TestInactivityTimeout { get; set; }
```

#### Property Value

[TimeSpan](https://learn.microsoft.com/en-us/dotnet/api/system.timespan)<br/>

### **CancellationToken**

CancellationToken that is canceled when the test is being aborted

```csharp
public abstract CancellationToken CancellationToken { get; }
```

#### Property Value

[CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

### **InactivityToken**

CancellationToken that is cancelled when the test inactivity timeout has elapsed with no bus activity

```csharp
public abstract CancellationToken InactivityToken { get; }
```

#### Property Value

[CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

### **InactivityTask**

Task that is completed when the bus inactivity timeout has elapsed with no bus activity

```csharp
public abstract Task InactivityTask { get; }
```

#### Property Value

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **Consumed**

```csharp
public abstract IReceivedMessageList Consumed { get; }
```

#### Property Value

[IReceivedMessageList](../masstransit-testing/ireceivedmessagelist)<br/>

### **Published**

```csharp
public abstract IPublishedMessageList Published { get; }
```

#### Property Value

[IPublishedMessageList](../masstransit-testing/ipublishedmessagelist)<br/>

### **Sent**

```csharp
public abstract ISentMessageList Sent { get; }
```

#### Property Value

[ISentMessageList](../masstransit-testing/isentmessagelist)<br/>

## Methods

### **Cancel()**

Sets the [IBaseTestHarness.CancellationToken](ibasetestharness#cancellationtoken), canceling the test execution

```csharp
void Cancel()
```

### **ForceInactive()**

Force the inactivity task to complete

```csharp
void ForceInactive()
```
