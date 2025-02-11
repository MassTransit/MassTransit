---

title: DictionaryTransportSetHeaderAdapter

---

# DictionaryTransportSetHeaderAdapter

Namespace: MassTransit.Transports

```csharp
public class DictionaryTransportSetHeaderAdapter : ITransportSetHeaderAdapter<Object>
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [DictionaryTransportSetHeaderAdapter](../masstransit-transports/dictionarytransportsetheaderadapter)<br/>
Implements [ITransportSetHeaderAdapter\<Object\>](../masstransit-transports/itransportsetheaderadapter-1)

## Constructors

### **DictionaryTransportSetHeaderAdapter(IHeaderValueConverter, TransportHeaderOptions)**

```csharp
public DictionaryTransportSetHeaderAdapter(IHeaderValueConverter converter, TransportHeaderOptions options)
```

#### Parameters

`converter` [IHeaderValueConverter](../masstransit-transports/iheadervalueconverter)<br/>

`options` [TransportHeaderOptions](../masstransit-transports/transportheaderoptions)<br/>

## Methods

### **Set(IDictionary\<String, Object\>, HeaderValue)**

```csharp
public void Set(IDictionary<string, object> dictionary, in HeaderValue headerValue)
```

#### Parameters

`dictionary` [IDictionary\<String, Object\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.idictionary-2)<br/>

`headerValue` [HeaderValue](../masstransit/headervalue)<br/>

### **Set\<T\>(IDictionary\<String, Object\>, HeaderValue\<T\>)**

```csharp
public void Set<T>(IDictionary<string, object> dictionary, in HeaderValue<T> headerValue)
```

#### Type Parameters

`T`<br/>

#### Parameters

`dictionary` [IDictionary\<String, Object\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.idictionary-2)<br/>

`headerValue` [HeaderValue\<T\>](../masstransit/headervalue-1)<br/>
