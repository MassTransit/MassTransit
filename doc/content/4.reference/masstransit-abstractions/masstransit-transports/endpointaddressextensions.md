---

title: EndpointAddressExtensions

---

# EndpointAddressExtensions

Namespace: MassTransit.Transports

```csharp
public static class EndpointAddressExtensions
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [EndpointAddressExtensions](../masstransit-transports/endpointaddressextensions)

## Methods

### **GetEndpointName(Uri)**

Returns the endpoint name (the last part of the URI, without the query string or preceding path)
 from the address

```csharp
public static string GetEndpointName(Uri address)
```

#### Parameters

`address` Uri<br/>

#### Returns

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

### **GetDiagnosticEndpointName(Uri)**

```csharp
public static string GetDiagnosticEndpointName(Uri address)
```

#### Parameters

`address` Uri<br/>

#### Returns

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>
