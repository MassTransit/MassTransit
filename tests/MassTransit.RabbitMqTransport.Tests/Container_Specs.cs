namespace MassTransit.RabbitMqTransport.Tests
{
    // public class Container_Specs :
    //     InMemoryTestFixture
    // {
    //     [Test]
    //     public async Task Should_configure_the_container()
    //     {
    //         var services = new ServiceCollection();
    //         services.AddSingleton(LoggerFactory);
    //         services.TryAdd(ServiceDescriptor.Singleton(typeof(ILogger<>), typeof(Logger<>)));
    //         services.AddMassTransit(x =>
    //         {
    //             x.UsingRabbitMq((context, cfg) =>
    //             {
    //                 ConfigureBusDiagnostics(cfg);
    //
    //                 cfg.ConfigureEndpoints(context);
    //             });
    //         });
    //         var provider = services.BuildServiceProvider();
    //
    //         var busControl = provider.GetRequiredService<IBusControl>();
    //
    //         await busControl.StartAsync();
    //         try
    //         {
    //         }
    //         finally
    //         {
    //             await busControl.StopAsync();
    //         }
    //     }
    // }
}
