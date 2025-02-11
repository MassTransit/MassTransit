---

title: InstrumentationOptions

---

# InstrumentationOptions

Namespace: MassTransit.Monitoring

```csharp
public class InstrumentationOptions
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [InstrumentationOptions](../masstransit-monitoring/instrumentationoptions)

## Fields

### **MeterName**

```csharp
public static string MeterName;
```

## Properties

### **ServiceName**

```csharp
public string ServiceName { get; set; }
```

#### Property Value

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

### **EndpointLabel**

```csharp
public string EndpointLabel { get; set; }
```

#### Property Value

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

### **ConsumerTypeLabel**

```csharp
public string ConsumerTypeLabel { get; set; }
```

#### Property Value

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

### **ExceptionTypeLabel**

```csharp
public string ExceptionTypeLabel { get; set; }
```

#### Property Value

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

### **MessageTypeLabel**

```csharp
public string MessageTypeLabel { get; set; }
```

#### Property Value

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

### **ActivityNameLabel**

```csharp
public string ActivityNameLabel { get; set; }
```

#### Property Value

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

### **ArgumentTypeLabel**

```csharp
public string ArgumentTypeLabel { get; set; }
```

#### Property Value

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

### **LogTypeLabel**

```csharp
public string LogTypeLabel { get; set; }
```

#### Property Value

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

### **ServiceNameLabel**

```csharp
public string ServiceNameLabel { get; set; }
```

#### Property Value

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

### **ReceiveTotal**

```csharp
public string ReceiveTotal { get; set; }
```

#### Property Value

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

### **ReceiveFaultTotal**

```csharp
public string ReceiveFaultTotal { get; set; }
```

#### Property Value

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

### **ReceiveDuration**

```csharp
public string ReceiveDuration { get; set; }
```

#### Property Value

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

### **ReceiveInProgress**

```csharp
public string ReceiveInProgress { get; set; }
```

#### Property Value

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

### **ConsumeTotal**

```csharp
public string ConsumeTotal { get; set; }
```

#### Property Value

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

### **ConsumeFaultTotal**

```csharp
public string ConsumeFaultTotal { get; set; }
```

#### Property Value

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

### **ConsumeRetryTotal**

```csharp
public string ConsumeRetryTotal { get; set; }
```

#### Property Value

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

### **SagaTotal**

```csharp
public string SagaTotal { get; set; }
```

#### Property Value

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

### **SagaFaultTotal**

```csharp
public string SagaFaultTotal { get; set; }
```

#### Property Value

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

### **SagaDuration**

```csharp
public string SagaDuration { get; set; }
```

#### Property Value

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

### **HandlerTotal**

```csharp
public string HandlerTotal { get; set; }
```

#### Property Value

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

### **HandlerFaultTotal**

```csharp
public string HandlerFaultTotal { get; set; }
```

#### Property Value

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

### **HandlerDuration**

```csharp
public string HandlerDuration { get; set; }
```

#### Property Value

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

### **PublishTotal**

#### Caution

This member is obsolete.

---

```csharp
public string PublishTotal { get; set; }
```

#### Property Value

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

### **PublishFaultTotal**

#### Caution

This member is obsolete.

---

```csharp
public string PublishFaultTotal { get; set; }
```

#### Property Value

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

### **SendTotal**

```csharp
public string SendTotal { get; set; }
```

#### Property Value

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

### **SendFaultTotal**

```csharp
public string SendFaultTotal { get; set; }
```

#### Property Value

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

### **ActivityExecuteTotal**

```csharp
public string ActivityExecuteTotal { get; set; }
```

#### Property Value

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

### **ActivityExecuteFaultTotal**

```csharp
public string ActivityExecuteFaultTotal { get; set; }
```

#### Property Value

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

### **ActivityExecuteDuration**

```csharp
public string ActivityExecuteDuration { get; set; }
```

#### Property Value

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

### **ActivityCompensateTotal**

```csharp
public string ActivityCompensateTotal { get; set; }
```

#### Property Value

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

### **ActivityCompensateFailureTotal**

```csharp
public string ActivityCompensateFailureTotal { get; set; }
```

#### Property Value

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

### **ActivityCompensateDuration**

```csharp
public string ActivityCompensateDuration { get; set; }
```

#### Property Value

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

### **BusInstances**

```csharp
public string BusInstances { get; set; }
```

#### Property Value

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

### **EndpointInstances**

```csharp
public string EndpointInstances { get; set; }
```

#### Property Value

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

### **ConsumerInProgress**

```csharp
public string ConsumerInProgress { get; set; }
```

#### Property Value

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

### **HandlerInProgress**

```csharp
public string HandlerInProgress { get; set; }
```

#### Property Value

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

### **SagaInProgress**

```csharp
public string SagaInProgress { get; set; }
```

#### Property Value

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

### **ExecuteInProgress**

```csharp
public string ExecuteInProgress { get; set; }
```

#### Property Value

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

### **CompensateInProgress**

```csharp
public string CompensateInProgress { get; set; }
```

#### Property Value

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

### **ConsumeDuration**

```csharp
public string ConsumeDuration { get; set; }
```

#### Property Value

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

### **DeliveryDuration**

```csharp
public string DeliveryDuration { get; set; }
```

#### Property Value

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

### **OutboxSendTotal**

```csharp
public string OutboxSendTotal { get; set; }
```

#### Property Value

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

### **OutboxSendFaultTotal**

```csharp
public string OutboxSendFaultTotal { get; set; }
```

#### Property Value

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

### **OutboxDeliveryTotal**

```csharp
public string OutboxDeliveryTotal { get; set; }
```

#### Property Value

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

### **OutboxDeliveryFaultTotal**

```csharp
public string OutboxDeliveryFaultTotal { get; set; }
```

#### Property Value

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

## Constructors

### **InstrumentationOptions()**

```csharp
public InstrumentationOptions()
```
