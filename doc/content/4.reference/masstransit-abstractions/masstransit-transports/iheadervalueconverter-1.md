---

title: IHeaderValueConverter<TValueType>

---

# IHeaderValueConverter\<TValueType\>

Namespace: MassTransit.Transports

```csharp
public interface IHeaderValueConverter<TValueType>
```

#### Type Parameters

`TValueType`<br/>

## Methods

### **TryConvert(HeaderValue, HeaderValue\<TValueType\>)**

```csharp
bool TryConvert(HeaderValue headerValue, out HeaderValue<TValueType> result)
```

#### Parameters

`headerValue` [HeaderValue](../masstransit/headervalue)<br/>

`result` [HeaderValue\<TValueType\>](../masstransit/headervalue-1)<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **TryConvert\<T\>(HeaderValue\<T\>, HeaderValue\<TValueType\>)**

```csharp
bool TryConvert<T>(HeaderValue<T> headerValue, out HeaderValue<TValueType> result)
```

#### Type Parameters

`T`<br/>

#### Parameters

`headerValue` [HeaderValue\<T\>](../masstransit/headervalue-1)<br/>

`result` [HeaderValue\<TValueType\>](../masstransit/headervalue-1)<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>
