---

title: SimpleHeaderValueConverter

---

# SimpleHeaderValueConverter

Namespace: MassTransit.Transports

```csharp
public class SimpleHeaderValueConverter : IHeaderValueConverter
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [SimpleHeaderValueConverter](../masstransit-transports/simpleheadervalueconverter)<br/>
Implements [IHeaderValueConverter](../masstransit-transports/iheadervalueconverter)

## Constructors

### **SimpleHeaderValueConverter()**

```csharp
public SimpleHeaderValueConverter()
```

## Methods

### **TryConvert(HeaderValue, HeaderValue)**

```csharp
public bool TryConvert(HeaderValue headerValue, out HeaderValue result)
```

#### Parameters

`headerValue` [HeaderValue](../masstransit/headervalue)<br/>

`result` [HeaderValue](../masstransit/headervalue)<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **TryConvert\<T\>(HeaderValue\<T\>, HeaderValue)**

```csharp
public bool TryConvert<T>(HeaderValue<T> headerValue, out HeaderValue result)
```

#### Type Parameters

`T`<br/>

#### Parameters

`headerValue` [HeaderValue\<T\>](../masstransit/headervalue-1)<br/>

`result` [HeaderValue](../masstransit/headervalue)<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>
