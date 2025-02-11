---

title: ILGeneratorHacks

---

# ILGeneratorHacks

Namespace: MassTransit.Internals

Reflecting the internal methods to access the more performant for defining the local variable

```csharp
public static class ILGeneratorHacks
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [ILGeneratorHacks](../masstransit-internals/ilgeneratorhacks)

## Methods

### **PostInc(Int32)**

```csharp
internal static int PostInc(ref int i)
```

#### Parameters

`i` [Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>

#### Returns

[Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>

### **GetNextLocalVarIndex(ILGenerator, Type)**

Efficiently returns the next variable index, hopefully without unnecessary allocations.

```csharp
public static int GetNextLocalVarIndex(ILGenerator il, Type t)
```

#### Parameters

`il` [ILGenerator](https://learn.microsoft.com/en-us/dotnet/api/system.reflection.emit.ilgenerator)<br/>

`t` [Type](https://learn.microsoft.com/en-us/dotnet/api/system.type)<br/>

#### Returns

[Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>
