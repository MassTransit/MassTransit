---

title: ISqlHostConfigurator

---

# ISqlHostConfigurator

Namespace: MassTransit

```csharp
public interface ISqlHostConfigurator
```

## Properties

### **ConnectionString**

Set the connection string, which the underlying database provider will parse into its individual components and rebuild at runtime

```csharp
public abstract string ConnectionString { set; }
```

#### Property Value

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

### **ConnectionTag**

Optional, specifies a connection tag used to identify the connection in the database

```csharp
public abstract string ConnectionTag { set; }
```

#### Property Value

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

### **Host**

The database server host name. If using SQL server with an instance, set the [ISqlHostConfigurator.InstanceName](isqlhostconfigurator#instancename) separately.

```csharp
public abstract string Host { set; }
```

#### Property Value

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

### **InstanceName**

The instance name if using SQL Server instances

```csharp
public abstract string InstanceName { set; }
```

#### Property Value

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

### **Port**

Optional, only specify if a custom port is being used.

```csharp
public abstract Nullable<int> Port { set; }
```

#### Property Value

[Nullable\<Int32\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

### **Database**

The database name

```csharp
public abstract string Database { set; }
```

#### Property Value

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

### **Schema**

The schema to use for the transport

```csharp
public abstract string Schema { set; }
```

#### Property Value

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

### **Username**

The username for the bus to access the transport

```csharp
public abstract string Username { set; }
```

#### Property Value

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

### **Password**

The password for the username

```csharp
public abstract string Password { set; }
```

#### Property Value

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

### **VirtualHost**

```csharp
public abstract string VirtualHost { set; }
```

#### Property Value

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

### **Area**

```csharp
public abstract string Area { set; }
```

#### Property Value

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

### **IsolationLevel**

Sets the isolation level used for database transactions (default: Repeatable Read)

```csharp
public abstract IsolationLevel IsolationLevel { set; }
```

#### Property Value

IsolationLevel<br/>

### **ConnectionLimit**

Sets the maximum number of connections used by the SQL transport concurrently.

```csharp
public abstract int ConnectionLimit { set; }
```

#### Property Value

[Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>

### **MaintenanceEnabled**

Should typically be left to the default (true), reserved for use cases such as delegating maintenance activities explicitly as application quantities grow.

```csharp
public abstract bool MaintenanceEnabled { set; }
```

#### Property Value

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **MaintenanceInterval**

How often database maintenance should be performed (metrics consolidation, topology cleanup, etc.)

```csharp
public abstract TimeSpan MaintenanceInterval { set; }
```

#### Property Value

[TimeSpan](https://learn.microsoft.com/en-us/dotnet/api/system.timespan)<br/>

### **MaintenanceBatchSize**

How many metrics events to compute in each batch

```csharp
public abstract int MaintenanceBatchSize { set; }
```

#### Property Value

[Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>

### **QueueCleanupInterval**

How often to purge auto-delete queues from the topology and all expired messages

```csharp
public abstract TimeSpan QueueCleanupInterval { set; }
```

#### Property Value

[TimeSpan](https://learn.microsoft.com/en-us/dotnet/api/system.timespan)<br/>

## Methods

### **UseLicense(String)**

Specify the license text to use

```csharp
void UseLicense(string license)
```

#### Parameters

`license` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>
The license text

### **UseLicenseFile(String)**

Specify the path to the file containing the license text

```csharp
void UseLicenseFile(string path)
```

#### Parameters

`path` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>
The path to the file
