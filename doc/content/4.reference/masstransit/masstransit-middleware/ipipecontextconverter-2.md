---

title: IPipeContextConverter<TInput, TOutput>

---

# IPipeContextConverter\<TInput, TOutput\>

Namespace: MassTransit.Middleware

Converts the input context to the output context

```csharp
public interface IPipeContextConverter<TInput, TOutput>
```

#### Type Parameters

`TInput`<br/>

`TOutput`<br/>

## Methods

### **TryConvert(TInput, TOutput)**

```csharp
bool TryConvert(TInput input, out TOutput output)
```

#### Parameters

`input` TInput<br/>

`output` TOutput<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>
