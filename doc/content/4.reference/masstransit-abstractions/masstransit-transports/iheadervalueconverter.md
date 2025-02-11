---

title: IHeaderValueConverter

---

# IHeaderValueConverter

Namespace: MassTransit.Transports

```csharp
public interface IHeaderValueConverter
```

## Methods

### **TryConvert(HeaderValue, HeaderValue)**

```csharp
bool TryConvert(HeaderValue headerValue, out HeaderValue result)
```

#### Parameters

`headerValue` [HeaderValue](../masstransit/headervalue)<br/>

`result` [HeaderValue](../masstransit/headervalue)<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **TryConvert\<T\>(HeaderValue\<T\>, HeaderValue)**

```csharp
bool TryConvert<T>(HeaderValue<T> headerValue, out HeaderValue result)
```

#### Type Parameters

`T`<br/>

#### Parameters

`headerValue` [HeaderValue\<T\>](../masstransit/headervalue-1)<br/>

`result` [HeaderValue](../masstransit/headervalue)<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>
