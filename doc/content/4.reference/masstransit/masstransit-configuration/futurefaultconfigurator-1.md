---

title: FutureFaultConfigurator<TFault>

---

# FutureFaultConfigurator\<TFault\>

Namespace: MassTransit.Configuration

```csharp
public class FutureFaultConfigurator<TFault> : IFutureFaultConfigurator<TFault>
```

#### Type Parameters

`TFault`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [FutureFaultConfigurator\<TFault\>](../masstransit-configuration/futurefaultconfigurator-1)<br/>
Implements [IFutureFaultConfigurator\<TFault\>](../masstransit/ifuturefaultconfigurator-1)

## Constructors

### **FutureFaultConfigurator(FutureFault\<TFault\>)**

```csharp
public FutureFaultConfigurator(FutureFault<TFault> fault)
```

#### Parameters

`fault` [FutureFault\<TFault\>](../masstransit-futures/futurefault-1)<br/>

## Methods

### **SetFaultedUsingFactory(EventMessageFactory\<FutureState, TFault\>)**

```csharp
public void SetFaultedUsingFactory(EventMessageFactory<FutureState, TFault> factoryMethod)
```

#### Parameters

`factoryMethod` [EventMessageFactory\<FutureState, TFault\>](../../masstransit-abstractions/masstransit/eventmessagefactory-2)<br/>

### **SetFaultedUsingFactory(AsyncEventMessageFactory\<FutureState, TFault\>)**

```csharp
public void SetFaultedUsingFactory(AsyncEventMessageFactory<FutureState, TFault> factoryMethod)
```

#### Parameters

`factoryMethod` [AsyncEventMessageFactory\<FutureState, TFault\>](../../masstransit-abstractions/masstransit/asynceventmessagefactory-2)<br/>

### **SetFaultedUsingInitializer(InitializerValueProvider)**

```csharp
public void SetFaultedUsingInitializer(InitializerValueProvider valueProvider)
```

#### Parameters

`valueProvider` [InitializerValueProvider](../masstransit/initializervalueprovider)<br/>
