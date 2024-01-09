namespace MassTransit.DbTransport.Tests;

using System.Threading.Tasks;
using Courier.Contracts;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using TestFramework.Courier;
using Testing;


[TestFixture]
public class Using_a_routing_slip_with_a_custom_subscription
{
    [Test]
    public async Task Should_be_sent()
    {
        await using var provider = new ServiceCollection()
            .ConfigurePostgresTransport()
            .AddMassTransitTestHarness(x =>
            {
                x.AddHandler<RegistrationCompleted>();
                x.AddActivity<TestActivity, TestArguments, TestLog>();

                x.UsingPostgres((context, cfg) =>
                {
                    cfg.ConfigureEndpoints(context);
                });
            })
            .BuildServiceProvider(true);

        var harness = await provider.StartTestHarness();

        var trackingNumber = NewId.NextGuid();
        var builder = new RoutingSlipBuilder(trackingNumber);
        await builder.AddSubscription(harness.GetHandlerAddress<RegistrationCompleted>(), RoutingSlipEvents.Completed, RoutingSlipEventContents.All,
            x => x.Send<RegistrationCompleted>(new { Value = "Secret Value" }));

        builder.AddActivity("TestActivity", harness.GetExecuteActivityAddress<TestActivity, TestArguments>(), new { Value = "Hello" });

        await harness.Bus.Execute(builder.Build());

        IReceivedMessage<RegistrationCompleted> completed = await harness.Consumed
            .SelectAsync<RegistrationCompleted>(x => x.Context.Message.TrackingNumber == trackingNumber).FirstOrDefault();
        Assert.That(completed, Is.Not.Null);

        Assert.That(completed.Context.Message.Value, Is.EqualTo("Secret Value"));
    }


    public interface RegistrationCompleted :
        RoutingSlipCompleted
    {
        string Value { get; }
    }
}