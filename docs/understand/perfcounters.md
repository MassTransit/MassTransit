# Performance counters

MassTransit has support for updating Windows performance counters. Chris has a post introducing them:
[Performance Counters Added to MassTransit][1].

## User permissions

The user running your mass transit enabled application will need access to update the performance counters.

```text
HKLM\SOFTWARE\Microsoft\Windows NT\CurrentVersion\Perflib
```

If MassTransit does not detect the performance counters it wishes to write to it will attempt to create them.
If the user credentials do not have administrative access likely they will not have the ability to create the
performance counters and errors will be logged.

## Windows installer

When deploying your mass transit enabled application it is possible to have Windows Installer create your performance counters for you. Below is Xml used by Wix 3.0 to define the Masstransit performance counters.

```xml
<Wix xmlns="http://schemas.microsoft.com/wix/2006/wi"
    xmlns:msmq="http://schemas.microsoft.com/wix/MsmqExtension"
    xmlns:util="http://schemas.microsoft.com/wix/UtilExtension">

...

<Component Id="masstransit_performance_counters" Guid="{E68DCC22-AD78-4bfe-A1F6-29AA189FD76C}">
    <util:PerformanceCategory Id="perfCategoryMassTransit" Name="MassTransit"
        Help="MassTransit Performance Counters" MultiInstance="yes">
    <util:PerformanceCounter Name="Consumer Threads"
        Help="The current number of threads processing messages."
        Type="numberOfItems32"/>
    <util:PerformanceCounter Name="Receive Threads"
        Help="The current number of threads receiving messages."
        Type="numberOfItems32"/>
    <util:PerformanceCounter Name="Received/s"
        Help="The number of messages received per second"
            Type="rateOfCountsPerSecond32"/>
    <util:PerformanceCounter Name="Published/s"
        Help="The number of messages published per second"
        Type="rateOfCountsPerSecond32"/>
    <util:PerformanceCounter Name="Sent/s"
        Help="The number of messages sent per second"
        Type="rateOfCountsPerSecond32"/>
    <util:PerformanceCounter Name="Messages Received"
        Help="The total number of message received."
        Type="numberOfItems32"/>
    <util:PerformanceCounter Name="Messages Published"
        Help="The total number of message published."
        Type="numberOfItems32"/>
    <util:PerformanceCounter Name="Messages Sent"
        Help="The total number of message sent."
        Type="numberOfItems32"/>
    <util:PerformanceCounter Name="Average Consumer Duration"
        Help="The average time a consumer spends processing a message."
        Type="averageCount64"/>
    <util:PerformanceCounter Name="Average Consumer Duration Base"
        Help="The average time a consumer spends processing a message."
        Type="averageBase"/>
    <util:PerformanceCounter Name="Average Receive Duration"
        Help="The average time to receive a message."
        Type="averageCount64"/>
    <util:PerformanceCounter Name="Average Receive Duration Base"
        Help="The average time to receive a message."
        Type="averageBase"/>
    <util:PerformanceCounter Name="Average Publish Duration"
        Help="The average time to publish a message."
        Type="averageCount64"/>
    <util:PerformanceCounter Name="Average Publish Duration Base"
        Help="The average time to publish a message."
        Type="averageBase"/>
    </util:PerformanceCategory>
</Component>
```

[1]: http://lostechies.com/chrispatterson/2009/10/14/performance-counters-added-to-masstransit/