namespace MassTransit.Testing
{
    using System;
    using System.Threading.Tasks;
    using Microsoft.Extensions.DependencyInjection;


    public static class TestingServiceProviderExtensions
    {
        public static ITestHarness GetTestHarness(this IServiceProvider provider)
        {
            return provider.GetRequiredService<ITestHarness>();
        }

        public static async Task<Task<ConsumeContext<T>>> ConnectPublishHandler<T>(this ITestHarness harness, Func<ConsumeContext<T>, bool> filter)
            where T : class
        {
            TaskCompletionSource<ConsumeContext<T>> source = harness.GetTask<ConsumeContext<T>>();

            var handle = harness.Bus.ConnectReceiveEndpoint(configurator =>
            {
                configurator.Handler<T>(async context =>
                {
                    if (filter(context))
                        source.TrySetResult(context);
                });
            });

            await handle.Ready;

            return source.Task;
        }

        public static void AddTaskCompletionSource<T>(this IBusRegistrationConfigurator configurator)
        {
            configurator.AddSingleton(provider => provider.GetRequiredService<ITestHarness>().GetTask<T>());
        }
    }
}
