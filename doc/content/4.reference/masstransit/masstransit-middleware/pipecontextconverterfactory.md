---

title: PipeContextConverterFactory

---

# PipeContextConverterFactory

Namespace: MassTransit.Middleware

```csharp
public class PipeContextConverterFactory : IPipeContextConverterFactory<PipeContext>
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [PipeContextConverterFactory](../masstransit-middleware/pipecontextconverterfactory)<br/>
Implements [IPipeContextConverterFactory\<PipeContext\>](../masstransit-middleware/ipipecontextconverterfactory-1)

## Constructors

### **PipeContextConverterFactory()**

```csharp
public PipeContextConverterFactory()
```

## Methods

### **GetConverter\<TOutput\>()**

```csharp
public IPipeContextConverter<PipeContext, TOutput> GetConverter<TOutput>()
```

#### Type Parameters

`TOutput`<br/>

#### Returns

[IPipeContextConverter\<PipeContext, TOutput\>](../masstransit-middleware/ipipecontextconverter-2)<br/>
