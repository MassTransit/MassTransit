---

title: HostConfigurationRetryExtensions

---

# HostConfigurationRetryExtensions

Namespace: MassTransit.Transports

```csharp
public static class HostConfigurationRetryExtensions
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [HostConfigurationRetryExtensions](../masstransit-transports/hostconfigurationretryextensions)

## Methods

### **Retry(IHostConfiguration, Func\<Task\>, CancellationToken, CancellationToken)**

```csharp
public static Task Retry(IHostConfiguration hostConfiguration, Func<Task> factory, CancellationToken cancellationToken, CancellationToken stoppingToken)
```

#### Parameters

`hostConfiguration` [IHostConfiguration](../masstransit-configuration/ihostconfiguration)<br/>

`factory` [Func\<Task\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-1)<br/>

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

`stoppingToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>
