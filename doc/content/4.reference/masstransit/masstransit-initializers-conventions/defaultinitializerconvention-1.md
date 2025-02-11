---

title: DefaultInitializerConvention<TMessage>

---

# DefaultInitializerConvention\<TMessage\>

Namespace: MassTransit.Initializers.Conventions

```csharp
public class DefaultInitializerConvention<TMessage> : InitializerConvention<TMessage>, IInitializerConvention<TMessage>, IMessageInitializerConvention
```

#### Type Parameters

`TMessage`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [InitializerConvention\<TMessage\>](../masstransit-initializers-conventions/initializerconvention-1) → [DefaultInitializerConvention\<TMessage\>](../masstransit-initializers-conventions/defaultinitializerconvention-1)<br/>
Implements [IInitializerConvention\<TMessage\>](../masstransit-initializers-conventions/iinitializerconvention-1), [IMessageInitializerConvention](../masstransit-initializers-conventions/imessageinitializerconvention)

## Constructors

### **DefaultInitializerConvention(IInitializerConvention)**

```csharp
public DefaultInitializerConvention(IInitializerConvention convention)
```

#### Parameters

`convention` [IInitializerConvention](../masstransit-initializers-conventions/iinitializerconvention)<br/>
