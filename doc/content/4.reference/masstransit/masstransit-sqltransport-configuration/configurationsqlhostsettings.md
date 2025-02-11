---

title: ConfigurationSqlHostSettings

---

# ConfigurationSqlHostSettings

Namespace: MassTransit.SqlTransport.Configuration

```csharp
public abstract class ConfigurationSqlHostSettings : SqlHostSettings, ISpecification
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [ConfigurationSqlHostSettings](../masstransit-sqltransport-configuration/configurationsqlhostsettings)<br/>
Implements [SqlHostSettings](../masstransit/sqlhostsettings), [ISpecification](../../masstransit-abstractions/masstransit/ispecification)

## Properties

### **Host**

```csharp
public string Host { get; set; }
```

#### Property Value

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

### **InstanceName**

```csharp
public string InstanceName { get; set; }
```

#### Property Value

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

### **Port**

```csharp
public Nullable<int> Port { get; set; }
```

#### Property Value

[Nullable\<Int32\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

### **Database**

```csharp
public string Database { get; set; }
```

#### Property Value

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

### **Schema**

```csharp
public string Schema { get; set; }
```

#### Property Value

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

### **Username**

```csharp
public string Username { get; set; }
```

#### Property Value

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

### **Password**

```csharp
public string Password { get; set; }
```

#### Property Value

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

### **License**

```csharp
public string License { get; set; }
```

#### Property Value

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

### **LicenseFile**

```csharp
public string LicenseFile { get; set; }
```

#### Property Value

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

### **IsolationLevel**

```csharp
public IsolationLevel IsolationLevel { get; set; }
```

#### Property Value

IsolationLevel<br/>

### **ConnectionLimit**

```csharp
public int ConnectionLimit { get; set; }
```

#### Property Value

[Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>

### **MaintenanceEnabled**

```csharp
public bool MaintenanceEnabled { get; set; }
```

#### Property Value

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **MaintenanceInterval**

```csharp
public TimeSpan MaintenanceInterval { get; set; }
```

#### Property Value

[TimeSpan](https://learn.microsoft.com/en-us/dotnet/api/system.timespan)<br/>

### **QueueCleanupInterval**

```csharp
public TimeSpan QueueCleanupInterval { get; set; }
```

#### Property Value

[TimeSpan](https://learn.microsoft.com/en-us/dotnet/api/system.timespan)<br/>

### **MaintenanceBatchSize**

```csharp
public int MaintenanceBatchSize { get; set; }
```

#### Property Value

[Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>

### **ConnectionTag**

```csharp
public string ConnectionTag { get; set; }
```

#### Property Value

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

### **VirtualHost**

```csharp
public string VirtualHost { get; set; }
```

#### Property Value

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

### **Area**

```csharp
public string Area { get; set; }
```

#### Property Value

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

### **HostAddress**

```csharp
public Uri HostAddress { get; }
```

#### Property Value

Uri<br/>

## Methods

### **CreateConnectionContextFactory(ISqlHostConfiguration)**

```csharp
public abstract ConnectionContextFactory CreateConnectionContextFactory(ISqlHostConfiguration configuration)
```

#### Parameters

`configuration` [ISqlHostConfiguration](../masstransit-sqltransport-configuration/isqlhostconfiguration)<br/>

#### Returns

[ConnectionContextFactory](../masstransit-sqltransport/connectioncontextfactory)<br/>

### **GetLicenseInfo()**

```csharp
public LicenseInfo GetLicenseInfo()
```

#### Returns

[LicenseInfo](../../masstransit-abstractions/masstransit-licensing/licenseinfo)<br/>

### **Validate()**

```csharp
public IEnumerable<ValidationResult> Validate()
```

#### Returns

[IEnumerable\<ValidationResult\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)<br/>
