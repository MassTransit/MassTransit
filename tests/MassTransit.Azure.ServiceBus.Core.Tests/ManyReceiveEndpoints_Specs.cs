namespace MassTransit.Azure.ServiceBus.Core.Tests
{
    using System;
    using System.Threading.Tasks;
    using ManyMessages;
    using MassTransit.Testing;
    using NUnit.Framework;
    using TestFramework;


    namespace ManyMessages
    {
        public class Foo01
        {
        }


        public class Foo02
        {
        }


        public class Foo03
        {
        }


        public class Foo04
        {
        }


        public class Foo05
        {
        }


        public class Foo06
        {
        }


        public class Foo07
        {
        }


        public class Foo08
        {
        }


        public class Foo09
        {
        }


        public class Foo10
        {
        }


        public class Foo11
        {
        }


        public class Foo12
        {
        }


        public class Foo13
        {
        }


        public class Foo14
        {
        }


        public class Foo15
        {
        }


        public class Foo16
        {
        }


        public class Foo17
        {
        }


        public class Foo18
        {
        }


        public class Foo19
        {
        }


        public class Foo20
        {
        }


        public class Foo21
        {
        }


        public class Foo22
        {
        }


        public class Foo23
        {
        }


        public class Foo24
        {
        }


        public class Foo25
        {
        }


        public class Foo26
        {
        }


        public class Foo27
        {
        }


        public class Foo28
        {
        }


        public class Foo29
        {
        }


        public class Foo30
        {
        }
    }


    [TestFixture]
    [Explicit]
    public class Setting_up_a_bunch_of_receive_endpoints :
        BusTestFixture
    {
        [Test]
        public async Task Should_start_stop_and_start()
        {
            ServiceBusTokenProviderSettings settings = new TestAzureServiceBusAccountSettings();
            var serviceBusNamespace = Configuration.ServiceNamespace;

            var serviceUri = AzureServiceBusEndpointUriCreator.Create(
                serviceBusNamespace,
                "MassTransit.Azure.ServiceBus.Core.Tests"
            );
            var bus = MassTransit.Bus.Factory.CreateUsingAzureServiceBus(cfg =>
            {
                cfg.Host(serviceUri, h =>
                {
                    h.NamedKey(s =>
                    {
                        s.NamedKeyCredential = settings.NamedKeyCredential;
                    });
                });

                ConfigureBusDiagnostics(cfg);

                cfg.ReceiveEndpoint("queue01", ep => ep.Consumer(() => new Consumer<Foo01>()));
                cfg.ReceiveEndpoint("queue02", ep => ep.Consumer(() => new Consumer<Foo02>()));
                cfg.ReceiveEndpoint("queue03", ep => ep.Consumer(() => new Consumer<Foo03>()));
                cfg.ReceiveEndpoint("queue04", ep => ep.Consumer(() => new Consumer<Foo04>()));
                cfg.ReceiveEndpoint("queue05", ep => ep.Consumer(() => new Consumer<Foo05>()));
                cfg.ReceiveEndpoint("queue06", ep => ep.Consumer(() => new Consumer<Foo06>()));
                cfg.ReceiveEndpoint("queue07", ep => ep.Consumer(() => new Consumer<Foo07>()));
                cfg.ReceiveEndpoint("queue08", ep => ep.Consumer(() => new Consumer<Foo08>()));
                cfg.ReceiveEndpoint("queue09", ep => ep.Consumer(() => new Consumer<Foo09>()));

                cfg.ReceiveEndpoint("queue10", ep => ep.Consumer(() => new Consumer<Foo10>()));
                cfg.ReceiveEndpoint("queue11", ep => ep.Consumer(() => new Consumer<Foo11>()));
                cfg.ReceiveEndpoint("queue12", ep => ep.Consumer(() => new Consumer<Foo12>()));
                cfg.ReceiveEndpoint("queue13", ep => ep.Consumer(() => new Consumer<Foo13>()));
                cfg.ReceiveEndpoint("queue14", ep => ep.Consumer(() => new Consumer<Foo14>()));
                cfg.ReceiveEndpoint("queue15", ep => ep.Consumer(() => new Consumer<Foo15>()));
                cfg.ReceiveEndpoint("queue16", ep => ep.Consumer(() => new Consumer<Foo16>()));
                cfg.ReceiveEndpoint("queue17", ep => ep.Consumer(() => new Consumer<Foo17>()));
                cfg.ReceiveEndpoint("queue18", ep => ep.Consumer(() => new Consumer<Foo18>()));
                cfg.ReceiveEndpoint("queue19", ep => ep.Consumer(() => new Consumer<Foo19>()));

                cfg.ReceiveEndpoint("queue20", ep => ep.Consumer(() => new Consumer<Foo20>()));
                cfg.ReceiveEndpoint("queue21", ep => ep.Consumer(() => new Consumer<Foo21>()));
                cfg.ReceiveEndpoint("queue22", ep => ep.Consumer(() => new Consumer<Foo22>()));
                cfg.ReceiveEndpoint("queue23", ep => ep.Consumer(() => new Consumer<Foo23>()));
                cfg.ReceiveEndpoint("queue24", ep => ep.Consumer(() => new Consumer<Foo24>()));
                cfg.ReceiveEndpoint("queue25", ep => ep.Consumer(() => new Consumer<Foo25>()));
                cfg.ReceiveEndpoint("queue26", ep => ep.Consumer(() => new Consumer<Foo26>()));
                cfg.ReceiveEndpoint("queue27", ep => ep.Consumer(() => new Consumer<Foo27>()));
                cfg.ReceiveEndpoint("queue28", ep => ep.Consumer(() => new Consumer<Foo28>()));
                cfg.ReceiveEndpoint("queue29", ep => ep.Consumer(() => new Consumer<Foo29>()));
                cfg.ReceiveEndpoint("queue30", ep => ep.Consumer(() => new Consumer<Foo30>()));
            });

            await bus.StartAsync(TestCancellationToken);
            try
            {
            }
            finally
            {
                await bus.StopAsync(TestCancellationToken);
            }

            await bus.StartAsync(TestCancellationToken);
            try
            {
            }
            finally
            {
                await bus.StopAsync(TestCancellationToken);
            }
        }

        public Setting_up_a_bunch_of_receive_endpoints()
            : base(new InMemoryTestHarness())
        {
            TestTimeout = TimeSpan.FromMinutes(5);
        }


        class Consumer<TFoo> : IConsumer<TFoo>
            where TFoo : class
        {
            public Task Consume(ConsumeContext<TFoo> context)
            {
                return Task.CompletedTask;
            }
        }
    }
}
