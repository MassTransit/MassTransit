namespace MassTransit.Testing
{
    using System;
    using Microsoft.Extensions.DependencyInjection;


    public static class TestingServiceProviderExtensions
    {
        public static ITestHarness GetTestHarness(this IServiceProvider provider)
        {
            return provider.GetRequiredService<ITestHarness>();
        }

        public static void AddTaskCompletionSource<T>(this IBusRegistrationConfigurator configurator)
        {
            configurator.AddSingleton(provider => provider.GetRequiredService<ITestHarness>().GetTask<T>());
        }
    }
}
