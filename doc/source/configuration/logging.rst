Logging in MassTransit
======================

Logging in MassTransit is done with the de facto logging tool 'log4net.' This tool was chosen for its many years of battle hardened code and ease of use. You can find out more about log4net and its configuration at http://logging.apache.org/log4net

Like NHibernate's 'NHibernate.SQL' where all of NHibernate's generated sql is logged, MassTransit has a log named 'MassTransit.Messages' where all of the message traffic is logged. This logging looks like:

    RECV:{Address}:{Message Id}:{Message Type Name}
    SEND:{Address}:{Message Name}


Logging with MassTransit.Log4Net
''''''''''''''''''''''''''''''''''''''

First you need to get the latest MassTransit.Log4Net from NuGet or download it
from https://nuget.org/packages/MassTransit.Log4Net.

Then add logging to your service bus initialization.

.. sourcecode:: csharp


    using MassTransit.Log4NetIntegration;


    XmlConfigurator.Configure();

    ServiceBus = ServiceBusFactory.New(sbc =>
        {
            /* usual stuff */
            sbc.UseLog4Net();
        });

The easiest way to configure logging is through the web.config or app.config.

.. sourcecode:: xml

  <configSections>
    <section name="log4net" type="log4net.Config.Log4NetConfigurationSectionHandler, log4net" />
  </configSections>
  <log4net>
    <appender name="RollingFile" type="log4net.Appender.RollingFileAppender">
      <file value="MassTransit.log" />
      <appendToFile value="true" />
      <maximumFileSize value="1000KB" />
      <maxSizeRollBackups value="5" />

      <layout type="log4net.Layout.PatternLayout">
        <conversionPattern value="%level %thread %logger - %message%newline" />
      </layout>
    </appender>

    <root>
      <level value="DEBUG" />
      <appender-ref ref="RollingFile" />
    </root>
  </log4net>

This will log to MassTransit.log in the root folder. There are a lot more configuration
options explained at http://logging.apache.org/log4net/release/manual/configuration.html.

Logging with MassTransit.NLog
'''''''''''''''''''''''''''''

First you need to get the latest MassTransit.NLog for NuGet or download it
from https://nuget.org/packages/MassTransit.NLog

Then add logging to your service bus initialization

.. sourcecode:: csharp
    using MassTransit.NLogIntegration;

    //configure NLog

    ServiceBus = ServiceBusFactory.New(sbc =>
    {
      /* usual stuff */
      sbc.UseNLog();
    });

The easiest way to configure logging is through ???? (thoughs for the NLog community?)
