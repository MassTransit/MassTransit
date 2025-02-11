---

title: IRequestPipeConfigurator

---

# IRequestPipeConfigurator

Namespace: MassTransit

```csharp
public interface IRequestPipeConfigurator
```

## Properties

### **RequestId**

The RequestId assigned to the request, and used in the header for the outgoing request message

```csharp
public abstract Guid RequestId { get; }
```

#### Property Value

[Guid](https://learn.microsoft.com/en-us/dotnet/api/system.guid)<br/>

### **TimeToLive**

Set the request message time to live, which by default is equal to the request timeout. Clearing this value
 will prevent any TimeToLive value from being specified.

```csharp
public abstract RequestTimeout TimeToLive { set; }
```

#### Property Value

[RequestTimeout](../masstransit/requesttimeout)<br/>
