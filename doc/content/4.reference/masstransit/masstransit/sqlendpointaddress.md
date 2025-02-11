---

title: SqlEndpointAddress

---

# SqlEndpointAddress

Namespace: MassTransit

```csharp
public struct SqlEndpointAddress
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [ValueType](https://learn.microsoft.com/en-us/dotnet/api/system.valuetype) → [SqlEndpointAddress](../masstransit/sqlendpointaddress)

## Fields

### **Scheme**

```csharp
public string Scheme;
```

### **Host**

```csharp
public string Host;
```

### **InstanceName**

```csharp
public string InstanceName;
```

### **Port**

```csharp
public Nullable<int> Port;
```

### **VirtualHost**

```csharp
public string VirtualHost;
```

### **Area**

```csharp
public string Area;
```

### **Name**

```csharp
public string Name;
```

### **AutoDeleteOnIdle**

```csharp
public Nullable<TimeSpan> AutoDeleteOnIdle;
```

### **Type**

```csharp
public AddressType Type;
```

## Constructors

### **SqlEndpointAddress(Uri, Uri, AddressType)**

```csharp
public SqlEndpointAddress(Uri hostAddress, Uri address, AddressType type)
```

#### Parameters

`hostAddress` Uri<br/>

`address` Uri<br/>

`type` [AddressType](../masstransit/addresstype)<br/>

### **SqlEndpointAddress(Uri, String, Nullable\<TimeSpan\>, AddressType)**

```csharp
public SqlEndpointAddress(Uri hostAddress, string name, Nullable<TimeSpan> autoDeleteOnIdle, AddressType type)
```

#### Parameters

`hostAddress` Uri<br/>

`name` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

`autoDeleteOnIdle` [Nullable\<TimeSpan\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

`type` [AddressType](../masstransit/addresstype)<br/>
