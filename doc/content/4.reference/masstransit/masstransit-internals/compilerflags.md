---

title: CompilerFlags

---

# CompilerFlags

Namespace: MassTransit.Internals

The options for the compiler

```csharp
public enum CompilerFlags
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [ValueType](https://learn.microsoft.com/en-us/dotnet/api/system.valuetype) → [Enum](https://learn.microsoft.com/en-us/dotnet/api/system.enum) → [CompilerFlags](../masstransit-internals/compilerflags)<br/>
Implements [IComparable](https://learn.microsoft.com/en-us/dotnet/api/system.icomparable), [ISpanFormattable](https://learn.microsoft.com/en-us/dotnet/api/system.ispanformattable), [IFormattable](https://learn.microsoft.com/en-us/dotnet/api/system.iformattable), [IConvertible](https://learn.microsoft.com/en-us/dotnet/api/system.iconvertible)

## Fields

| Name | Value | Description |
| --- | --: | --- |
| Default | 0 | The default options: Invocation lambda is inlined, no debug info |
| NoInvocationLambdaInlining | 1 | Prevents the inlining of the lambda in the Invocation expression to optimize for the multiple same lambda compiled once |
| EnableDelegateDebugInfo | 2 | Adds the Expression, ExpressionString, and CSharpString to the delegate closure for the debugging inspection |
| ThrowOnNotSupportedExpression | 4 | When the flag set then instead of the returning `null` the specific exception |
