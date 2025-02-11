---

title: SqlHostSettings

---

# SqlHostSettings

Namespace: MassTransit

Settings to configure a DbTransport host explicitly without requiring the fluent interface

```csharp
public interface SqlHostSettings : ISpecification
```

Implements [ISpecification](../../masstransit-abstractions/masstransit/ispecification)

## Properties

### **HostAddress**

```csharp
public abstract Uri HostAddress { get; }
```

#### Property Value

Uri<br/>

### **ConnectionTag**

```csharp
public abstract string ConnectionTag { get; }
```

#### Property Value

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

### **VirtualHost**

```csharp
public abstract string VirtualHost { get; }
```

#### Property Value

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

### **Area**

```csharp
public abstract string Area { get; }
```

#### Property Value

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

### **IsolationLevel**

```csharp
public abstract IsolationLevel IsolationLevel { get; }
```

#### Property Value

IsolationLevel<br/>

### **ConnectionLimit**

```csharp
public abstract int ConnectionLimit { get; }
```

#### Property Value

[Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>

### **MaintenanceEnabled**

```csharp
public abstract bool MaintenanceEnabled { get; }
```

#### Property Value

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **MaintenanceInterval**

```csharp
public abstract TimeSpan MaintenanceInterval { get; }
```

#### Property Value

[TimeSpan](https://learn.microsoft.com/en-us/dotnet/api/system.timespan)<br/>

### **QueueCleanupInterval**

```csharp
public abstract TimeSpan QueueCleanupInterval { get; }
```

#### Property Value

[TimeSpan](https://learn.microsoft.com/en-us/dotnet/api/system.timespan)<br/>

### **MaintenanceBatchSize**

```csharp
public abstract int MaintenanceBatchSize { get; }
```

#### Property Value

[Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>

## Methods

### **CreateConnectionContextFactory(ISqlHostConfiguration)**

```csharp
ConnectionContextFactory CreateConnectionContextFactory(ISqlHostConfiguration configuration)
```

#### Parameters

`configuration` [ISqlHostConfiguration](../masstransit-sqltransport-configuration/isqlhostconfiguration)<br/>

#### Returns

[ConnectionContextFactory](../masstransit-sqltransport/connectioncontextfactory)<br/>

### **GetLicenseInfo()**

```csharp
LicenseInfo GetLicenseInfo()
```

#### Returns

[LicenseInfo](../../masstransit-abstractions/masstransit-licensing/licenseinfo)<br/>
