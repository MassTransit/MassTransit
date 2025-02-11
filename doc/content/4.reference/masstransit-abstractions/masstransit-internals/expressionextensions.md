---

title: ExpressionExtensions

---

# ExpressionExtensions

Namespace: MassTransit.Internals

```csharp
public static class ExpressionExtensions
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [ExpressionExtensions](../masstransit-internals/expressionextensions)

## Methods

### **GetMemberName\<T, TMember\>(Expression\<Func\<T, TMember\>\>)**

Gets the name of the member specified

```csharp
public static string GetMemberName<T, TMember>(Expression<Func<T, TMember>> expression)
```

#### Type Parameters

`T`<br/>
The type referenced

`TMember`<br/>
The type of the member referenced

#### Parameters

`expression` Expression\<Func\<T, TMember\>\><br/>
The expression referencing the member

#### Returns

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>
The name of the member referenced by the expression

### **GetMemberName\<T\>(Expression\<Action\<T\>\>)**

Gets the name of the member specified

```csharp
public static string GetMemberName<T>(Expression<Action<T>> expression)
```

#### Type Parameters

`T`<br/>
The type referenced

#### Parameters

`expression` Expression\<Action\<T\>\><br/>
The expression referencing the member

#### Returns

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>
The name of the member referenced by the expression

### **GetMemberName\<T\>(Expression\<Func\<T\>\>)**

```csharp
public static string GetMemberName<T>(Expression<Func<T>> expression)
```

#### Type Parameters

`T`<br/>

#### Parameters

`expression` Expression\<Func\<T\>\><br/>

#### Returns

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

### **GetPropertyInfo\<T, TMember\>(Expression\<Func\<T, TMember\>\>)**

```csharp
public static PropertyInfo GetPropertyInfo<T, TMember>(Expression<Func<T, TMember>> expression)
```

#### Type Parameters

`T`<br/>

`TMember`<br/>

#### Parameters

`expression` Expression\<Func\<T, TMember\>\><br/>

#### Returns

[PropertyInfo](https://learn.microsoft.com/en-us/dotnet/api/system.reflection.propertyinfo)<br/>

### **GetPropertyInfo\<T\>(Expression\<Func\<T\>\>)**

```csharp
public static PropertyInfo GetPropertyInfo<T>(Expression<Func<T>> expression)
```

#### Type Parameters

`T`<br/>

#### Parameters

`expression` Expression\<Func\<T\>\><br/>

#### Returns

[PropertyInfo](https://learn.microsoft.com/en-us/dotnet/api/system.reflection.propertyinfo)<br/>

### **GetMemberInfo\<T\>(Expression\<Action\<T\>\>)**

```csharp
public static MemberInfo GetMemberInfo<T>(Expression<Action<T>> expression)
```

#### Type Parameters

`T`<br/>

#### Parameters

`expression` Expression\<Action\<T\>\><br/>

#### Returns

[MemberInfo](https://learn.microsoft.com/en-us/dotnet/api/system.reflection.memberinfo)<br/>

### **GetMemberExpression\<T, TMember\>(Expression\<Func\<T, TMember\>\>)**

```csharp
public static MemberExpression GetMemberExpression<T, TMember>(Expression<Func<T, TMember>> expression)
```

#### Type Parameters

`T`<br/>

`TMember`<br/>

#### Parameters

`expression` Expression\<Func\<T, TMember\>\><br/>

#### Returns

MemberExpression<br/>

### **GetMemberExpression\<T\>(Expression\<Action\<T\>\>)**

```csharp
public static MemberExpression GetMemberExpression<T>(Expression<Action<T>> expression)
```

#### Type Parameters

`T`<br/>

#### Parameters

`expression` Expression\<Action\<T\>\><br/>

#### Returns

MemberExpression<br/>

### **GetMemberExpression\<T\>(Expression\<Func\<T\>\>)**

```csharp
public static MemberExpression GetMemberExpression<T>(Expression<Func<T>> expression)
```

#### Type Parameters

`T`<br/>

#### Parameters

`expression` Expression\<Func\<T\>\><br/>

#### Returns

MemberExpression<br/>

### **GetMemberExpression\<T1, T2\>(Expression\<Action\<T1, T2\>\>)**

```csharp
public static MemberExpression GetMemberExpression<T1, T2>(Expression<Action<T1, T2>> expression)
```

#### Type Parameters

`T1`<br/>

`T2`<br/>

#### Parameters

`expression` Expression\<Action\<T1, T2\>\><br/>

#### Returns

MemberExpression<br/>
