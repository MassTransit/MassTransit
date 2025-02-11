---

title: Vertex

---

# Vertex

Namespace: MassTransit.SagaStateMachine

```csharp
public class Vertex : IEquatable<Vertex>
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [Vertex](../masstransit-sagastatemachine/vertex)<br/>
Implements [IEquatable\<Vertex\>](https://learn.microsoft.com/en-us/dotnet/api/system.iequatable-1)

## Properties

### **Title**

```csharp
public string Title { get; }
```

#### Property Value

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

### **VertexType**

```csharp
public Type VertexType { get; }
```

#### Property Value

[Type](https://learn.microsoft.com/en-us/dotnet/api/system.type)<br/>

### **TargetType**

```csharp
public Type TargetType { get; }
```

#### Property Value

[Type](https://learn.microsoft.com/en-us/dotnet/api/system.type)<br/>

### **IsComposite**

```csharp
public bool IsComposite { get; }
```

#### Property Value

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **DebuggerDisplay**

```csharp
public string DebuggerDisplay { get; }
```

#### Property Value

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

## Constructors

### **Vertex(Type, Type, String, Boolean)**

```csharp
public Vertex(Type type, Type targetType, string title, bool isComposite)
```

#### Parameters

`type` [Type](https://learn.microsoft.com/en-us/dotnet/api/system.type)<br/>

`targetType` [Type](https://learn.microsoft.com/en-us/dotnet/api/system.type)<br/>

`title` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

`isComposite` [Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

## Methods

### **Equals(Vertex)**

```csharp
public bool Equals(Vertex other)
```

#### Parameters

`other` [Vertex](../masstransit-sagastatemachine/vertex)<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **Equals(Object)**

```csharp
public bool Equals(object obj)
```

#### Parameters

`obj` [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **GetHashCode()**

```csharp
public int GetHashCode()
```

#### Returns

[Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>

### **ToString()**

```csharp
public string ToString()
```

#### Returns

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>
