---

title: Activation

---

# Activation

Namespace: MassTransit.Metadata

```csharp
public static class Activation
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [Activation](../masstransit-metadata/activation)

## Methods

### **Activate\<TResult\>(Type, IActivationType\<TResult\>)**

```csharp
public static TResult Activate<TResult>(Type type, IActivationType<TResult> activationType)
```

#### Type Parameters

`TResult`<br/>

#### Parameters

`type` [Type](https://learn.microsoft.com/en-us/dotnet/api/system.type)<br/>

`activationType` [IActivationType\<TResult\>](../masstransit-metadata/iactivationtype-1)<br/>

#### Returns

TResult<br/>

### **Activate\<TResult, T1\>(Type, IActivationType\<TResult, T1\>, T1)**

```csharp
public static TResult Activate<TResult, T1>(Type type, IActivationType<TResult, T1> activationType, T1 arg1)
```

#### Type Parameters

`TResult`<br/>

`T1`<br/>

#### Parameters

`type` [Type](https://learn.microsoft.com/en-us/dotnet/api/system.type)<br/>

`activationType` [IActivationType\<TResult, T1\>](../masstransit-metadata/iactivationtype-2)<br/>

`arg1` T1<br/>

#### Returns

TResult<br/>

### **Activate\<TResult, T1, T2\>(Type, IActivationType\<TResult, T1, T2\>, T1, T2)**

```csharp
public static TResult Activate<TResult, T1, T2>(Type type, IActivationType<TResult, T1, T2> activationType, T1 arg1, T2 arg2)
```

#### Type Parameters

`TResult`<br/>

`T1`<br/>

`T2`<br/>

#### Parameters

`type` [Type](https://learn.microsoft.com/en-us/dotnet/api/system.type)<br/>

`activationType` [IActivationType\<TResult, T1, T2\>](../masstransit-metadata/iactivationtype-3)<br/>

`arg1` T1<br/>

`arg2` T2<br/>

#### Returns

TResult<br/>
