namespace MassTransit.Tests;

using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;


[TestFixture]
public class When_the_transport_is_not_configured
{
    [Test]
    public async Task Should_throw_a_configuration_exception()
    {
        var provider = new ServiceCollection()
            .AddMassTransitTextWriterLogger()
            .AddMassTransit(x =>
            {
            })
            .BuildServiceProvider();

        try
        {
            Assert.That(() => provider.StartHostedServices(), Throws.TypeOf<ConfigurationException>());
        }
        finally
        {
            await provider.DisposeAsync();
        }
    }
}
