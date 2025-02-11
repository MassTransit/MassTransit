---

title: IPipeContextConverterFactory<TInput>

---

# IPipeContextConverterFactory\<TInput\>

Namespace: MassTransit.Middleware

```csharp
public interface IPipeContextConverterFactory<TInput>
```

#### Type Parameters

`TInput`<br/>

## Methods

### **GetConverter\<TOutput\>()**

Given a known input context type, convert it to the correct output
 context type.

```csharp
IPipeContextConverter<TInput, TOutput> GetConverter<TOutput>()
```

#### Type Parameters

`TOutput`<br/>

#### Returns

[IPipeContextConverter\<TInput, TOutput\>](../masstransit-middleware/ipipecontextconverter-2)<br/>
