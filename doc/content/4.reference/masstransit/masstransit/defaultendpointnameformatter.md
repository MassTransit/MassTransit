---

title: DefaultEndpointNameFormatter

---

# DefaultEndpointNameFormatter

Namespace: MassTransit

The default endpoint name formatter, which simply trims the words Consumer, Activity, and Saga
 from the type name. If you need something more readable, consider the [SnakeCaseEndpointNameFormatter](../masstransit/snakecaseendpointnameformatter)
 or the [KebabCaseEndpointNameFormatter](../masstransit/kebabcaseendpointnameformatter).

```csharp
public class DefaultEndpointNameFormatter : IEndpointNameFormatter
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [DefaultEndpointNameFormatter](../masstransit/defaultendpointnameformatter)<br/>
Implements [IEndpointNameFormatter](../../masstransit-abstractions/masstransit/iendpointnameformatter)

## Properties

### **Instance**

```csharp
public static IEndpointNameFormatter Instance { get; }
```

#### Property Value

[IEndpointNameFormatter](../../masstransit-abstractions/masstransit/iendpointnameformatter)<br/>

### **Separator**

```csharp
public string Separator { get; protected set; }
```

#### Property Value

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

## Constructors

### **DefaultEndpointNameFormatter(Boolean)**

Default endpoint name formatter.

```csharp
public DefaultEndpointNameFormatter(bool includeNamespace)
```

#### Parameters

`includeNamespace` [Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>
If true, the namespace is included in the name

### **DefaultEndpointNameFormatter(String, Boolean)**

Default endpoint name formatter with prefix.

```csharp
public DefaultEndpointNameFormatter(string prefix, bool includeNamespace)
```

#### Parameters

`prefix` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>
Prefix to start the name, should match the casing of the formatter (such as Dev or PreProd)

`includeNamespace` [Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>
If true, the namespace is included in the name

### **DefaultEndpointNameFormatter(String)**

Default endpoint name formatter with prefix.

```csharp
public DefaultEndpointNameFormatter(string prefix)
```

#### Parameters

`prefix` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>
Prefix to start the name, should match the casing of the formatter (such as Dev or PreProd)

### **DefaultEndpointNameFormatter(String, String, Boolean)**

Default endpoint name formatter with join separator and prefix.

```csharp
public DefaultEndpointNameFormatter(string joinSeparator, string prefix, bool includeNamespace)
```

#### Parameters

`joinSeparator` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>
Define the join separator between the words

`prefix` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>
Prefix to start the name, should match the casing of the formatter (such as Dev or PreProd)

`includeNamespace` [Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>
If true, the namespace is included in the name

## Methods

### **TemporaryEndpoint(String)**

```csharp
public string TemporaryEndpoint(string tag)
```

#### Parameters

`tag` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

#### Returns

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

### **Consumer\<T\>()**

```csharp
public string Consumer<T>()
```

#### Type Parameters

`T`<br/>

#### Returns

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

### **Message\<T\>()**

```csharp
public string Message<T>()
```

#### Type Parameters

`T`<br/>

#### Returns

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

### **Saga\<T\>()**

```csharp
public string Saga<T>()
```

#### Type Parameters

`T`<br/>

#### Returns

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

### **ExecuteActivity\<T, TArguments\>()**

```csharp
public string ExecuteActivity<T, TArguments>()
```

#### Type Parameters

`T`<br/>

`TArguments`<br/>

#### Returns

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

### **CompensateActivity\<T, TLog\>()**

```csharp
public string CompensateActivity<T, TLog>()
```

#### Type Parameters

`T`<br/>

`TLog`<br/>

#### Returns

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

### **SanitizeName(String)**

```csharp
public string SanitizeName(string name)
```

#### Parameters

`name` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

#### Returns

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

### **GetTemporaryQueueName(String)**

```csharp
public static string GetTemporaryQueueName(string tag)
```

#### Parameters

`tag` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

#### Returns

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

### **GetConsumerName(Type)**

Gets the endpoint name for a consumer of the given type.

```csharp
protected string GetConsumerName(Type type)
```

#### Parameters

`type` [Type](https://learn.microsoft.com/en-us/dotnet/api/system.type)<br/>
The type of the consumer implementing

#### Returns

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>
The fully formatted name as it will be provided via [DefaultEndpointNameFormatter.Consumer\<T\>()](defaultendpointnameformatter#consumert)

### **GetMessageName(Type)**

Gets the endpoint name for a message of the given type.

```csharp
protected string GetMessageName(Type type)
```

#### Parameters

`type` [Type](https://learn.microsoft.com/en-us/dotnet/api/system.type)<br/>
The type of the message

#### Returns

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>
The fully formatted name as it will be provided via [DefaultEndpointNameFormatter.Message\<T\>()](defaultendpointnameformatter#messaget)

### **GetSagaName(Type)**

Gets the endpoint name for a saga of the given type.

```csharp
protected string GetSagaName(Type type)
```

#### Parameters

`type` [Type](https://learn.microsoft.com/en-us/dotnet/api/system.type)<br/>
The type of the saga implementing

#### Returns

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>
The fully formatted name as it will be provided via [DefaultEndpointNameFormatter.Saga\<T\>()](defaultendpointnameformatter#sagat)

### **GetActivityName(Type, Type)**

Gets the name for an activity of the given type.

```csharp
protected string GetActivityName(Type activityType, Type argumentType)
```

#### Parameters

`activityType` [Type](https://learn.microsoft.com/en-us/dotnet/api/system.type)<br/>
The type of the activity implementing

`argumentType` [Type](https://learn.microsoft.com/en-us/dotnet/api/system.type)<br/>
For execution endpoints this is the activity arguments, for compensation this is the log type.

#### Returns

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>
The formatted activity name further used in [DefaultEndpointNameFormatter.ExecuteActivity\<T, TArguments\>()](defaultendpointnameformatter#executeactivityt-targuments) and [DefaultEndpointNameFormatter.CompensateActivity\<T, TLog\>()](defaultendpointnameformatter#compensateactivityt-tlog).

**Remarks:**

The activity name is used both for execution and compensation endpoint names.

### **FormatName(Type)**

Does a basic formatting of the type respecting settings like .

```csharp
protected string FormatName(Type type)
```

#### Parameters

`type` [Type](https://learn.microsoft.com/en-us/dotnet/api/system.type)<br/>
The type to format.

#### Returns

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>
A formatted type name, not yet sanitized via [DefaultEndpointNameFormatter.SanitizeName(String)](defaultendpointnameformatter#sanitizenamestring).
