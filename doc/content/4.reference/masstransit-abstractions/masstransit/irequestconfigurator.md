---

title: IRequestConfigurator

---

# IRequestConfigurator

Namespace: MassTransit

```csharp
public interface IRequestConfigurator
```

## Properties

### **ServiceAddress**

Sets the service address of the request handler

```csharp
public abstract Uri ServiceAddress { set; }
```

#### Property Value

Uri<br/>

### **Timeout**

Sets the request timeout

```csharp
public abstract TimeSpan Timeout { set; }
```

#### Property Value

[TimeSpan](https://learn.microsoft.com/en-us/dotnet/api/system.timespan)<br/>

### **TimeToLive**

Set the time to live of the request message sent by the saga. If not specified, and the timeout
 is &gt; TimeSpan.Zero, the [IRequestConfigurator.Timeout](irequestconfigurator#timeout) value is used.

```csharp
public abstract Nullable<TimeSpan> TimeToLive { set; }
```

#### Property Value

[Nullable\<TimeSpan\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

### **ClearRequestIdOnFaulted**

By default, the RequestId is not cleared when the request is Faulted. Set to true to clear the requestId

```csharp
public abstract bool ClearRequestIdOnFaulted { set; }
```

#### Property Value

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>
