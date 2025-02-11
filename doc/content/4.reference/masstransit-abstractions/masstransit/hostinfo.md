---

title: HostInfo

---

# HostInfo

Namespace: MassTransit

The host where an event or otherwise was produced
 a routing slip

```csharp
public interface HostInfo
```

## Properties

### **MachineName**

The machine name (or role instance name) of the local machine

```csharp
public abstract string MachineName { get; }
```

#### Property Value

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

### **ProcessName**

The process name hosting the routing slip activity

```csharp
public abstract string ProcessName { get; }
```

#### Property Value

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

### **ProcessId**

The processId of the hosting process

```csharp
public abstract int ProcessId { get; }
```

#### Property Value

[Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>

### **Assembly**

The assembly where the exception occurred

```csharp
public abstract string Assembly { get; }
```

#### Property Value

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

### **AssemblyVersion**

The assembly version

```csharp
public abstract string AssemblyVersion { get; }
```

#### Property Value

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

### **FrameworkVersion**

The .NET framework version

```csharp
public abstract string FrameworkVersion { get; }
```

#### Property Value

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

### **MassTransitVersion**

The version of MassTransit used by the process

```csharp
public abstract string MassTransitVersion { get; }
```

#### Property Value

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

### **OperatingSystemVersion**

The operating system version hosting the application

```csharp
public abstract string OperatingSystemVersion { get; }
```

#### Property Value

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>
