---

title: IDelegateDebugInfo

---

# IDelegateDebugInfo

Namespace: MassTransit.Internals

The interface is implemented by the compiled delegate Target if `CompilerFlags.EnableDelegateDebugInfo` is set.

```csharp
public interface IDelegateDebugInfo
```

## Properties

### **Expression**

The lambda expression object that was compiled to the delegate

```csharp
public abstract LambdaExpression Expression { get; }
```

#### Property Value

LambdaExpression<br/>

### **ExpressionString**

The lambda expression construction syntax C# code

```csharp
public abstract string ExpressionString { get; }
```

#### Property Value

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

### **CSharpString**

The lambda expression equivalent C# code

```csharp
public abstract string CSharpString { get; }
```

#### Property Value

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>
