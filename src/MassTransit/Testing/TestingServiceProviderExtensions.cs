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
    }
}
