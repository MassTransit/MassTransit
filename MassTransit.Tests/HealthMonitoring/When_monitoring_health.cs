namespace MassTransit.Tests.HealthMonitoring
{
    using System;
    using System.Threading;
    using MassTransit.Services.HealthMonitoring;
    using MassTransit.Services.HealthMonitoring.Messages;
    using NUnit.Framework;
    using Rhino.Mocks;
    using Rhino.Mocks.Constraints;
    
    [TestFixture]
    public class When_monitoring_health :
        Specification
    {
        private IServiceBus _mockBus;
        private IEndpoint _mockEndpoint;

        private Uri u = new Uri("msmq://localhost/test");

        protected override void Before_each()
        {
            _mockBus = DynamicMock<IServiceBus>();
            _mockEndpoint = DynamicMock<IEndpoint>();
            SetupResult.For(_mockBus.Endpoint).Return(_mockEndpoint);
            SetupResult.For(_mockEndpoint.Uri).Return(u);
        }

        [Test]
        public void Starting_should_enable_the_service()
        {
            HealthClient hc = new HealthClient(_mockBus);
            Assert.IsFalse(hc.Enabled);
            hc.Start();
            Assert.IsTrue(hc.Enabled);
            hc.Stop();
            Assert.IsFalse(hc.Enabled);
            hc.Dispose();
            hc.Dispose();
        }

        [Test]
        public void Calling_dispose_twice_shouldnt_error()
        {
            HealthClient hc = new HealthClient(_mockBus);
            hc.Start();
            hc.Dispose();
            hc.Dispose();
        }


        [Test]
        public void Should_beat_continously()
        {
            ManualResetEvent evt = new ManualResetEvent(false);
            using(Record())
            {
                Expect.Call(delegate { _mockBus.Publish(new Heartbeat(1, u)); })
                    .Constraints(Is.Matching<Heartbeat>(delegate(Heartbeat message)
                                                            {
                                                                return message.EndpointAddress.Equals(u);
                                                            }));

                Expect.Call(delegate { _mockBus.Publish(new Heartbeat(1, u)); })
                    .Constraints(Is.Matching<Heartbeat>(delegate(Heartbeat message)
                                                            {
                                                                if(message.EndpointAddress != u) return false;

                                                                evt.Set();

                                                                return true;
                                                            }));
            }
            using(Playback())
            {
                HealthClient hc = new HealthClient(_mockBus, 1);
                hc.Start();

                Assert.IsTrue(evt.WaitOne(3000, true));
            }


        }

        [Test]
        public void Should_respond_to_pings_by_publishing_pongs()
        {
            Guid id = Guid.NewGuid();
            Ping message = new Ping(id);

            using(Record())
            {
                Expect.Call(delegate { _mockBus.Publish(new Pong(id, u)); })
                    .Constraints(Is.Matching<Pong>(delegate(Pong msg)
                                                       {
                                                           return msg.CorrelationId.Equals(id) &&
                                                                  msg.EndpointUri.Equals(u);
                                                       }));
            }
            using(Playback())
            {
                HealthClient hc = new HealthClient(_mockBus);
                hc.Consume(message);    
            }
        }
    }
}