---

title: IMessageDataConverter<T>

---

# IMessageDataConverter\<T\>

Namespace: MassTransit.Metadata

```csharp
public interface IMessageDataConverter<T>
```

#### Type Parameters

`T`<br/>

## Methods

### **Convert(Stream, CancellationToken)**

```csharp
Task<T> Convert(Stream stream, CancellationToken cancellationToken)
```

#### Parameters

`stream` [Stream](https://learn.microsoft.com/en-us/dotnet/api/system.io.stream)<br/>

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[Task\<T\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>
