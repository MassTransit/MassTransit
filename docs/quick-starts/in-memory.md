---
prev: false
next: /usage/configuration
sidebarDepth: 0
---

# In Memory

> This tutorial will get you from zero to up and running with [In Memory](/usage/transports/in-memory) and MassTransit. 

<iframe id="ytplayer" type="text/html" width="640" height="360"
  src="https://www.youtube.com/embed/WjOX1DrzO-w?autoplay=0">
</iframe>

## Prerequisites

This example requires the following:

- a functioning installation of the dotnet runtime and sdk (at least 6.0)

### Install MassTransit Templates

MassTransit includes project and item [templates](/usage/templates) simplifying the creation of new projects. Install the templates by executing `dotnet new -i MassTransit.Templates` at the console. A video introducing the templates is available on [YouTube](https://youtu.be/nYKq61-DFBQ).

```
dotnet new install MassTransit.Templates
```

## Initial Project Creation

### Create the worker project

To create a service using MassTransit, create a worker via the Command Prompt.

```bash
$ dotnet new mtworker -n GettingStarted
$ cd GettingStarted
$ dotnet new mtconsumer
```

### Overview of the code

When you open the project you will see that you have 1 class file.

- `Program.cs` is the standard entry point and here we configure the host builder.

### Create a Contract
Create a `Contracts` folder in the root of your project, and within that folder create a file named `GettingStarted.cs` with the following contents:

``` cs
namespace GettingStarted.Contracts;

public record GettingStarted() 
{
    public string Value { get; init; }
}
```

### Add A BackgroundService

In the root of the project add `Worker.cs`

<<< @/docs/code/quickstart/Worker.cs

### Register Worker

In `Program.cs` at the bottom of the `ConfigureServices` method add

```csharp
services.AddHostedService<Worker>();
```

### Create a Consumer

Create a `Consumers` folder in the root of your project, and within that folder create a file named `GettingStartedConsumer.cs` with the following contents:

<<< @/docs/code/quickstart/GettingStartedConsumer.cs

### Run the project

```bash
$ dotnet run
```

The output should have changed to show the message consumer generating the output (again, press Control+C to exit).

``` {2-5,12-15}
Building...
info: MassTransit[0]
      Configured endpoint Message, Consumer: GettingStarted.MessageConsumer
info: MassTransit[0]
      Bus started: loopback://localhost/
info: Microsoft.Hosting.Lifetime[0]
      Application started. Press Ctrl+C to shut down.
info: Microsoft.Hosting.Lifetime[0]
      Hosting environment: Development
info: Microsoft.Hosting.Lifetime[0]
      Content root path: /Users/chris/Garbage/start/GettingStarted
info: GettingStarted.MessageConsumer[0]
      Received Text: The time is 3/24/2021 12:02:01 PM -05:00
info: GettingStarted.MessageConsumer[0]
      Received Text: The time is 3/24/2021 12:02:02 PM -05:00
```
