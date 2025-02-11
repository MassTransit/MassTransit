---

title: ConsumeContextConverterFactory

---

# ConsumeContextConverterFactory

Namespace: MassTransit.Middleware

```csharp
public class ConsumeContextConverterFactory : IPipeContextConverterFactory<ConsumeContext>
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [ConsumeContextConverterFactory](../masstransit-middleware/consumecontextconverterfactory)<br/>
Implements [IPipeContextConverterFactory\<ConsumeContext\>](../masstransit-middleware/ipipecontextconverterfactory-1)

## Constructors

### **ConsumeContextConverterFactory()**

```csharp
public ConsumeContextConverterFactory()
```

## Methods

### **GetConverter\<TOutput\>()**

```csharp
public IPipeContextConverter<ConsumeContext, TOutput> GetConverter<TOutput>()
```

#### Type Parameters

`TOutput`<br/>

#### Returns

[IPipeContextConverter\<ConsumeContext, TOutput\>](../masstransit-middleware/ipipecontextconverter-2)<br/>
