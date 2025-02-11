---

title: TestActivityListener

---

# TestActivityListener

Namespace: MassTransit.Testing

```csharp
public class TestActivityListener : IAsyncDisposable
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [TestActivityListener](../masstransit-testing/testactivitylistener)<br/>
Implements [IAsyncDisposable](https://learn.microsoft.com/en-us/dotnet/api/system.iasyncdisposable)

## Constructors

### **TestActivityListener(TextWriter, String, String, Boolean)**

```csharp
public TestActivityListener(TextWriter writer, string methodName, string className, bool includeDetails)
```

#### Parameters

`writer` [TextWriter](https://learn.microsoft.com/en-us/dotnet/api/system.io.textwriter)<br/>

`methodName` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

`className` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

`includeDetails` [Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

## Methods

### **DisposeAsync()**

```csharp
public ValueTask DisposeAsync()
```

#### Returns

[ValueTask](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.valuetask)<br/>
