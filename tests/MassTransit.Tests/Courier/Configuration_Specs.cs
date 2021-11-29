namespace MassTransit.Tests.Courier
{
    using System;
    using NUnit.Framework;
    using TestFramework;
    using TestFramework.Courier;


    [TestFixture]
    public class When_configuring_activity_hosts_with_masstransit :
        InMemoryTestFixture
    {
        [Test]
        public void Should_have_a_clean_interface()
        {
        }

        public When_configuring_activity_hosts_with_masstransit()
        {
            _executeUri = new Uri("loopback://localhost/execute_testactivity");
            _compensateUri = new Uri("loopback://localhost/compensate_testactivity");
        }

        readonly Uri _executeUri;
        readonly Uri _compensateUri;

        protected override void ConfigureInMemoryBus(IInMemoryBusFactoryConfigurator configurator)
        {
            configurator.ReceiveEndpoint("execute_testactivity", x =>
            {
                x.ExecuteActivityHost<TestActivity, TestArguments>(_compensateUri, h =>
                {
                    h.UseExecute(context => Console.WriteLine(context.ActivityName));

                    h.ActivityArguments(a => a.UseExecute(context => Console.WriteLine(context.Arguments.Value)));

                    h.RoutingSlip(rs => rs.UseExecute(context => Console.WriteLine(context.Message.TrackingNumber.ToString("N"))));
                });
            });

            configurator.ReceiveEndpoint("compensate_testactivity", x =>
            {
                x.CompensateActivityHost<TestActivity, TestLog>(h =>
                {
                    h.UseExecute(context => Console.WriteLine(context.Log.OriginalValue));

                    h.ActivityLog(l => l.UseExecute(context => Console.WriteLine(context.Log.OriginalValue)));

                    h.RoutingSlip(rs => rs.UseExecute(context => Console.WriteLine(context.Message.TrackingNumber.ToString("N"))));
                });
            });
        }
    }
}
