# How to install

## NuGet

The simplest way to install MassTransit into your solution/project is to use NuGet.

```
nuget Install-Package MassTransit
```

Then, depending upon your transport, you could install one of the following transports.

RabbitMQ:
```
nuget Install-Package MassTransit.RabbitMQ
```

Azure Service Bus:
```
nuget Install-Package MassTransit.AzureServiceBus
```

## Compiling from source

Lastly, if you want to hack on MassTransit or just want to have the actual source code you can clone the source from github.com.

To clone the repository using git try the following:

```
git clone https://github.com/MassTransit/MassTransit.git

```

**Note:** The default branch for this project is develop. This is done to make development easier. The master branch in this case represents gold code.

## Build dependencies

To compile MassTransit from source you will need the following developer tools installed:

- .NET 4.5.2 SDK or later

## Compiling

To compile the source code, drop to the command line and type:

```
.\build.bat
```
