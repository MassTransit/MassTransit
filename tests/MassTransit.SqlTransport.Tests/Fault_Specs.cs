namespace MassTransit.DbTransport.Tests
{
    using System;
    using System.Threading.Tasks;
    using FaultMessages;
    using Microsoft.Extensions.DependencyInjection;
    using NUnit.Framework;
    using Testing;
    using UnitTests;


    [TestFixture]
    public class When_a_consumer_throws_an_exception
    {
        [Test, Explicit]
        public async Task Should_dead_letter_skipped_messages()
        {
            await using var provider = new ServiceCollection()
                .ConfigurePostgresTransport()
                .AddMassTransitTestHarness(x =>
                {
                    x.UsingPostgres((context, cfg) =>
                    {
                        cfg.ReceiveEndpoint("input-queue", e =>
                        {
                            e.PollingInterval = TimeSpan.FromSeconds(.5);
                            e.ConfigureConsumeTopology = false;
                        });
                    });
                })
                .BuildServiceProvider(true);

            var harness = provider.GetTestHarness();

            await harness.Start();

            var endpoint = await harness.Bus.GetSendEndpoint(new Uri("queue:input-queue"));

            await endpoint.Send(new TestMessage("Hello, World!"));

            await Task.Delay(2000);
        }

        [Test, Explicit]
        public async Task Should_publish_fault_and_move_to_the_error_queue()
        {
            await using var provider = new ServiceCollection()
                .ConfigurePostgresTransport()
                .AddMassTransitTestHarness(x =>
                {
                    x.AddHandler(async (ConsumeContext<Fault<MemberUpdateCommand>> _) =>
                    {
                    });
                    x.AddHandler(async (ConsumeContext<UpdateMemberAddress> _) => throw new ApplicationException("I meant to do that!"));

                    x.UsingPostgres((context, cfg) =>
                    {
                        cfg.ConfigureEndpoints(context);
                    });
                })
                .BuildServiceProvider(true);

            var harness = provider.GetTestHarness();

            await harness.Start();

            await harness.Bus.Publish<UpdateMemberAddress>(new
            {
                MemberName = "Frank",
                Address = "123 American Way"
            });

            Assert.That(await harness.Consumed.Any<Fault<MemberUpdateCommand>>(), Is.True);

            await harness.Stop();
        }

        [Test, Explicit]
        public async Task Should_use_built_in_redelivery_to_redeliver_faulted_messages()
        {
            await using var provider = new ServiceCollection()
                .ConfigurePostgresTransport()
                .AddMassTransitTestHarness(x =>
                {
                    x.SetTestTimeouts(testInactivityTimeout: TimeSpan.FromSeconds(2));
                    x.AddHandler(async (ConsumeContext<Fault<MemberUpdateCommand>> _) =>
                    {
                    });
                    x.AddHandler(async (ConsumeContext<UpdateMemberAddress> _) => throw new ApplicationException("I meant to do that!"));

                    x.AddConfigureEndpointsCallback((_, _, cfg) =>
                    {
                        cfg.UseDelayedRedelivery(r => r.Interval(3, TimeSpan.FromSeconds(1)));
                    });

                    x.UsingPostgres((context, cfg) =>
                    {
                        cfg.ConfigureEndpoints(context);
                    });
                })
                .BuildServiceProvider(true);

            var harness = provider.GetTestHarness();

            await harness.Start();

            await harness.Bus.Publish<UpdateMemberAddress>(new
            {
                MemberName = "Frank",
                Address = "123 American Way"
            });

            Assert.That(await harness.Consumed.Any<Fault<MemberUpdateCommand>>(), Is.True);

            await harness.Stop();
        }
    }


    namespace FaultMessages
    {
        [ExcludeFromTopology]
        public interface ICommand
        {
        }


        public interface MemberUpdateCommand :
            ICommand
        {
            string MemberName { get; }
        }


        public interface UpdateMemberAddress :
            MemberUpdateCommand
        {
            string Address { get; }
        }
    }
}
