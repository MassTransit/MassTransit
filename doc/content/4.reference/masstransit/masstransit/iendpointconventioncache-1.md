---

title: IEndpointConventionCache<TMessage>

---

# IEndpointConventionCache\<TMessage\>

Namespace: MassTransit

```csharp
public interface IEndpointConventionCache<TMessage>
```

#### Type Parameters

`TMessage`<br/>

## Methods

### **TryGetEndpointAddress(Uri)**

Returns the endpoint address for the message

```csharp
bool TryGetEndpointAddress(out Uri address)
```

#### Parameters

`address` Uri<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>
