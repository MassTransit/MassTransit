---

title: TestConsumeObserver

---

# TestConsumeObserver

Namespace: MassTransit.Testing.Implementations

```csharp
public class TestConsumeObserver : IConsumeObserver
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [TestConsumeObserver](../masstransit-testing-implementations/testconsumeobserver)<br/>
Implements [IConsumeObserver](../../masstransit-abstractions/masstransit/iconsumeobserver)

## Properties

### **Messages**

```csharp
public IReceivedMessageList Messages { get; }
```

#### Property Value

[IReceivedMessageList](../masstransit-testing/ireceivedmessagelist)<br/>

## Constructors

### **TestConsumeObserver(TimeSpan, CancellationToken)**

```csharp
public TestConsumeObserver(TimeSpan timeout, CancellationToken inactivityToken)
```

#### Parameters

`timeout` [TimeSpan](https://learn.microsoft.com/en-us/dotnet/api/system.timespan)<br/>

`inactivityToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>
