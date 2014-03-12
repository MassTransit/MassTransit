Mass Transit - A Service Bus Implementation for .NET
=======

![Mass Transit](http://www.phatboyg.com/mt-logo.png "Mass Transit")

MassTransit is lean service bus implementation for building loosely coupled applications using the .NET Framework.

MassTransit is Apache 2.0 licensed.

## Getting started with Mass Transit

In order to get started with MassTransit, you can have a look at the documentation, which is located at [http://docs.masstransit-project.com/](http://docs.masstransit-project.com/).

### Simplest possible thing:

`install-package MassTransit.RabbitMq` then;

```
ServiceBusFactory.New(sbc =>
{
	sbc.UseRabbitMq();
	sbc.ReceiveFrom("rabbitmq://localhost/mybus");
});
```

You will also need to set up RabbitMQ; 

 1. **Install Erlang** using the [installer](http://www.erlang.org/download.html). (Next -> Next ...)
 2. **Install RabbitMQ** using the [installer](http://www.rabbitmq.com/download.html). (Next -> Next ...) You now have a RabbitMQ broker (look in `services.msc` for it) that you can [log into](http://localhost:55672/#/) using `guest`, `guest`. You can see message rates, routings and active consumers using this interface. 
 
**Please note** 

You need to add the management interface before you can login. First, from an elevated command prompt, change directory to the sbin folder within the RabbitMQ Server installation directory e.g. `%PROGRAMFILES%\RabbitMQ Server\rabbitmq_server_2.7.1\sbin\`

Next, run the following command to enable the rabbitmq management plugin:

	rabbitmq-plugins.bat enable rabbitmq_management 

### Downloads

Download from NuGet 'MassTransit' [Search NuGet for MassTransit](http://nuget.org/packages?q=masstransit)

Download the continuously integrated Binaries from [TeamCity](http://teamcity.codebetter.com/viewType.html?buildTypeId=bt8&tab=buildTypeStatusDiv).

### Supported Transports

We support [MSMQ](http://readthedocs.org/docs/masstransit/en/latest/configuration/quickstart.html) if you already have that installed, [Azure Service Bus](https://github.com/mpsbroadband/MassTransit-AzureServiceBus) and [Stomp](https://github.com/enix/MassTransit-Stomp) transports. 

If you want to use ZeroMQ, have a look at that branch and consider adding to it. It may make an appearance in v 3.0.

## Mailing List

[MassTransit Discuss](http://groups.google.com/group/masstransit-discuss)

## Building from Source

 1. Clone the source down to your machine. 
   `git clone git://github.com/MassTransit/MassTransit.git`
 1. Ensure Ruby is installed. [RubyInstaller for Windows](http://rubyinstaller.org/)
 1. Ensure `git` is on your path. `git.exe` and `git.cmd` work equally well.
 1. **Ensure gems are installed**, run:

```
gem install zip-zip 
gem install albacore
gem install semver2
```

4. Run `build.bat`

## Contributing

 1. `git config --global core.autocrlf false`
 1. Hack!
 1. Make a pull request.
 
# REQUIREMENTS
* .Net 3.5
* .Net 4.0

# CREDITS
Logo Design by [The Agile Badger](http://www.theagilebadger.com)
