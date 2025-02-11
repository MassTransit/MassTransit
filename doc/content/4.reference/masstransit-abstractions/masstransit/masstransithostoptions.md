---

title: MassTransitHostOptions

---

# MassTransitHostOptions

Namespace: MassTransit

If present in the container, these options will be used by the MassTransit hosted service.

```csharp
public class MassTransitHostOptions
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [MassTransitHostOptions](../masstransit/masstransithostoptions)

## Properties

### **WaitUntilStarted**

If True, the hosted service will not return from StartAsync until the bus has started.

```csharp
public bool WaitUntilStarted { get; set; }
```

#### Property Value

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **StartTimeout**

If specified, the timeout will be used with StartAsync to cancel if the timeout is reached

```csharp
public Nullable<TimeSpan> StartTimeout { get; set; }
```

#### Property Value

[Nullable\<TimeSpan\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

### **StopTimeout**

If specified, the timeout will be used with StopAsync to cancel if the timeout is reached.
 The bus is still stopped, only the wait is canceled.

```csharp
public Nullable<TimeSpan> StopTimeout { get; set; }
```

#### Property Value

[Nullable\<TimeSpan\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

### **ConsumerStopTimeout**

If specified, the timeout will be used to wait for Consumers to complete their work
 After this timeout ConsumeContext.CancellationToken will be cancelled [PipeContext.CancellationToken](pipecontext#cancellationtoken)

```csharp
public Nullable<TimeSpan> ConsumerStopTimeout { get; set; }
```

#### Property Value

[Nullable\<TimeSpan\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

## Constructors

### **MassTransitHostOptions()**

```csharp
public MassTransitHostOptions()
```
