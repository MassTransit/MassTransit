---

title: IPublishPipe

---

# IPublishPipe

Namespace: MassTransit.Transports

```csharp
public interface IPublishPipe : IProbeSite
```

Implements [IProbeSite](../masstransit/iprobesite)

## Methods

### **Send\<T\>(PublishContext\<T\>)**

```csharp
Task Send<T>(PublishContext<T> context)
```

#### Type Parameters

`T`<br/>

#### Parameters

`context` [PublishContext\<T\>](../masstransit/publishcontext-1)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>
