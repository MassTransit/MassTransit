---

title: SqlTransportMigrationOptions

---

# SqlTransportMigrationOptions

Namespace: MassTransit

```csharp
public class SqlTransportMigrationOptions
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [SqlTransportMigrationOptions](../masstransit/sqltransportmigrationoptions)

## Properties

### **CreateDatabase**

If true, the database and all transport components will be created/updated on startup

```csharp
public bool CreateDatabase { get; set; }
```

#### Property Value

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **CreateSchema**

If true, the schema for transport components will be created/updated on startup
 
 Use this, without CreateDatabase, if you do not have the required permissions to create the schema and grant access

```csharp
public bool CreateSchema { get; set; }
```

#### Property Value

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **CreateInfrastructure**

If true, the infrastructure components for the transport will be created/updated on startup
 
 Use this, without CreateDatabase, if you do not have the required permissions to create databases and logins

```csharp
public bool CreateInfrastructure { get; set; }
```

#### Property Value

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **DeleteDatabase**

If true, the database and all transport components will be deleted on shutdown

```csharp
public bool DeleteDatabase { get; set; }
```

#### Property Value

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

## Constructors

### **SqlTransportMigrationOptions()**

```csharp
public SqlTransportMigrationOptions()
```
