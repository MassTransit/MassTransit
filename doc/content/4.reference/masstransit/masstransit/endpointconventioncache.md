---

title: EndpointConventionCache

---

# EndpointConventionCache

Namespace: MassTransit

```csharp
public static class EndpointConventionCache
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [EndpointConventionCache](../masstransit/endpointconventioncache)

## Methods

### **TryGetEndpointAddress\<T\>(Uri)**

```csharp
public static bool TryGetEndpointAddress<T>(out Uri address)
```

#### Type Parameters

`T`<br/>

#### Parameters

`address` Uri<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **TryGetEndpointAddress(Type, Uri)**

```csharp
public static bool TryGetEndpointAddress(Type messageType, out Uri address)
```

#### Parameters

`messageType` [Type](https://learn.microsoft.com/en-us/dotnet/api/system.type)<br/>

`address` Uri<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>
