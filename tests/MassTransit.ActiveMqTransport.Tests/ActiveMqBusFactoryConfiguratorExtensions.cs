using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MassTransit.ActiveMqTransport.Tests
{
    internal static class ActiveMqBusFactoryConfiguratorExtensions
    {
        public static void ConfigureHost(this IActiveMqBusFactoryConfigurator configurator, string testFlavor)
        {
            if (testFlavor == "artemis")
            {
                configurator.Host("localhost", 61618, cfgHost =>
                {
                    cfgHost.Username("admin");
                    cfgHost.Password("admin");
                });
                configurator.EnableArtemisCompatibility();
            }
            else if (testFlavor == ActiveMqHostAddress.AmqpScheme)
            {
                configurator.Host(new Uri("amqp://localhost:5672"), cfgHost =>
                {
                    cfgHost.Username("admin");
                    cfgHost.Password("admin");
                });
            }
        }
    }
}
