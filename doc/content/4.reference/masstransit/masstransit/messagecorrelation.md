---

title: MessageCorrelation

---

# MessageCorrelation

Namespace: MassTransit

```csharp
public static class MessageCorrelation
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [MessageCorrelation](../masstransit/messagecorrelation)

## Methods

### **UseCorrelationId\<T\>(Func\<T, Guid\>)**

```csharp
public static void UseCorrelationId<T>(Func<T, Guid> getCorrelationId)
```

#### Type Parameters

`T`<br/>

#### Parameters

`getCorrelationId` [Func\<T, Guid\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>
