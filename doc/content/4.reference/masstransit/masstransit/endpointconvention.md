---

title: EndpointConvention

---

# EndpointConvention

Namespace: MassTransit

```csharp
public static class EndpointConvention
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [EndpointConvention](../masstransit/endpointconvention)

## Methods

### **Map\<T\>(Uri)**

Map the message type to the specified address

```csharp
public static void Map<T>(Uri destinationAddress)
```

#### Type Parameters

`T`<br/>

#### Parameters

`destinationAddress` Uri<br/>

### **Map\<T\>(EndpointAddressProvider\<T\>)**

Map the message type to the endpoint returned by the specified method

```csharp
public static void Map<T>(EndpointAddressProvider<T> endpointAddressProvider)
```

#### Type Parameters

`T`<br/>

#### Parameters

`endpointAddressProvider` [EndpointAddressProvider\<T\>](../masstransit/endpointaddressprovider-1)<br/>

### **TryGetDestinationAddress\<T\>(Uri)**

```csharp
public static bool TryGetDestinationAddress<T>(out Uri destinationAddress)
```

#### Type Parameters

`T`<br/>

#### Parameters

`destinationAddress` Uri<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **TryGetDestinationAddress(Type, Uri)**

```csharp
public static bool TryGetDestinationAddress(Type messageType, out Uri destinationAddress)
```

#### Parameters

`messageType` [Type](https://learn.microsoft.com/en-us/dotnet/api/system.type)<br/>

`destinationAddress` Uri<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>
