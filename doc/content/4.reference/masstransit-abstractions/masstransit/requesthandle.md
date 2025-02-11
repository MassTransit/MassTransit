---

title: RequestHandle

---

# RequestHandle

Namespace: MassTransit

```csharp
public interface RequestHandle : IRequestPipeConfigurator, IDisposable
```

Implements [IRequestPipeConfigurator](../masstransit/irequestpipeconfigurator), [IDisposable](https://learn.microsoft.com/en-us/dotnet/api/system.idisposable)

## Methods

### **GetResponse\<T\>(Boolean)**

If the specified result type is present, it is returned.

```csharp
Task<Response<T>> GetResponse<T>(bool readyToSend)
```

#### Type Parameters

`T`<br/>
The result type

#### Parameters

`readyToSend` [Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>
If true, sets the request as ready to send and sends it

#### Returns

[Task\<Response\<T\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>
True if the result type specified is present, otherwise false

### **Cancel()**

Cancel the request

```csharp
void Cancel()
```
