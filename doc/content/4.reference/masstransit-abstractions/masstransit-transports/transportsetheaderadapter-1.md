---

title: TransportSetHeaderAdapter<TValueType>

---

# TransportSetHeaderAdapter\<TValueType\>

Namespace: MassTransit.Transports

```csharp
public class TransportSetHeaderAdapter<TValueType> : ITransportSetHeaderAdapter<TValueType>
```

#### Type Parameters

`TValueType`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [TransportSetHeaderAdapter\<TValueType\>](../masstransit-transports/transportsetheaderadapter-1)<br/>
Implements [ITransportSetHeaderAdapter\<TValueType\>](../masstransit-transports/itransportsetheaderadapter-1)

## Constructors

### **TransportSetHeaderAdapter(IHeaderValueConverter\<TValueType\>, TransportHeaderOptions)**

```csharp
public TransportSetHeaderAdapter(IHeaderValueConverter<TValueType> converter, TransportHeaderOptions options)
```

#### Parameters

`converter` [IHeaderValueConverter\<TValueType\>](../masstransit-transports/iheadervalueconverter-1)<br/>

`options` [TransportHeaderOptions](../masstransit-transports/transportheaderoptions)<br/>

## Methods

### **Set(IDictionary\<String, TValueType\>, HeaderValue)**

```csharp
public void Set(IDictionary<string, TValueType> dictionary, in HeaderValue headerValue)
```

#### Parameters

`dictionary` [IDictionary\<String, TValueType\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.idictionary-2)<br/>

`headerValue` [HeaderValue](../masstransit/headervalue)<br/>

### **Set\<T\>(IDictionary\<String, TValueType\>, HeaderValue\<T\>)**

```csharp
public void Set<T>(IDictionary<string, TValueType> dictionary, in HeaderValue<T> headerValue)
```

#### Type Parameters

`T`<br/>

#### Parameters

`dictionary` [IDictionary\<String, TValueType\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.idictionary-2)<br/>

`headerValue` [HeaderValue\<T\>](../masstransit/headervalue-1)<br/>
