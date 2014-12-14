namespace MassTransit.Tests
{
    using System.Linq;
    using Magnum.Extensions;
    using NUnit.Framework;
    using NUnit.Framework;
    using Shouldly;
    using TestFramework;
    using TextFixtures;


    [TestFixture]
    public class When_using_mixed_serialization_types :
        LoopbackLocalAndRemoteTestFixture
    {
        readonly Future<A> _requestReceived;
        readonly Future<B> _responseReceived;

        public When_using_mixed_serialization_types()
        {
            _requestReceived = new Future<A>(); 
            _responseReceived = new Future<B>();
        }

        [Test]
        public void Should_be_able_to_read_xml_when_using_json()
        {
            Assert.IsTrue(RemoteBus.ShouldHaveSubscriptionFor<B>().Any());
            Assert.IsTrue(LocalBus.ShouldHaveSubscriptionFor<A>().Any());

            LocalBus.GetEndpoint(RemoteUri).Send(new A { Key = "Hello" });

            _requestReceived.WaitUntilCompleted(8.Seconds()).ShouldBe(true);

            _responseReceived.WaitUntilCompleted(8.Seconds()).ShouldBe(true);

        }

        protected override void ConfigureLocalBus(BusConfigurators.ServiceBusConfigurator configurator)
        {
            base.ConfigureLocalBus(configurator);

            configurator.UseJsonSerializer();
            configurator.SupportXmlSerializer();

            ///configurator.Subscribe(s => s.Handler<B>(async context => _responseReceived.Complete(context.Message)));
        }

        protected override void ConfigureRemoteBus(BusConfigurators.ServiceBusConfigurator configurator)
        {
            base.ConfigureRemoteBus(configurator);

            configurator.UseXmlSerializer();
            configurator.SupportJsonSerializer();

//            configurator.Subscribe(s => s.Handler<A>(async (context) =>
//            {
//                _requestReceived.Complete(context.Message);
//
//                context.Respond(new B { Key = context.Message.Key, Value = "Value of " + context.Message.Key });
//            }));
        }


        class A
        {
            public string Key { get; set; }
        }

        class B
        {
            public string Key { get; set; }
            public string Value { get; set; }
        }

    }
}
