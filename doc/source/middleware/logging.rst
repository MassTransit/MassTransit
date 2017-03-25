Logging in MassTransit
======================

.. attention:: **This page is obsolete!**

   New documentation is located at http://masstransit-project.com/MassTransit.

   The latest version of this page can be found here_.

.. _here: http://masstransit-project.com/MassTransit/usage/logging.html

Loggin in MassTransit is now done using an internal abstraction allowing you
to choose your preferred logging solution. Like NHibernate's 'NHibernate.SQL'
where all of NHibernate's generated sql is logged, MassTransit has a log named
'MassTransit.Messages' where all of the message traffic is logged.
This logging looks like:

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
