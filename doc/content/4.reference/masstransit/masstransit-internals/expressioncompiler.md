---

title: ExpressionCompiler

---

# ExpressionCompiler

Namespace: MassTransit.Internals

Compiles expression to delegate ~20 times faster than Expression.Compile.
 Partial to extend with your things when used as source file.

```csharp
public static class ExpressionCompiler
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [ExpressionCompiler](../masstransit-internals/expressioncompiler)

## Fields

### **EmptyArrayClosure**

```csharp
public static ArrayClosure EmptyArrayClosure;
```

### **ArrayClosureArrayField**

```csharp
public static FieldInfo ArrayClosureArrayField;
```

### **ArrayClosureWithNonPassedParamsField**

```csharp
public static FieldInfo ArrayClosureWithNonPassedParamsField;
```

### **ArrayClosureWithNonPassedParamsConstructor**

```csharp
public static ConstructorInfo ArrayClosureWithNonPassedParamsConstructor;
```

### **ArrayClosureWithNonPassedParamsConstructorWithoutConstants**

```csharp
public static ConstructorInfo ArrayClosureWithNonPassedParamsConstructorWithoutConstants;
```

## Methods

### **CompileFast\<TDelegate\>(LambdaExpression, Boolean, CompilerFlags)**

Compiles lambda expression to TDelegate type. Use ifFastFailedReturnNull parameter to Not fallback to Expression.Compile, useful for testing.

```csharp
public static TDelegate CompileFast<TDelegate>(LambdaExpression lambdaExpr, bool ifFastFailedReturnNull, CompilerFlags flags)
```

#### Type Parameters

`TDelegate`<br/>

#### Parameters

`lambdaExpr` LambdaExpression<br/>

`ifFastFailedReturnNull` [Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

`flags` [CompilerFlags](../masstransit-internals/compilerflags)<br/>

#### Returns

TDelegate<br/>

### **CompileFastToIL(LambdaExpression, ILGenerator, Boolean, CompilerFlags)**

```csharp
public static bool CompileFastToIL(LambdaExpression lambdaExpr, ILGenerator il, bool ifFastFailedReturnNull, CompilerFlags flags)
```

#### Parameters

`lambdaExpr` LambdaExpression<br/>

`il` [ILGenerator](https://learn.microsoft.com/en-us/dotnet/api/system.reflection.emit.ilgenerator)<br/>

`ifFastFailedReturnNull` [Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

`flags` [CompilerFlags](../masstransit-internals/compilerflags)<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **CompileFast(LambdaExpression, Boolean, CompilerFlags)**

Compiles lambda expression to delegate. Use ifFastFailedReturnNull parameter to Not fallback to Expression.Compile, useful for testing.

```csharp
public static Delegate CompileFast(LambdaExpression lambdaExpr, bool ifFastFailedReturnNull, CompilerFlags flags)
```

#### Parameters

`lambdaExpr` LambdaExpression<br/>

`ifFastFailedReturnNull` [Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

`flags` [CompilerFlags](../masstransit-internals/compilerflags)<br/>

#### Returns

[Delegate](https://learn.microsoft.com/en-us/dotnet/api/system.delegate)<br/>

### **CompileSys\<TDelegate\>(Expression\<TDelegate\>)**

Unifies Compile for System.Linq.Expressions and FEC.LightExpression

```csharp
public static TDelegate CompileSys<TDelegate>(Expression<TDelegate> lambdaExpr)
```

#### Type Parameters

`TDelegate`<br/>

#### Parameters

`lambdaExpr` Expression\<TDelegate\><br/>

#### Returns

TDelegate<br/>

### **CompileSys(LambdaExpression)**

Unifies Compile for System.Linq.Expressions and FEC.LightExpression

```csharp
public static Delegate CompileSys(LambdaExpression lambdaExpr)
```

#### Parameters

`lambdaExpr` LambdaExpression<br/>

#### Returns

[Delegate](https://learn.microsoft.com/en-us/dotnet/api/system.delegate)<br/>

### **CompileFast\<TDelegate\>(Expression\<TDelegate\>, Boolean, CompilerFlags)**

Compiles lambda expression to TDelegate type. Use ifFastFailedReturnNull parameter to Not fallback to Expression.Compile, useful for testing.

```csharp
public static TDelegate CompileFast<TDelegate>(Expression<TDelegate> lambdaExpr, bool ifFastFailedReturnNull, CompilerFlags flags)
```

#### Type Parameters

`TDelegate`<br/>

#### Parameters

`lambdaExpr` Expression\<TDelegate\><br/>

`ifFastFailedReturnNull` [Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

`flags` [CompilerFlags](../masstransit-internals/compilerflags)<br/>

#### Returns

TDelegate<br/>

### **CompileFast\<R\>(Expression\<Func\<R\>\>, Boolean, CompilerFlags)**

Compiles lambda expression to delegate. Use ifFastFailedReturnNull parameter to Not fallback to Expression.Compile, useful for testing.

```csharp
public static Func<R> CompileFast<R>(Expression<Func<R>> lambdaExpr, bool ifFastFailedReturnNull, CompilerFlags flags)
```

#### Type Parameters

`R`<br/>

#### Parameters

`lambdaExpr` Expression\<Func\<R\>\><br/>

`ifFastFailedReturnNull` [Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

`flags` [CompilerFlags](../masstransit-internals/compilerflags)<br/>

#### Returns

[Func\<R\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-1)<br/>

### **CompileFast\<T1, R\>(Expression\<Func\<T1, R\>\>, Boolean, CompilerFlags)**

Compiles lambda expression to delegate. Use ifFastFailedReturnNull parameter to Not fallback to Expression.Compile, useful for testing.

```csharp
public static Func<T1, R> CompileFast<T1, R>(Expression<Func<T1, R>> lambdaExpr, bool ifFastFailedReturnNull, CompilerFlags flags)
```

#### Type Parameters

`T1`<br/>

`R`<br/>

#### Parameters

`lambdaExpr` Expression\<Func\<T1, R\>\><br/>

`ifFastFailedReturnNull` [Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

`flags` [CompilerFlags](../masstransit-internals/compilerflags)<br/>

#### Returns

[Func\<T1, R\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>

### **CompileFast\<T1, T2, R\>(Expression\<Func\<T1, T2, R\>\>, Boolean, CompilerFlags)**

Compiles lambda expression to TDelegate type. Use ifFastFailedReturnNull parameter to Not fallback to Expression.Compile, useful for testing.

```csharp
public static Func<T1, T2, R> CompileFast<T1, T2, R>(Expression<Func<T1, T2, R>> lambdaExpr, bool ifFastFailedReturnNull, CompilerFlags flags)
```

#### Type Parameters

`T1`<br/>

`T2`<br/>

`R`<br/>

#### Parameters

`lambdaExpr` Expression\<Func\<T1, T2, R\>\><br/>

`ifFastFailedReturnNull` [Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

`flags` [CompilerFlags](../masstransit-internals/compilerflags)<br/>

#### Returns

[Func\<T1, T2, R\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-3)<br/>

### **CompileFast\<T1, T2, T3, R\>(Expression\<Func\<T1, T2, T3, R\>\>, Boolean, CompilerFlags)**

Compiles lambda expression to delegate. Use ifFastFailedReturnNull parameter to Not fallback to Expression.Compile, useful for testing.

```csharp
public static Func<T1, T2, T3, R> CompileFast<T1, T2, T3, R>(Expression<Func<T1, T2, T3, R>> lambdaExpr, bool ifFastFailedReturnNull, CompilerFlags flags)
```

#### Type Parameters

`T1`<br/>

`T2`<br/>

`T3`<br/>

`R`<br/>

#### Parameters

`lambdaExpr` Expression\<Func\<T1, T2, T3, R\>\><br/>

`ifFastFailedReturnNull` [Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

`flags` [CompilerFlags](../masstransit-internals/compilerflags)<br/>

#### Returns

[Func\<T1, T2, T3, R\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-4)<br/>

### **CompileFast\<T1, T2, T3, T4, R\>(Expression\<Func\<T1, T2, T3, T4, R\>\>, Boolean, CompilerFlags)**

Compiles lambda expression to TDelegate type. Use ifFastFailedReturnNull parameter to Not fallback to Expression.Compile, useful for testing.

```csharp
public static Func<T1, T2, T3, T4, R> CompileFast<T1, T2, T3, T4, R>(Expression<Func<T1, T2, T3, T4, R>> lambdaExpr, bool ifFastFailedReturnNull, CompilerFlags flags)
```

#### Type Parameters

`T1`<br/>

`T2`<br/>

`T3`<br/>

`T4`<br/>

`R`<br/>

#### Parameters

`lambdaExpr` Expression\<Func\<T1, T2, T3, T4, R\>\><br/>

`ifFastFailedReturnNull` [Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

`flags` [CompilerFlags](../masstransit-internals/compilerflags)<br/>

#### Returns

[Func\<T1, T2, T3, T4, R\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-5)<br/>

### **CompileFast\<T1, T2, T3, T4, T5, R\>(Expression\<Func\<T1, T2, T3, T4, T5, R\>\>, Boolean, CompilerFlags)**

Compiles lambda expression to delegate. Use ifFastFailedReturnNull parameter to Not fallback to Expression.Compile, useful for testing.

```csharp
public static Func<T1, T2, T3, T4, T5, R> CompileFast<T1, T2, T3, T4, T5, R>(Expression<Func<T1, T2, T3, T4, T5, R>> lambdaExpr, bool ifFastFailedReturnNull, CompilerFlags flags)
```

#### Type Parameters

`T1`<br/>

`T2`<br/>

`T3`<br/>

`T4`<br/>

`T5`<br/>

`R`<br/>

#### Parameters

`lambdaExpr` Expression\<Func\<T1, T2, T3, T4, T5, R\>\><br/>

`ifFastFailedReturnNull` [Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

`flags` [CompilerFlags](../masstransit-internals/compilerflags)<br/>

#### Returns

[Func\<T1, T2, T3, T4, T5, R\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-6)<br/>

### **CompileFast\<T1, T2, T3, T4, T5, T6, R\>(Expression\<Func\<T1, T2, T3, T4, T5, T6, R\>\>, Boolean, CompilerFlags)**

Compiles lambda expression to delegate. Use ifFastFailedReturnNull parameter to Not fallback to Expression.Compile, useful for testing.

```csharp
public static Func<T1, T2, T3, T4, T5, T6, R> CompileFast<T1, T2, T3, T4, T5, T6, R>(Expression<Func<T1, T2, T3, T4, T5, T6, R>> lambdaExpr, bool ifFastFailedReturnNull, CompilerFlags flags)
```

#### Type Parameters

`T1`<br/>

`T2`<br/>

`T3`<br/>

`T4`<br/>

`T5`<br/>

`T6`<br/>

`R`<br/>

#### Parameters

`lambdaExpr` Expression\<Func\<T1, T2, T3, T4, T5, T6, R\>\><br/>

`ifFastFailedReturnNull` [Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

`flags` [CompilerFlags](../masstransit-internals/compilerflags)<br/>

#### Returns

[Func\<T1, T2, T3, T4, T5, T6, R\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-7)<br/>

### **CompileFast(Expression\<Action\>, Boolean, CompilerFlags)**

Compiles lambda expression to delegate. Use ifFastFailedReturnNull parameter to Not fallback to Expression.Compile, useful for testing.

```csharp
public static Action CompileFast(Expression<Action> lambdaExpr, bool ifFastFailedReturnNull, CompilerFlags flags)
```

#### Parameters

`lambdaExpr` Expression\<Action\><br/>

`ifFastFailedReturnNull` [Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

`flags` [CompilerFlags](../masstransit-internals/compilerflags)<br/>

#### Returns

[Action](https://learn.microsoft.com/en-us/dotnet/api/system.action)<br/>

### **CompileFast\<T1\>(Expression\<Action\<T1\>\>, Boolean, CompilerFlags)**

Compiles lambda expression to delegate. Use ifFastFailedReturnNull parameter to Not fallback to Expression.Compile, useful for testing.

```csharp
public static Action<T1> CompileFast<T1>(Expression<Action<T1>> lambdaExpr, bool ifFastFailedReturnNull, CompilerFlags flags)
```

#### Type Parameters

`T1`<br/>

#### Parameters

`lambdaExpr` Expression\<Action\<T1\>\><br/>

`ifFastFailedReturnNull` [Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

`flags` [CompilerFlags](../masstransit-internals/compilerflags)<br/>

#### Returns

[Action\<T1\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

### **CompileFast\<T1, T2\>(Expression\<Action\<T1, T2\>\>, Boolean, CompilerFlags)**

Compiles lambda expression to delegate. Use ifFastFailedReturnNull parameter to Not fallback to Expression.Compile, useful for testing.

```csharp
public static Action<T1, T2> CompileFast<T1, T2>(Expression<Action<T1, T2>> lambdaExpr, bool ifFastFailedReturnNull, CompilerFlags flags)
```

#### Type Parameters

`T1`<br/>

`T2`<br/>

#### Parameters

`lambdaExpr` Expression\<Action\<T1, T2\>\><br/>

`ifFastFailedReturnNull` [Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

`flags` [CompilerFlags](../masstransit-internals/compilerflags)<br/>

#### Returns

[Action\<T1, T2\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-2)<br/>

### **CompileFast\<T1, T2, T3\>(Expression\<Action\<T1, T2, T3\>\>, Boolean, CompilerFlags)**

Compiles lambda expression to delegate. Use ifFastFailedReturnNull parameter to Not fallback to Expression.Compile, useful for testing.

```csharp
public static Action<T1, T2, T3> CompileFast<T1, T2, T3>(Expression<Action<T1, T2, T3>> lambdaExpr, bool ifFastFailedReturnNull, CompilerFlags flags)
```

#### Type Parameters

`T1`<br/>

`T2`<br/>

`T3`<br/>

#### Parameters

`lambdaExpr` Expression\<Action\<T1, T2, T3\>\><br/>

`ifFastFailedReturnNull` [Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

`flags` [CompilerFlags](../masstransit-internals/compilerflags)<br/>

#### Returns

[Action\<T1, T2, T3\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-3)<br/>

### **CompileFast\<T1, T2, T3, T4\>(Expression\<Action\<T1, T2, T3, T4\>\>, Boolean, CompilerFlags)**

Compiles lambda expression to delegate. Use ifFastFailedReturnNull parameter to Not fallback to Expression.Compile, useful for testing.

```csharp
public static Action<T1, T2, T3, T4> CompileFast<T1, T2, T3, T4>(Expression<Action<T1, T2, T3, T4>> lambdaExpr, bool ifFastFailedReturnNull, CompilerFlags flags)
```

#### Type Parameters

`T1`<br/>

`T2`<br/>

`T3`<br/>

`T4`<br/>

#### Parameters

`lambdaExpr` Expression\<Action\<T1, T2, T3, T4\>\><br/>

`ifFastFailedReturnNull` [Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

`flags` [CompilerFlags](../masstransit-internals/compilerflags)<br/>

#### Returns

[Action\<T1, T2, T3, T4\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-4)<br/>

### **CompileFast\<T1, T2, T3, T4, T5\>(Expression\<Action\<T1, T2, T3, T4, T5\>\>, Boolean, CompilerFlags)**

Compiles lambda expression to delegate. Use ifFastFailedReturnNull parameter to Not fallback to Expression.Compile, useful for testing.

```csharp
public static Action<T1, T2, T3, T4, T5> CompileFast<T1, T2, T3, T4, T5>(Expression<Action<T1, T2, T3, T4, T5>> lambdaExpr, bool ifFastFailedReturnNull, CompilerFlags flags)
```

#### Type Parameters

`T1`<br/>

`T2`<br/>

`T3`<br/>

`T4`<br/>

`T5`<br/>

#### Parameters

`lambdaExpr` Expression\<Action\<T1, T2, T3, T4, T5\>\><br/>

`ifFastFailedReturnNull` [Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

`flags` [CompilerFlags](../masstransit-internals/compilerflags)<br/>

#### Returns

[Action\<T1, T2, T3, T4, T5\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-5)<br/>

### **CompileFast\<T1, T2, T3, T4, T5, T6\>(Expression\<Action\<T1, T2, T3, T4, T5, T6\>\>, Boolean, CompilerFlags)**

Compiles lambda expression to delegate. Use ifFastFailedReturnNull parameter to Not fallback to Expression.Compile, useful for testing.

```csharp
public static Action<T1, T2, T3, T4, T5, T6> CompileFast<T1, T2, T3, T4, T5, T6>(Expression<Action<T1, T2, T3, T4, T5, T6>> lambdaExpr, bool ifFastFailedReturnNull, CompilerFlags flags)
```

#### Type Parameters

`T1`<br/>

`T2`<br/>

`T3`<br/>

`T4`<br/>

`T5`<br/>

`T6`<br/>

#### Parameters

`lambdaExpr` Expression\<Action\<T1, T2, T3, T4, T5, T6\>\><br/>

`ifFastFailedReturnNull` [Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

`flags` [CompilerFlags](../masstransit-internals/compilerflags)<br/>

#### Returns

[Action\<T1, T2, T3, T4, T5, T6\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-6)<br/>

### **TryCompile\<TDelegate\>(LambdaExpression, CompilerFlags)**

Tries to compile lambda expression to

```csharp
public static TDelegate TryCompile<TDelegate>(LambdaExpression lambdaExpr, CompilerFlags flags)
```

#### Type Parameters

`TDelegate`<br/>

#### Parameters

`lambdaExpr` LambdaExpression<br/>

`flags` [CompilerFlags](../masstransit-internals/compilerflags)<br/>

#### Returns

TDelegate<br/>

### **TryCompileWithPreCreatedClosure\<TDelegate\>(LambdaExpression, ConstantExpression[])**

Tries to compile lambda expression to 
 with the provided closure object and constant expressions (or lack there of) -
 Constant expression should be the in order of Fields in closure object!
 Note 1: Use it on your own risk - FEC won't verify the expression is compile-able with passed closure, it is up to you!
 Note 2: The expression with NESTED LAMBDA IS NOT SUPPORTED!
 Note 3: `Label` and `GoTo` are not supported in this case, because they need first round to collect out-of-order labels

```csharp
public static TDelegate TryCompileWithPreCreatedClosure<TDelegate>(LambdaExpression lambdaExpr, ConstantExpression[] closureConstantsExprs)
```

#### Type Parameters

`TDelegate`<br/>

#### Parameters

`lambdaExpr` LambdaExpression<br/>

`closureConstantsExprs` ConstantExpression[]<br/>

#### Returns

TDelegate<br/>

### **TryCompileWithPreCreatedClosure\<TDelegate\>(LambdaExpression, ConstantExpression[], CompilerFlags)**

Tries to compile lambda expression to 
 with the provided closure object and constant expressions (or lack there of)

```csharp
public static TDelegate TryCompileWithPreCreatedClosure<TDelegate>(LambdaExpression lambdaExpr, ConstantExpression[] closureConstantsExprs, CompilerFlags flags)
```

#### Type Parameters

`TDelegate`<br/>

#### Parameters

`lambdaExpr` LambdaExpression<br/>

`closureConstantsExprs` ConstantExpression[]<br/>

`flags` [CompilerFlags](../masstransit-internals/compilerflags)<br/>

#### Returns

TDelegate<br/>

### **TryCompileWithPreCreatedClosure\<TDelegate\>(LambdaExpression, ClosureInfo, CompilerFlags)**

```csharp
internal static TDelegate TryCompileWithPreCreatedClosure<TDelegate>(LambdaExpression lambdaExpr, ref ClosureInfo closureInfo, CompilerFlags flags)
```

#### Type Parameters

`TDelegate`<br/>

#### Parameters

`lambdaExpr` LambdaExpression<br/>

`closureInfo` [ClosureInfo](../masstransit-internals/closureinfo)<br/>

`flags` [CompilerFlags](../masstransit-internals/compilerflags)<br/>

#### Returns

TDelegate<br/>

### **TryCompileWithoutClosure\<TDelegate\>(LambdaExpression, CompilerFlags)**

Tries to compile expression to "static" delegate, skipping the step of collecting the closure object.

```csharp
public static TDelegate TryCompileWithoutClosure<TDelegate>(LambdaExpression lambdaExpr, CompilerFlags flags)
```

#### Type Parameters

`TDelegate`<br/>

#### Parameters

`lambdaExpr` LambdaExpression<br/>

`flags` [CompilerFlags](../masstransit-internals/compilerflags)<br/>

#### Returns

TDelegate<br/>

### **TryCompileBoundToFirstClosureParam(Type, Expression, IReadOnlyList\<ParameterExpression\>, Type[], Type, CompilerFlags)**

```csharp
internal static object TryCompileBoundToFirstClosureParam(Type delegateType, Expression bodyExpr, IReadOnlyList<ParameterExpression> paramExprs, Type[] closurePlusParamTypes, Type returnType, CompilerFlags flags)
```

#### Parameters

`delegateType` [Type](https://learn.microsoft.com/en-us/dotnet/api/system.type)<br/>

`bodyExpr` Expression<br/>

`paramExprs` [IReadOnlyList\<ParameterExpression\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ireadonlylist-1)<br/>

`closurePlusParamTypes` [Type[]](https://learn.microsoft.com/en-us/dotnet/api/system.type)<br/>

`returnType` [Type](https://learn.microsoft.com/en-us/dotnet/api/system.type)<br/>

`flags` [CompilerFlags](../masstransit-internals/compilerflags)<br/>

#### Returns

[Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>

### **IsClosureBoundConstant(Object, Type)**

```csharp
public static bool IsClosureBoundConstant(object value, Type type)
```

#### Parameters

`value` [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>

`type` [Type](https://learn.microsoft.com/en-us/dotnet/api/system.type)<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **IgnoresResult(ParentFlags)**

```csharp
internal static bool IgnoresResult(ParentFlags parent)
```

#### Parameters

`parent` [ParentFlags](../masstransit-internals/parentflags)<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **EmitPopIfIgnoreResult(ILGenerator, ParentFlags)**

```csharp
internal static bool EmitPopIfIgnoreResult(ILGenerator il, ParentFlags parent)
```

#### Parameters

`il` [ILGenerator](https://learn.microsoft.com/en-us/dotnet/api/system.reflection.emit.ilgenerator)<br/>

`parent` [ParentFlags](../masstransit-internals/parentflags)<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>
