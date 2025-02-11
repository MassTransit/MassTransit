---

title: TelemetryMonitorExtensions

---

# TelemetryMonitorExtensions

Namespace: MassTransit.Testing

```csharp
public static class TelemetryMonitorExtensions
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [TelemetryMonitorExtensions](../masstransit-testing/telemetrymonitorextensions)

## Methods

### **Wait(IPublishEndpoint, Func\<IPublishEndpoint, Task\>, Nullable\<TimeSpan\>, Nullable\<TimeSpan\>)**

Wraps the call on the  and waits for the published message to be consumed, along with
 all subsequently produced messages until the specified timeout.

```csharp
public static Task Wait(IPublishEndpoint publishEndpoint, Func<IPublishEndpoint, Task> callback, Nullable<TimeSpan> timeout, Nullable<TimeSpan> idleTimeout)
```

#### Parameters

`publishEndpoint` [IPublishEndpoint](../../masstransit-abstractions/masstransit/ipublishendpoint)<br/>

`callback` [Func\<IPublishEndpoint, Task\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>

`timeout` [Nullable\<TimeSpan\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

`idleTimeout` [Nullable\<TimeSpan\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **Wait(ISendEndpoint, Func\<ISendEndpoint, Task\>, Nullable\<TimeSpan\>, Nullable\<TimeSpan\>)**

Wraps the call on the  and waits for the sent message to be consumed, along with
 all subsequently produced messages until the specified timeout.

```csharp
public static Task Wait(ISendEndpoint sendEndpoint, Func<ISendEndpoint, Task> callback, Nullable<TimeSpan> timeout, Nullable<TimeSpan> idleTimeout)
```

#### Parameters

`sendEndpoint` [ISendEndpoint](../../masstransit-abstractions/masstransit/isendendpoint)<br/>

`callback` [Func\<ISendEndpoint, Task\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>

`timeout` [Nullable\<TimeSpan\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

`idleTimeout` [Nullable\<TimeSpan\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **Wait\<T, T1\>(IRequestClient\<T\>, Func\<IRequestClient\<T\>, Task\<Response\<T1\>\>\>, Nullable\<TimeSpan\>, Nullable\<TimeSpan\>)**

Wraps the call on the  and waits for the request to be completed, along with
 all subsequently produced messages until the specified timeout.

```csharp
public static Task<Response<T1>> Wait<T, T1>(IRequestClient<T> client, Func<IRequestClient<T>, Task<Response<T1>>> callback, Nullable<TimeSpan> timeout, Nullable<TimeSpan> idleTimeout)
```

#### Type Parameters

`T`<br/>

`T1`<br/>

#### Parameters

`client` [IRequestClient\<T\>](../../masstransit-abstractions/masstransit/irequestclient-1)<br/>

`callback` [Func\<IRequestClient\<T\>, Task\<Response\<T1\>\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>

`timeout` [Nullable\<TimeSpan\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

`idleTimeout` [Nullable\<TimeSpan\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

#### Returns

[Task\<Response\<T1\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>

### **Wait\<T, T1, T2\>(IRequestClient\<T\>, Func\<IRequestClient\<T\>, Task\<Response\<T1, T2\>\>\>, Nullable\<TimeSpan\>, Nullable\<TimeSpan\>)**

Wraps the call on the  and waits for the request to be completed, along with
 all subsequently produced messages until the specified timeout.

```csharp
public static Task<Response<T1, T2>> Wait<T, T1, T2>(IRequestClient<T> client, Func<IRequestClient<T>, Task<Response<T1, T2>>> callback, Nullable<TimeSpan> timeout, Nullable<TimeSpan> idleTimeout)
```

#### Type Parameters

`T`<br/>

`T1`<br/>

`T2`<br/>

#### Parameters

`client` [IRequestClient\<T\>](../../masstransit-abstractions/masstransit/irequestclient-1)<br/>

`callback` [Func\<IRequestClient\<T\>, Task\<Response\<T1, T2\>\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>

`timeout` [Nullable\<TimeSpan\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

`idleTimeout` [Nullable\<TimeSpan\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

#### Returns

[Task\<Response\<T1, T2\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>

### **Wait\<T, T1, T2, T3\>(IRequestClient\<T\>, Func\<IRequestClient\<T\>, Task\<Response\<T1, T2, T3\>\>\>, Nullable\<TimeSpan\>, Nullable\<TimeSpan\>)**

Wraps the call on the  and waits for the request to be completed, along with
 all subsequently produced messages until the specified timeout.

```csharp
public static Task<Response<T1, T2, T3>> Wait<T, T1, T2, T3>(IRequestClient<T> client, Func<IRequestClient<T>, Task<Response<T1, T2, T3>>> callback, Nullable<TimeSpan> timeout, Nullable<TimeSpan> idleTimeout)
```

#### Type Parameters

`T`<br/>

`T1`<br/>

`T2`<br/>

`T3`<br/>

#### Parameters

`client` [IRequestClient\<T\>](../../masstransit-abstractions/masstransit/irequestclient-1)<br/>

`callback` [Func\<IRequestClient\<T\>, Task\<Response\<T1, T2, T3\>\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>

`timeout` [Nullable\<TimeSpan\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

`idleTimeout` [Nullable\<TimeSpan\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

#### Returns

[Task\<Response\<T1, T2, T3\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>
