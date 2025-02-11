---

title: ToExpressionPrinter

---

# ToExpressionPrinter

Namespace: MassTransit.Internals

```csharp
public static class ToExpressionPrinter
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [ToExpressionPrinter](../masstransit-internals/toexpressionprinter)

## Methods

### **ToExpressionString(Expression, TryPrintConstant)**

Prints the expression in its constructing syntax -
 helpful to get the expression from the debug session and put into it the code for the test.

```csharp
public static string ToExpressionString(Expression expr, TryPrintConstant tryPrintConstant)
```

#### Parameters

`expr` Expression<br/>

`tryPrintConstant` [TryPrintConstant](../masstransit-internals/tryprintconstant)<br/>

#### Returns

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

### **ToExpressionString(Expression, List\<ParameterExpression\>, List\<Expression\>, List\<LabelTarget\>, Boolean, Func\<Type, String, String\>, Int32, TryPrintConstant)**

Prints the expression in its constructing syntax -
 helpful to get the expression from the debug session and put into it the code for the test.
 In addition, returns the gathered expressions, parameters ad labels.

```csharp
public static string ToExpressionString(Expression expr, out List<ParameterExpression> paramsExprs, out List<Expression> uniqueExprs, out List<LabelTarget> lts, bool stripNamespace, Func<Type, string, string> printType, int identSpaces, TryPrintConstant tryPrintConstant)
```

#### Parameters

`expr` Expression<br/>

`paramsExprs` [List\<ParameterExpression\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.list-1)<br/>

`uniqueExprs` [List\<Expression\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.list-1)<br/>

`lts` [List\<LabelTarget\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.list-1)<br/>

`stripNamespace` [Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

`printType` [Func\<Type, String, String\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-3)<br/>

`identSpaces` [Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>

`tryPrintConstant` [TryPrintConstant](../masstransit-internals/tryprintconstant)<br/>

#### Returns

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

### **ToExpressionString(Expression, StringBuilder, List\<ParameterExpression\>, List\<Expression\>, List\<LabelTarget\>, Int32, Boolean, Func\<Type, String, String\>, Int32, TryPrintConstant)**

```csharp
internal static StringBuilder ToExpressionString(Expression expr, StringBuilder sb, List<ParameterExpression> paramsExprs, List<Expression> uniqueExprs, List<LabelTarget> lts, int lineIdent, bool stripNamespace, Func<Type, string, string> printType, int identSpaces, TryPrintConstant tryPrintConstant)
```

#### Parameters

`expr` Expression<br/>

`sb` [StringBuilder](https://learn.microsoft.com/en-us/dotnet/api/system.text.stringbuilder)<br/>

`paramsExprs` [List\<ParameterExpression\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.list-1)<br/>

`uniqueExprs` [List\<Expression\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.list-1)<br/>

`lts` [List\<LabelTarget\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.list-1)<br/>

`lineIdent` [Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>

`stripNamespace` [Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

`printType` [Func\<Type, String, String\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-3)<br/>

`identSpaces` [Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>

`tryPrintConstant` [TryPrintConstant](../masstransit-internals/tryprintconstant)<br/>

#### Returns

[StringBuilder](https://learn.microsoft.com/en-us/dotnet/api/system.text.stringbuilder)<br/>

### **ToExpressionString(ParameterExpression, StringBuilder, List\<ParameterExpression\>, List\<Expression\>, List\<LabelTarget\>, Int32, Boolean, Func\<Type, String, String\>, Int32, TryPrintConstant)**

```csharp
internal static StringBuilder ToExpressionString(ParameterExpression pe, StringBuilder sb, List<ParameterExpression> paramsExprs, List<Expression> uniqueExprs, List<LabelTarget> lts, int lineIdent, bool stripNamespace, Func<Type, string, string> printType, int identSpaces, TryPrintConstant tryPrintConstant)
```

#### Parameters

`pe` ParameterExpression<br/>

`sb` [StringBuilder](https://learn.microsoft.com/en-us/dotnet/api/system.text.stringbuilder)<br/>

`paramsExprs` [List\<ParameterExpression\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.list-1)<br/>

`uniqueExprs` [List\<Expression\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.list-1)<br/>

`lts` [List\<LabelTarget\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.list-1)<br/>

`lineIdent` [Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>

`stripNamespace` [Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

`printType` [Func\<Type, String, String\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-3)<br/>

`identSpaces` [Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>

`tryPrintConstant` [TryPrintConstant](../masstransit-internals/tryprintconstant)<br/>

#### Returns

[StringBuilder](https://learn.microsoft.com/en-us/dotnet/api/system.text.stringbuilder)<br/>

### **ToExpressionString(LabelTarget, StringBuilder, List\<LabelTarget\>, Int32, Boolean, Func\<Type, String, String\>)**

```csharp
internal static StringBuilder ToExpressionString(LabelTarget lt, StringBuilder sb, List<LabelTarget> labelTargets, int lineIdent, bool stripNamespace, Func<Type, string, string> printType)
```

#### Parameters

`lt` LabelTarget<br/>

`sb` [StringBuilder](https://learn.microsoft.com/en-us/dotnet/api/system.text.stringbuilder)<br/>

`labelTargets` [List\<LabelTarget\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.list-1)<br/>

`lineIdent` [Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>

`stripNamespace` [Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

`printType` [Func\<Type, String, String\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-3)<br/>

#### Returns

[StringBuilder](https://learn.microsoft.com/en-us/dotnet/api/system.text.stringbuilder)<br/>

### **CreateExpressionString(Expression, StringBuilder, List\<ParameterExpression\>, List\<Expression\>, List\<LabelTarget\>, Int32, Boolean, Func\<Type, String, String\>, Int32, TryPrintConstant)**

```csharp
internal static StringBuilder CreateExpressionString(Expression e, StringBuilder sb, List<ParameterExpression> paramsExprs, List<Expression> uniqueExprs, List<LabelTarget> lts, int lineIdent, bool stripNamespace, Func<Type, string, string> printType, int identSpaces, TryPrintConstant tryPrintConstant)
```

#### Parameters

`e` Expression<br/>

`sb` [StringBuilder](https://learn.microsoft.com/en-us/dotnet/api/system.text.stringbuilder)<br/>

`paramsExprs` [List\<ParameterExpression\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.list-1)<br/>

`uniqueExprs` [List\<Expression\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.list-1)<br/>

`lts` [List\<LabelTarget\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.list-1)<br/>

`lineIdent` [Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>

`stripNamespace` [Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

`printType` [Func\<Type, String, String\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-3)<br/>

`identSpaces` [Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>

`tryPrintConstant` [TryPrintConstant](../masstransit-internals/tryprintconstant)<br/>

#### Returns

[StringBuilder](https://learn.microsoft.com/en-us/dotnet/api/system.text.stringbuilder)<br/>
