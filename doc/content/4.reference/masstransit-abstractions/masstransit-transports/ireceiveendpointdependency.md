---

title: IReceiveEndpointDependency

---

# IReceiveEndpointDependency

Namespace: MassTransit.Transports

```csharp
public interface IReceiveEndpointDependency
```

## Properties

### **Ready**

The task which is completed once the receive endpoint is ready

```csharp
public abstract Task Ready { get; }
```

#### Property Value

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>
