---

title: FutureFaultConfigurator<TCommand, TFault, TInput>

---

# FutureFaultConfigurator\<TCommand, TFault, TInput\>

Namespace: MassTransit.Configuration

```csharp
public class FutureFaultConfigurator<TCommand, TFault, TInput> : IFutureFaultConfigurator<TFault, TInput>
```

#### Type Parameters

`TCommand`<br/>

`TFault`<br/>

`TInput`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [FutureFaultConfigurator\<TCommand, TFault, TInput\>](../masstransit-configuration/futurefaultconfigurator-3)<br/>
Implements [IFutureFaultConfigurator\<TFault, TInput\>](../masstransit/ifuturefaultconfigurator-2)

## Constructors

### **FutureFaultConfigurator(FutureFault\<TCommand, TFault, TInput\>)**

```csharp
public FutureFaultConfigurator(FutureFault<TCommand, TFault, TInput> fault)
```

#### Parameters

`fault` [FutureFault\<TCommand, TFault, TInput\>](../masstransit-futures/futurefault-3)<br/>

## Methods

### **SetFaultedUsingFactory(EventMessageFactory\<FutureState, TInput, TFault\>)**

```csharp
public void SetFaultedUsingFactory(EventMessageFactory<FutureState, TInput, TFault> factoryMethod)
```

#### Parameters

`factoryMethod` [EventMessageFactory\<FutureState, TInput, TFault\>](../../masstransit-abstractions/masstransit/eventmessagefactory-3)<br/>

### **SetFaultedUsingFactory(AsyncEventMessageFactory\<FutureState, TInput, TFault\>)**

```csharp
public void SetFaultedUsingFactory(AsyncEventMessageFactory<FutureState, TInput, TFault> factoryMethod)
```

#### Parameters

`factoryMethod` [AsyncEventMessageFactory\<FutureState, TInput, TFault\>](../../masstransit-abstractions/masstransit/asynceventmessagefactory-3)<br/>

### **SetFaultedUsingInitializer(InitializerValueProvider\<TInput\>)**

```csharp
public void SetFaultedUsingInitializer(InitializerValueProvider<TInput> valueProvider)
```

#### Parameters

`valueProvider` [InitializerValueProvider\<TInput\>](../masstransit/initializervalueprovider-1)<br/>
