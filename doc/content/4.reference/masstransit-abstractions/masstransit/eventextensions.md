---

title: EventExtensions

---

# EventExtensions

Namespace: MassTransit

```csharp
public static class EventExtensions
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [EventExtensions](../masstransit/eventextensions)

## Methods

### **PublishEvent\<T\>(IPipe\<EventContext\>, T)**

```csharp
public static Task PublishEvent<T>(IPipe<EventContext> pipe, T message)
```

#### Type Parameters

`T`<br/>

#### Parameters

`pipe` [IPipe\<EventContext\>](../masstransit/ipipe-1)<br/>

`message` T<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>
