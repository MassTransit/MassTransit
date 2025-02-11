---

title: CodePrinter

---

# CodePrinter

Namespace: MassTransit.Internals

```csharp
public static class CodePrinter
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [CodePrinter](../masstransit-internals/codeprinter)

## Methods

### **AppendTypeof(StringBuilder, Type, Boolean, Func\<Type, String, String\>, Boolean)**

```csharp
public static StringBuilder AppendTypeof(StringBuilder sb, Type type, bool stripNamespace, Func<Type, string, string> printType, bool printGenericTypeArgs)
```

#### Parameters

`sb` [StringBuilder](https://learn.microsoft.com/en-us/dotnet/api/system.text.stringbuilder)<br/>

`type` [Type](https://learn.microsoft.com/en-us/dotnet/api/system.type)<br/>

`stripNamespace` [Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

`printType` [Func\<Type, String, String\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-3)<br/>

`printGenericTypeArgs` [Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

#### Returns

[StringBuilder](https://learn.microsoft.com/en-us/dotnet/api/system.text.stringbuilder)<br/>

### **AppendTypeofList(StringBuilder, Type[], Boolean, Func\<Type, String, String\>, Boolean)**

```csharp
public static StringBuilder AppendTypeofList(StringBuilder sb, Type[] types, bool stripNamespace, Func<Type, string, string> printType, bool printGenericTypeArgs)
```

#### Parameters

`sb` [StringBuilder](https://learn.microsoft.com/en-us/dotnet/api/system.text.stringbuilder)<br/>

`types` [Type[]](https://learn.microsoft.com/en-us/dotnet/api/system.type)<br/>

`stripNamespace` [Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

`printType` [Func\<Type, String, String\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-3)<br/>

`printGenericTypeArgs` [Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

#### Returns

[StringBuilder](https://learn.microsoft.com/en-us/dotnet/api/system.text.stringbuilder)<br/>

### **AppendMember(StringBuilder, MemberInfo, Boolean, Func\<Type, String, String\>)**

```csharp
internal static StringBuilder AppendMember(StringBuilder sb, MemberInfo member, bool stripNamespace, Func<Type, string, string> printType)
```

#### Parameters

`sb` [StringBuilder](https://learn.microsoft.com/en-us/dotnet/api/system.text.stringbuilder)<br/>

`member` [MemberInfo](https://learn.microsoft.com/en-us/dotnet/api/system.reflection.memberinfo)<br/>

`stripNamespace` [Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

`printType` [Func\<Type, String, String\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-3)<br/>

#### Returns

[StringBuilder](https://learn.microsoft.com/en-us/dotnet/api/system.text.stringbuilder)<br/>

### **AppendField(StringBuilder, FieldInfo, Boolean, Func\<Type, String, String\>)**

```csharp
internal static StringBuilder AppendField(StringBuilder sb, FieldInfo field, bool stripNamespace, Func<Type, string, string> printType)
```

#### Parameters

`sb` [StringBuilder](https://learn.microsoft.com/en-us/dotnet/api/system.text.stringbuilder)<br/>

`field` [FieldInfo](https://learn.microsoft.com/en-us/dotnet/api/system.reflection.fieldinfo)<br/>

`stripNamespace` [Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

`printType` [Func\<Type, String, String\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-3)<br/>

#### Returns

[StringBuilder](https://learn.microsoft.com/en-us/dotnet/api/system.text.stringbuilder)<br/>

### **AppendProperty(StringBuilder, PropertyInfo, Boolean, Func\<Type, String, String\>)**

```csharp
internal static StringBuilder AppendProperty(StringBuilder sb, PropertyInfo property, bool stripNamespace, Func<Type, string, string> printType)
```

#### Parameters

`sb` [StringBuilder](https://learn.microsoft.com/en-us/dotnet/api/system.text.stringbuilder)<br/>

`property` [PropertyInfo](https://learn.microsoft.com/en-us/dotnet/api/system.reflection.propertyinfo)<br/>

`stripNamespace` [Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

`printType` [Func\<Type, String, String\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-3)<br/>

#### Returns

[StringBuilder](https://learn.microsoft.com/en-us/dotnet/api/system.text.stringbuilder)<br/>

### **AppendEnum\<TEnum\>(StringBuilder, TEnum, Boolean, Func\<Type, String, String\>)**

```csharp
internal static StringBuilder AppendEnum<TEnum>(StringBuilder sb, TEnum value, bool stripNamespace, Func<Type, string, string> printType)
```

#### Type Parameters

`TEnum`<br/>

#### Parameters

`sb` [StringBuilder](https://learn.microsoft.com/en-us/dotnet/api/system.text.stringbuilder)<br/>

`value` TEnum<br/>

`stripNamespace` [Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

`printType` [Func\<Type, String, String\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-3)<br/>

#### Returns

[StringBuilder](https://learn.microsoft.com/en-us/dotnet/api/system.text.stringbuilder)<br/>

### **AppendMethod(StringBuilder, MethodInfo, Boolean, Func\<Type, String, String\>)**

```csharp
public static StringBuilder AppendMethod(StringBuilder sb, MethodInfo method, bool stripNamespace, Func<Type, string, string> printType)
```

#### Parameters

`sb` [StringBuilder](https://learn.microsoft.com/en-us/dotnet/api/system.text.stringbuilder)<br/>

`method` [MethodInfo](https://learn.microsoft.com/en-us/dotnet/api/system.reflection.methodinfo)<br/>

`stripNamespace` [Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

`printType` [Func\<Type, String, String\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-3)<br/>

#### Returns

[StringBuilder](https://learn.microsoft.com/en-us/dotnet/api/system.text.stringbuilder)<br/>

### **AppendName\<T\>(StringBuilder, String, Type, T)**

```csharp
internal static StringBuilder AppendName<T>(StringBuilder sb, string name, Type type, T identity)
```

#### Type Parameters

`T`<br/>

#### Parameters

`sb` [StringBuilder](https://learn.microsoft.com/en-us/dotnet/api/system.text.stringbuilder)<br/>

`name` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

`type` [Type](https://learn.microsoft.com/en-us/dotnet/api/system.type)<br/>

`identity` T<br/>

#### Returns

[StringBuilder](https://learn.microsoft.com/en-us/dotnet/api/system.text.stringbuilder)<br/>

### **ToCode(Type, Boolean, Func\<Type, String, String\>, Boolean)**

Converts the  into the proper C# representation.

```csharp
public static string ToCode(Type type, bool stripNamespace, Func<Type, string, string> printType, bool printGenericTypeArgs)
```

#### Parameters

`type` [Type](https://learn.microsoft.com/en-us/dotnet/api/system.type)<br/>

`stripNamespace` [Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

`printType` [Func\<Type, String, String\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-3)<br/>

`printGenericTypeArgs` [Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

#### Returns

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

### **ToCode(Boolean)**

Prints valid C# Boolean

```csharp
public static string ToCode(bool x)
```

#### Parameters

`x` [Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

#### Returns

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

### **ToCode(String)**

Prints valid C# String escaping the things

```csharp
public static string ToCode(string x)
```

#### Parameters

`x` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

#### Returns

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

### **ToEnumValueCode(Type, Object, Boolean, Func\<Type, String, String\>)**

Prints valid C# Enum literal

```csharp
public static string ToEnumValueCode(Type enumType, object x, bool stripNamespace, Func<Type, string, string> printType)
```

#### Parameters

`enumType` [Type](https://learn.microsoft.com/en-us/dotnet/api/system.type)<br/>

`x` [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>

`stripNamespace` [Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

`printType` [Func\<Type, String, String\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-3)<br/>

#### Returns

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

### **ToCommaSeparatedCode(IEnumerable, IObjectToCode, Boolean, Func\<Type, String, String\>)**

Prints many code items as the array initializer.

```csharp
public static string ToCommaSeparatedCode(IEnumerable items, IObjectToCode notRecognizedToCode, bool stripNamespace, Func<Type, string, string> printType)
```

#### Parameters

`items` [IEnumerable](https://learn.microsoft.com/en-us/dotnet/api/system.collections.ienumerable)<br/>

`notRecognizedToCode` [IObjectToCode](../masstransit-internals/iobjecttocode)<br/>

`stripNamespace` [Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

`printType` [Func\<Type, String, String\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-3)<br/>

#### Returns

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

### **ToArrayInitializerCode(IEnumerable, Type, IObjectToCode, Boolean, Func\<Type, String, String\>)**

Prints many code items as array initializer.

```csharp
public static string ToArrayInitializerCode(IEnumerable items, Type itemType, IObjectToCode notRecognizedToCode, bool stripNamespace, Func<Type, string, string> printType)
```

#### Parameters

`items` [IEnumerable](https://learn.microsoft.com/en-us/dotnet/api/system.collections.ienumerable)<br/>

`itemType` [Type](https://learn.microsoft.com/en-us/dotnet/api/system.type)<br/>

`notRecognizedToCode` [IObjectToCode](../masstransit-internals/iobjecttocode)<br/>

`stripNamespace` [Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

`printType` [Func\<Type, String, String\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-3)<br/>

#### Returns

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

### **ToCode(Object, IObjectToCode, Boolean, Func\<Type, String, String\>)**

Prints a valid C# for known ,
 otherwise uses passed  or falls back to `ToString()`.

```csharp
public static string ToCode(object x, IObjectToCode notRecognizedToCode, bool stripNamespace, Func<Type, string, string> printType)
```

#### Parameters

`x` [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>

`notRecognizedToCode` [IObjectToCode](../masstransit-internals/iobjecttocode)<br/>

`stripNamespace` [Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

`printType` [Func\<Type, String, String\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-3)<br/>

#### Returns

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

### **NewLineIdent(StringBuilder, Int32)**

```csharp
internal static StringBuilder NewLineIdent(StringBuilder sb, int lineIdent)
```

#### Parameters

`sb` [StringBuilder](https://learn.microsoft.com/en-us/dotnet/api/system.text.stringbuilder)<br/>

`lineIdent` [Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>

#### Returns

[StringBuilder](https://learn.microsoft.com/en-us/dotnet/api/system.text.stringbuilder)<br/>

### **NewLine(StringBuilder, Int32, Int32)**

```csharp
internal static StringBuilder NewLine(StringBuilder sb, int lineIdent, int identSpaces)
```

#### Parameters

`sb` [StringBuilder](https://learn.microsoft.com/en-us/dotnet/api/system.text.stringbuilder)<br/>

`lineIdent` [Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>

`identSpaces` [Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>

#### Returns

[StringBuilder](https://learn.microsoft.com/en-us/dotnet/api/system.text.stringbuilder)<br/>

### **NewLineIdentExpr(StringBuilder, Expression, List\<ParameterExpression\>, List\<Expression\>, List\<LabelTarget\>, Int32, Boolean, Func\<Type, String, String\>, Int32, TryPrintConstant)**

```csharp
internal static StringBuilder NewLineIdentExpr(StringBuilder sb, Expression expr, List<ParameterExpression> paramsExprs, List<Expression> uniqueExprs, List<LabelTarget> lts, int lineIdent, bool stripNamespace, Func<Type, string, string> printType, int identSpaces, TryPrintConstant tryPrintConstant)
```

#### Parameters

`sb` [StringBuilder](https://learn.microsoft.com/en-us/dotnet/api/system.text.stringbuilder)<br/>

`expr` Expression<br/>

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

### **NewLineIdentArgumentExprs\<T\>(StringBuilder, IReadOnlyList\<T\>, List\<ParameterExpression\>, List\<Expression\>, List\<LabelTarget\>, Int32, Boolean, Func\<Type, String, String\>, Int32, TryPrintConstant)**

```csharp
internal static StringBuilder NewLineIdentArgumentExprs<T>(StringBuilder sb, IReadOnlyList<T> exprs, List<ParameterExpression> paramsExprs, List<Expression> uniqueExprs, List<LabelTarget> lts, int lineIdent, bool stripNamespace, Func<Type, string, string> printType, int identSpaces, TryPrintConstant tryPrintConstant)
```

#### Type Parameters

`T`<br/>

#### Parameters

`sb` [StringBuilder](https://learn.microsoft.com/en-us/dotnet/api/system.text.stringbuilder)<br/>

`exprs` [IReadOnlyList\<T\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ireadonlylist-1)<br/>

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

### **NewLineIdentCs(StringBuilder, Expression, Int32, Boolean, Func\<Type, String, String\>, Int32, TryPrintConstant)**

```csharp
internal static StringBuilder NewLineIdentCs(StringBuilder sb, Expression expr, int lineIdent, bool stripNamespace, Func<Type, string, string> printType, int identSpaces, TryPrintConstant tryPrintConstant)
```

#### Parameters

`sb` [StringBuilder](https://learn.microsoft.com/en-us/dotnet/api/system.text.stringbuilder)<br/>

`expr` Expression<br/>

`lineIdent` [Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>

`stripNamespace` [Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

`printType` [Func\<Type, String, String\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-3)<br/>

`identSpaces` [Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>

`tryPrintConstant` [TryPrintConstant](../masstransit-internals/tryprintconstant)<br/>

#### Returns

[StringBuilder](https://learn.microsoft.com/en-us/dotnet/api/system.text.stringbuilder)<br/>
