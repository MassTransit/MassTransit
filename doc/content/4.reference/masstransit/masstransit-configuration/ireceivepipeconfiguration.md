---

title: IReceivePipeConfiguration

---

# IReceivePipeConfiguration

Namespace: MassTransit.Configuration

```csharp
public interface IReceivePipeConfiguration
```

## Properties

### **Specification**

```csharp
public abstract ISpecification Specification { get; }
```

#### Property Value

[ISpecification](../../masstransit-abstractions/masstransit/ispecification)<br/>

### **Configurator**

```csharp
public abstract IReceivePipeConfigurator Configurator { get; }
```

#### Property Value

[IReceivePipeConfigurator](../../masstransit-abstractions/masstransit/ireceivepipeconfigurator)<br/>

### **DeadLetterConfigurator**

```csharp
public abstract IBuildPipeConfigurator<ReceiveContext> DeadLetterConfigurator { get; }
```

#### Property Value

[IBuildPipeConfigurator\<ReceiveContext\>](../../masstransit-abstractions/masstransit-configuration/ibuildpipeconfigurator-1)<br/>

### **ErrorConfigurator**

```csharp
public abstract IBuildPipeConfigurator<ExceptionReceiveContext> ErrorConfigurator { get; }
```

#### Property Value

[IBuildPipeConfigurator\<ExceptionReceiveContext\>](../../masstransit-abstractions/masstransit-configuration/ibuildpipeconfigurator-1)<br/>

## Methods

### **CreatePipe(IConsumePipe, ISerialization)**

```csharp
IReceivePipe CreatePipe(IConsumePipe consumePipe, ISerialization serializers)
```

#### Parameters

`consumePipe` [IConsumePipe](../../masstransit-abstractions/masstransit-transports/iconsumepipe)<br/>

`serializers` [ISerialization](../../masstransit-abstractions/masstransit/iserialization)<br/>

#### Returns

[IReceivePipe](../../masstransit-abstractions/masstransit-transports/ireceivepipe)<br/>
