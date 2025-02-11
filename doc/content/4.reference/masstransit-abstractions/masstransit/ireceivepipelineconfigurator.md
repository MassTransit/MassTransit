---

title: IReceivePipelineConfigurator

---

# IReceivePipelineConfigurator

Namespace: MassTransit

```csharp
public interface IReceivePipelineConfigurator
```

## Methods

### **ConfigureReceive(Action\<IReceivePipeConfigurator\>)**

Configure the Receive pipeline

```csharp
void ConfigureReceive(Action<IReceivePipeConfigurator> callback)
```

#### Parameters

`callback` [Action\<IReceivePipeConfigurator\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

### **ConfigureDeadLetter(Action\<IPipeConfigurator\<ReceiveContext\>\>)**

Configure the dead letter pipeline, which is called if the message is not consumed

```csharp
void ConfigureDeadLetter(Action<IPipeConfigurator<ReceiveContext>> callback)
```

#### Parameters

`callback` [Action\<IPipeConfigurator\<ReceiveContext\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

### **ConfigureError(Action\<IPipeConfigurator\<ExceptionReceiveContext\>\>)**

Configure the exception pipeline, which is called if there are uncaught consumer exceptions

```csharp
void ConfigureError(Action<IPipeConfigurator<ExceptionReceiveContext>> callback)
```

#### Parameters

`callback` [Action\<IPipeConfigurator\<ExceptionReceiveContext\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

### **ConfigureTransport(Action\<ITransportConfigurator\>)**

Configure the transport options

```csharp
void ConfigureTransport(Action<ITransportConfigurator> callback)
```

#### Parameters

`callback` [Action\<ITransportConfigurator\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>
