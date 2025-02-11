---

title: SqlHostConfigurator

---

# SqlHostConfigurator

Namespace: MassTransit.SqlTransport.Configuration

```csharp
public abstract class SqlHostConfigurator : ISqlHostConfigurator
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [SqlHostConfigurator](../masstransit-sqltransport-configuration/sqlhostconfigurator)<br/>
Implements [ISqlHostConfigurator](../masstransit/isqlhostconfigurator)

## Properties

### **ConnectionString**

```csharp
public abstract string ConnectionString { set; }
```

#### Property Value

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

### **ConnectionTag**

```csharp
public string ConnectionTag { set; }
```

#### Property Value

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

### **Host**

```csharp
public string Host { set; }
```

#### Property Value

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

### **InstanceName**

```csharp
public string InstanceName { set; }
```

#### Property Value

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

### **Port**

```csharp
public Nullable<int> Port { set; }
```

#### Property Value

[Nullable\<Int32\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

### **Database**

```csharp
public string Database { set; }
```

#### Property Value

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

### **Schema**

```csharp
public string Schema { set; }
```

#### Property Value

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

### **Username**

```csharp
public string Username { set; }
```

#### Property Value

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

### **Password**

```csharp
public string Password { set; }
```

#### Property Value

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

### **VirtualHost**

```csharp
public string VirtualHost { set; }
```

#### Property Value

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

### **Area**

```csharp
public string Area { set; }
```

#### Property Value

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

### **IsolationLevel**

```csharp
public IsolationLevel IsolationLevel { set; }
```

#### Property Value

IsolationLevel<br/>

### **ConnectionLimit**

```csharp
public int ConnectionLimit { set; }
```

#### Property Value

[Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>

### **MaintenanceEnabled**

```csharp
public bool MaintenanceEnabled { set; }
```

#### Property Value

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **MaintenanceInterval**

```csharp
public TimeSpan MaintenanceInterval { set; }
```

#### Property Value

[TimeSpan](https://learn.microsoft.com/en-us/dotnet/api/system.timespan)<br/>

### **QueueCleanupInterval**

```csharp
public TimeSpan QueueCleanupInterval { set; }
```

#### Property Value

[TimeSpan](https://learn.microsoft.com/en-us/dotnet/api/system.timespan)<br/>

### **MaintenanceBatchSize**

```csharp
public int MaintenanceBatchSize { set; }
```

#### Property Value

[Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>

## Methods

### **UseLicense(String)**

```csharp
public void UseLicense(string license)
```

#### Parameters

`license` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

### **UseLicenseFile(String)**

```csharp
public void UseLicenseFile(string path)
```

#### Parameters

`path` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>
