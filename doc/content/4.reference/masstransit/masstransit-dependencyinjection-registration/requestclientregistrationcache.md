---

title: RequestClientRegistrationCache

---

# RequestClientRegistrationCache

Namespace: MassTransit.DependencyInjection.Registration

```csharp
public static class RequestClientRegistrationCache
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [RequestClientRegistrationCache](../masstransit-dependencyinjection-registration/requestclientregistrationcache)

## Methods

### **Register(Type, RequestTimeout, IContainerRegistrar)**

```csharp
public static void Register(Type requestType, RequestTimeout timeout, IContainerRegistrar registrar)
```

#### Parameters

`requestType` [Type](https://learn.microsoft.com/en-us/dotnet/api/system.type)<br/>

`timeout` [RequestTimeout](../../masstransit-abstractions/masstransit/requesttimeout)<br/>

`registrar` [IContainerRegistrar](../masstransit-configuration/icontainerregistrar)<br/>

### **Register(Type, Uri, RequestTimeout, IContainerRegistrar)**

```csharp
public static void Register(Type requestType, Uri destinationAddress, RequestTimeout timeout, IContainerRegistrar registrar)
```

#### Parameters

`requestType` [Type](https://learn.microsoft.com/en-us/dotnet/api/system.type)<br/>

`destinationAddress` Uri<br/>

`timeout` [RequestTimeout](../../masstransit-abstractions/masstransit/requesttimeout)<br/>

`registrar` [IContainerRegistrar](../masstransit-configuration/icontainerregistrar)<br/>
