---

title: MessageDataDefaults

---

# MessageDataDefaults

Namespace: MassTransit

```csharp
public static class MessageDataDefaults
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [MessageDataDefaults](../masstransit/messagedatadefaults)

## Properties

### **AlwaysWriteToRepository**

Transitional, will always write to the repository but will include inline to avoid reading on
 current framework clients. If all services are upgraded, set to false so that data sizes below
 the threshold are not written to the repository.

```csharp
public static bool AlwaysWriteToRepository { get; set; }
```

#### Property Value

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **Threshold**

Set the threshold for automatic message data to be written to the repository, vs stored inline.

```csharp
public static int Threshold { get; set; }
```

#### Property Value

[Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>

### **TimeToLive**

Set the default time to live for message data when no expiration is specified

```csharp
public static Nullable<TimeSpan> TimeToLive { get; set; }
```

#### Property Value

[Nullable\<TimeSpan\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

### **ExtraTimeToLive**

Set an extra time to live for message data, which is added to inferred expiration based upon
 SendContext TimeToLive.

```csharp
public static Nullable<TimeSpan> ExtraTimeToLive { get; set; }
```

#### Property Value

[Nullable\<TimeSpan\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>
