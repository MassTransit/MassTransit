# Application Insights

Application Insights (part of Azure Monitor) is able to capture and record metrics from MassTransit. It can also be configured as a log sink for logging.

[Create an Application Insights resource](https://docs.microsoft.com/en-us/azure/application-insights/app-insights-create-new-resource#create-an-application-insights-resource-1)

[Copy the instrumentation key](https://docs.microsoft.com/en-us/azure/application-insights/app-insights-create-new-resource#copy-the-instrumentation-key)

To configure an application to use Application Insights with MassTransit:

> Requires NuGets `MassTransit`, `Microsoft.ApplicationInsights.DependencyCollector`
>
> (for logging, add `Microsoft.Extensions.Logging.ApplicationInsights`)

```csharp
using System;
using System.Reflection;
using System.Threading.Tasks;
using MassTransit;

namespace Example
{
    public class MyMessageConsumerConsumer : 
        MassTransit.IConsumer<MyMessage>
    {
        public async Task Consume(ConsumeContext<MyMessage> context)
        {
             await Console.Out.WriteLineAsync($"Received: {context.Message.Value}");
        }
    }

    // Message Definition
    public class MyMessage
    {
        public string Value { get; set; }
    }

    public class Program
    {
        public static async Task Main(string[] args)
        {
            var module = new DependencyTrackingTelemetryModule();
            module.IncludeDiagnosticSourceActivities.Add("MassTransit");

            TelemetryConfiguration configuration = TelemetryConfiguration.CreateDefault();
            configuration.InstrumentationKey = "<your instrumentation key>";
            configuration.TelemetryInitializers.Add(new HttpDependenciesParsingTelemetryInitializer());

            var telemetryClient = new TelemetryClient(configuration);
            module.Initialize(configuration);

            var loggerOptions = new ApplicationInsightsLoggerOptions();

            var applicationInsightsLoggerProvider = new ApplicationInsightsLoggerProvider(Options.Create(configuration),
                Options.Create(loggerOptions));

            ILoggerFactory factory = new LoggerFactory();
            factory.AddProvider(applicationInsightsLoggerProvider);

            LogContext.ConfigureCurrentLogContext(factory);

            var busControl = Bus.Factory.CreateUsingInMemory(cfg =>
            {
                cfg.ReceiveEndpoint("my_queue", ec =>
                {
                    ec.Consumer<MyMessageConsumer>();
                });
            });

            using(bus.Start())
            {
                bus.Publish(new MyMessage{Value = "Hello, World."});

                await Task.Run(() => Console.ReadLine());
            }

            module.Dispose();

            telemetryClient.Flush();
            await Task.Delay(5000);

            configuration.Dispose();
        }
    }
}
```
