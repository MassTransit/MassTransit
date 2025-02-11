---

title: ITransportSetHeaderAdapter<TValueType>

---

# ITransportSetHeaderAdapter\<TValueType\>

Namespace: MassTransit.Transports

```csharp
public interface ITransportSetHeaderAdapter<TValueType>
```

#### Type Parameters

`TValueType`<br/>

## Methods

### **Set(IDictionary\<String, TValueType\>, HeaderValue)**

```csharp
void Set(IDictionary<string, TValueType> dictionary, in HeaderValue headerValue)
```

#### Parameters

`dictionary` [IDictionary\<String, TValueType\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.idictionary-2)<br/>

`headerValue` [HeaderValue](../masstransit/headervalue)<br/>

### **Set\<T\>(IDictionary\<String, TValueType\>, HeaderValue\<T\>)**

```csharp
void Set<T>(IDictionary<string, TValueType> dictionary, in HeaderValue<T> headerValue)
```

#### Type Parameters

`T`<br/>

#### Parameters

`dictionary` [IDictionary\<String, TValueType\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.idictionary-2)<br/>

`headerValue` [HeaderValue\<T\>](../masstransit/headervalue-1)<br/>
