---

title: IReceiveEndpointDependent

---

# IReceiveEndpointDependent

Namespace: MassTransit.Transports

```csharp
public interface IReceiveEndpointDependent
```

## Properties

### **Completed**

The task which is completed once the receive endpoint is completed

```csharp
public abstract Task Completed { get; }
```

#### Property Value

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>
