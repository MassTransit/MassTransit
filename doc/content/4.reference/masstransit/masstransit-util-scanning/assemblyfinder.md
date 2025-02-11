---

title: AssemblyFinder

---

# AssemblyFinder

Namespace: MassTransit.Util.Scanning

```csharp
public class AssemblyFinder
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [AssemblyFinder](../masstransit-util-scanning/assemblyfinder)

## Constructors

### **AssemblyFinder()**

```csharp
public AssemblyFinder()
```

## Methods

### **FindAssemblies(AssemblyLoadFailure, Boolean, AssemblyFilter)**

```csharp
public static IEnumerable<Assembly> FindAssemblies(AssemblyLoadFailure loadFailure, bool includeExeFiles, AssemblyFilter filter)
```

#### Parameters

`loadFailure` [AssemblyLoadFailure](../masstransit-util-scanning/assemblyloadfailure)<br/>

`includeExeFiles` [Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

`filter` [AssemblyFilter](../masstransit-util-scanning/assemblyfilter)<br/>

#### Returns

[IEnumerable\<Assembly\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)<br/>

### **FindAssemblies(String, AssemblyLoadFailure, Boolean, AssemblyFilter)**

```csharp
public static IEnumerable<Assembly> FindAssemblies(string assemblyPath, AssemblyLoadFailure loadFailure, bool includeExeFiles, AssemblyFilter filter)
```

#### Parameters

`assemblyPath` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

`loadFailure` [AssemblyLoadFailure](../masstransit-util-scanning/assemblyloadfailure)<br/>

`includeExeFiles` [Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

`filter` [AssemblyFilter](../masstransit-util-scanning/assemblyfilter)<br/>

#### Returns

[IEnumerable\<Assembly\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)<br/>
