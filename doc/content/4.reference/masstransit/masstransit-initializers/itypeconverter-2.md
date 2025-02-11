---

title: ITypeConverter<TResult, TInput>

---

# ITypeConverter\<TResult, TInput\>

Namespace: MassTransit.Initializers

A synchronous property type conversion, which may or may not succeed.

```csharp
public interface ITypeConverter<TResult, TInput>
```

#### Type Parameters

`TResult`<br/>

`TInput`<br/>

## Methods

### **TryConvert(TInput, TResult)**

Convert the input to the result type

```csharp
bool TryConvert(TInput input, out TResult result)
```

#### Parameters

`input` TInput<br/>
The input value

`result` TResult<br/>
The result value

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>
True if the value was converted, otherwise false
