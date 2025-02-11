---

title: TransformFilter<T>

---

# TransformFilter\<T\>

Namespace: MassTransit.Middleware

Applies a transform to the message

```csharp
public class TransformFilter<T> : IFilter<ConsumeContext<T>>, IProbeSite, IFilter<ExecuteContext<T>>, IFilter<CompensateContext<T>>, IFilter<SendContext<T>>
```

#### Type Parameters

`T`<br/>
The message type

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [TransformFilter\<T\>](../masstransit-middleware/transformfilter-1)<br/>
Implements [IFilter\<ConsumeContext\<T\>\>](../../masstransit-abstractions/masstransit/ifilter-1), [IProbeSite](../../masstransit-abstractions/masstransit/iprobesite), [IFilter\<ExecuteContext\<T\>\>](../../masstransit-abstractions/masstransit/ifilter-1), [IFilter\<CompensateContext\<T\>\>](../../masstransit-abstractions/masstransit/ifilter-1), [IFilter\<SendContext\<T\>\>](../../masstransit-abstractions/masstransit/ifilter-1)

## Constructors

### **TransformFilter(IMessageInitializer\<T\>)**

```csharp
public TransformFilter(IMessageInitializer<T> initializer)
```

#### Parameters

`initializer` [IMessageInitializer\<T\>](../../masstransit-abstractions/masstransit-initializers/imessageinitializer-1)<br/>
