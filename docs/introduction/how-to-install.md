# How to install

## NuGet

The simplest way to install MassTransit into your solution/project is to use NuGet.

```
nuget nstall-Package MassTransit
```

However, the NuGet packages don’t contain the MassTransit.RuntimeServices executable and database SQL scripts. The RuntimeServices system routes messages to multiple subscribers via the Subscription Service. If you plan to use the “UseSubscriptionService” feature, then you’ll need to get compile that from source.

### Then you will need to add references to

* MassTransit.dll
* MassTransit.&lt;Transport&gt;.dll \(RabbitMQ and Azure Service Bus\)
* *Optionally* MassTransit.&lt;ContainerSupport&gt;.dll \(Castle, AutoFac, and StructureMap\)

## Compiling from source

Lastly, if you want to hack on MassTransit or just want to have the actual source code you can clone the source from github.com.

To clone the repository using git try the following:

```
git clone git://github.com/MassTransit/MassTransit.git

```

**Note:** The default branch for this project is develop. This is done to make development easier. The master branch in this case represents gold code.

## Build dependencies

To compile MassTransit from source you will need the following developer tools installed:

 - .NET 4.5 SDK or later

## Compiling

To compile the source code, drop to the command line and type:

```
.\build.bat

```

If you look in the`.\build_output`folder you should see the binaries.

