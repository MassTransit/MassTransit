---

title: SqlTransportOptions

---

# SqlTransportOptions

Namespace: MassTransit

```csharp
public class SqlTransportOptions
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [SqlTransportOptions](../masstransit/sqltransportoptions)

## Properties

### **Host**

```csharp
public string Host { get; set; }
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

### **Role**

```csharp
public string Role { get; set; }
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

### **AdminUsername**

```csharp
public string AdminUsername { get; set; }
```

#### Property Value

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

### **AdminPassword**

```csharp
public string AdminPassword { get; set; }
```

#### Property Value

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

### **ConnectionString**

Optional, if specified, will be parsed to capture additional properties on the connection.

```csharp
public string ConnectionString { get; set; }
```

#### Property Value

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

### **ConnectionLimit**

If specified, changes the connection limit from the default value (10)

```csharp
public Nullable<int> ConnectionLimit { get; set; }
```

#### Property Value

[Nullable\<Int32\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

### **DisableMaintenance**

Disable maintenance and cleanup jobs (metrics consolidation, topology cleanup, etc.)
 Should typically be left to the default (false), reserved for use cases such as delegating maintenance activities explicitly as application quantities grow.

```csharp
public bool DisableMaintenance { get; set; }
```

#### Property Value

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

## Constructors

### **SqlTransportOptions()**

```csharp
public SqlTransportOptions()
```
