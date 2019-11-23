# Configuring Application Insights

Get actionable insights through application performance management and instant analytics

[Create an Application Insights resource](https://docs.microsoft.com/en-us/azure/application-insights/app-insights-create-new-resource#create-an-application-insights-resource-1)

[Copy the instrumentation key](https://docs.microsoft.com/en-us/azure/application-insights/app-insights-create-new-resource#copy-the-instrumentation-key)

> Requires NuGets `MassTransit`, `MassTransit.ApplicationInsights`

```csharp
using System;
using System.Reflection;
using System.Threading.Tasks;
using MassTransit;

namespace Example
{
    public class MyMessageConsumerConsumer : MassTransit.IConsumer<MyMessage>
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
        public static void Main(string[] args)
        {
            var configuration = new TelemetryConfiguration();
            // Your instrumentation key can be found in Azure Portal
            configuration.InstrumentationKey = "fb8a0b03-235a-4b52-b491-307e9fd6b209";

            var telemetryClient = new TelemetryClient(configuration);

            var busControl = Bus.Factory.CreateUsingInMemory(cfg =>
            {
                cfg.UseApplicationInsights(telemetryClient);

                cfg.ReceiveEndpoint("my_queue", ec =>
                {
                    ec.Consumer<MyMessageConsumer>();
                });
            });

            using(bus.Start())
            {
                bus.Publish(new MyMessage{Value = "Hello, World."});

                Console.ReadLine();
            }
        }
    }
}
```
