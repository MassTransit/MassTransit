---

title: EndpointConventionCache<TMessage>

---

# EndpointConventionCache\<TMessage\>

Namespace: MassTransit

A cache of convention-based CorrelationId mappers, used unless overridden by some mystical force

```csharp
public class EndpointConventionCache<TMessage> : IEndpointConventionCache<TMessage>
```

#### Type Parameters

`TMessage`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [EndpointConventionCache\<TMessage\>](../masstransit/endpointconventioncache-1)<br/>
Implements [IEndpointConventionCache\<TMessage\>](../masstransit/iendpointconventioncache-1)

## Methods

### **Map(EndpointAddressProvider\<TMessage\>)**

```csharp
internal static void Map(EndpointAddressProvider<TMessage> endpointAddressProvider)
```

#### Parameters

`endpointAddressProvider` [EndpointAddressProvider\<TMessage\>](../masstransit/endpointaddressprovider-1)<br/>

### **Map(Uri)**

```csharp
internal static void Map(Uri destinationAddress)
```

#### Parameters

`destinationAddress` Uri<br/>

### **TryGetEndpointAddress(Uri)**

```csharp
internal static bool TryGetEndpointAddress(out Uri address)
```

#### Parameters

`address` Uri<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>
