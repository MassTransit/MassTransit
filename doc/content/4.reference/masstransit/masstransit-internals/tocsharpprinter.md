---

title: ToCSharpPrinter

---

# ToCSharpPrinter

Namespace: MassTransit.Internals

Converts the expression into the valid C# code representation

```csharp
public static class ToCSharpPrinter
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [ToCSharpPrinter](../masstransit-internals/tocsharpprinter)

## Methods

### **ToCSharpString(Expression)**

Tries hard to convert the expression into the correct C# code

```csharp
public static string ToCSharpString(Expression expr)
```

#### Parameters

`expr` Expression<br/>

#### Returns

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

### **ToCSharpString(Expression, TryPrintConstant)**

Tries hard to convert the expression into the correct C# code

```csharp
public static string ToCSharpString(Expression expr, TryPrintConstant tryPrintConstant)
```

#### Parameters

`expr` Expression<br/>

`tryPrintConstant` [TryPrintConstant](../masstransit-internals/tryprintconstant)<br/>

#### Returns

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

### **ToCSharpString(Expression, StringBuilder, Int32, Boolean, Func\<Type, String, String\>, Int32, TryPrintConstant)**

Tries hard to convert the expression into the correct C# code

```csharp
public static StringBuilder ToCSharpString(Expression e, StringBuilder sb, int lineIdent, bool stripNamespace, Func<Type, string, string> printType, int identSpaces, TryPrintConstant tryPrintConstant)
```

#### Parameters

`e` Expression<br/>

`sb` [StringBuilder](https://learn.microsoft.com/en-us/dotnet/api/system.text.stringbuilder)<br/>

`lineIdent` [Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>

`stripNamespace` [Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

`printType` [Func\<Type, String, String\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-3)<br/>

`identSpaces` [Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>

`tryPrintConstant` [TryPrintConstant](../masstransit-internals/tryprintconstant)<br/>

#### Returns

[StringBuilder](https://learn.microsoft.com/en-us/dotnet/api/system.text.stringbuilder)<br/>

### **ToCSharpString(LabelTarget, StringBuilder)**

```csharp
internal static StringBuilder ToCSharpString(LabelTarget target, StringBuilder sb)
```

#### Parameters

`target` LabelTarget<br/>

`sb` [StringBuilder](https://learn.microsoft.com/en-us/dotnet/api/system.text.stringbuilder)<br/>

#### Returns

[StringBuilder](https://learn.microsoft.com/en-us/dotnet/api/system.text.stringbuilder)<br/>
