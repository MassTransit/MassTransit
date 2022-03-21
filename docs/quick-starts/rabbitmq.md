---
prev: false
next: /usage/configuration
sidebarDepth: 0
---

# RabbitMQ

> This tutorial will get you from zero to up and running with [RabbitMQ](/usage/transports/rabbitmq) and MassTransit. 

<iframe id="ytplayer" type="text/html" width="640" height="360"
  src="https://www.youtube.com/embed/_dfEMm7rRrI?autoplay=0">
</iframe>

- The source for this sample is available [on GitHub](https://github.com/MassTransit/Sample-GettingStarted).

## Prerequisites

::: tip NOTE
The following instructions assume you are starting from a completed [In-Memory Quick Start](/quick-starts/in-memory)
:::

This example requires the following:

- a functioning installation of the dotnet runtime and sdk (at least 6.0)
- a functioning installation of `docker` with `docker compose` support

## Get RabbitMQ up and running

For this quick start we recommend running the preconfigured [Docker image maintained by the MassTransit team](https://github.com/MassTransit/docker-rabbitmq). It includes the delayed exchange plug-in, as well as the Management interface enabled.

```bash
$ docker run -p 15672:15672 -p 5672:5672 masstransit/rabbitmq
```

If you are running on an ARM platform

```bash
$ docker run --platform linux/arm64 -p 15672:15672 -p 5672:5672 masstransit/rabbitmq
```

Once its up and running you can [log into](http://localhost:15672) the broker using `guest`, `guest`. You can see message rates, routings and active consumers using this interface. 


## Change the Transport to RabbitMQ

Add the _MassTransit.RabbitMQ_ package to the project.

```bash
$ dotnet add package MassTransit.RabbitMQ
```

## Edit Program.cs

Change `UsingInMemory` to `UsingRabbitMq`

```csharp {9-17}
public static IHostBuilder CreateHostBuilder(string[] args) =>
    Host.CreateDefaultBuilder(args)
        .ConfigureServices((hostContext, services) =>
        {
            services.AddMassTransit(x =>
            {
                // elided...

                x.UsingRabbitMq((context,cfg) =>
                {
                    cfg.Host("localhost", "/", h => {
                        h.Username("guest");
                        h.Password("guest");
                    });

                    cfg.ConfigureEndpoints(context);
                });
            });

            services.AddHostedService<Worker>();
        });
```

`localhost` is where the docker image is running. We are inferring the default port of `5672` and are using `\` as the [virtual host](https://www.rabbitmq.com/vhosts.html). `guest` and `guest` are the default username and password to talk to the cluster and [management dashboard](http://localhost:15672).

## Run the project

```bash
$ dotnet run
```

The output should have changed to show the message consumer generating the output (again, press Control+C to exit). Notice that the bus address now starts with `rabbitmq`.

``` {11}
Building...
info: MassTransit[0]
      Configured endpoint Message, Consumer: GettingStarted.MessageConsumer
info: Microsoft.Hosting.Lifetime[0]
      Application started. Press Ctrl+C to shut down.
info: Microsoft.Hosting.Lifetime[0]
      Hosting environment: Development
info: Microsoft.Hosting.Lifetime[0]
      Content root path: /Users/chris/Garbage/start/GettingStarted
info: MassTransit[0]
      Bus started: rabbitmq://localhost/
info: GettingStarted.MessageConsumer[0]
      Received Text: The time is 3/24/2021 12:11:10 PM -05:00
```

At this point the service is connecting to RabbbitMQ on `localhost` and publishing messages which are received by the consumer.

:tada: