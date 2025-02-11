---

title: SqlHostAddress

---

# SqlHostAddress

Namespace: MassTransit

The database host address is composed of specific parts
 db://localhost/virtual_host_name.scope
 db://localhost/.scope
 FragmentDescriptionHostThe host name from the connection string, or the host alias if configuredVirtual Host
            The name for an isolated set of topics, queues, and subscriptions in the host/schema specified by the connection string.
            If not specified, the default virtual host is used.
            AreaThe name an area, which contains one or more queues. If not specified, the default area within the virtual host is used.

```csharp
public struct SqlHostAddress
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [ValueType](https://learn.microsoft.com/en-us/dotnet/api/system.valuetype) → [SqlHostAddress](../masstransit/sqlhostaddress)

## Fields

### **Scheme**

```csharp
public string Scheme;
```

### **Host**

```csharp
public string Host;
```

### **Port**

```csharp
public Nullable<int> Port;
```

### **InstanceName**

```csharp
public string InstanceName;
```

### **VirtualHost**

```csharp
public string VirtualHost;
```

### **Area**

```csharp
public string Area;
```

### **DbScheme**

```csharp
public static string DbScheme;
```

## Constructors

### **SqlHostAddress(Uri)**

```csharp
public SqlHostAddress(Uri address)
```

#### Parameters

`address` Uri<br/>

### **SqlHostAddress(String, String, Nullable\<Int32\>, String, String)**

```csharp
public SqlHostAddress(string host, string instanceName, Nullable<int> port, string virtualHost, string area)
```

#### Parameters

`host` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

`instanceName` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

`port` [Nullable\<Int32\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

`virtualHost` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

`area` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

## Methods

### **ParseLeft(Uri, String, String, Nullable\<Int32\>, String, String)**

```csharp
internal static void ParseLeft(Uri address, out string scheme, out string host, out Nullable<int> port, out string virtualHost, out string area)
```

#### Parameters

`address` Uri<br/>

`scheme` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

`host` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

`port` [Nullable\<Int32\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

`virtualHost` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

`area` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>
