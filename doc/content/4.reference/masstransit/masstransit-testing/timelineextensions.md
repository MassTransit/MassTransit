---

title: TimelineExtensions

---

# TimelineExtensions

Namespace: MassTransit.Testing

```csharp
public static class TimelineExtensions
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [TimelineExtensions](../masstransit-testing/timelineextensions)

## Methods

### **OutputTimeline(IBaseTestHarness, TextWriter, Action\<OutputTimelineOptions\>)**

Output a timeline of messages published, sent, and consumed by the test harness.

```csharp
public static Task OutputTimeline(IBaseTestHarness harness, TextWriter textWriter, Action<OutputTimelineOptions> configure)
```

#### Parameters

`harness` [IBaseTestHarness](../masstransit-testing/ibasetestharness)<br/>

`textWriter` [TextWriter](https://learn.microsoft.com/en-us/dotnet/api/system.io.textwriter)<br/>

`configure` [Action\<OutputTimelineOptions\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>
Configure the timeout output options

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

#### Exceptions

[ArgumentNullException](https://learn.microsoft.com/en-us/dotnet/api/system.argumentnullexception)<br/>
