namespace MassTransit.ServiceBus.Tests.Subscriptions
{
    using System;
    using System.Runtime.Remoting.Messaging;
    using MassTransit.ServiceBus.Subscriptions;
    using MassTransit.ServiceBus.Subscriptions.Messages;
    using NUnit.Framework;
    using Rhino.Mocks;
    using Rhino.Mocks.Constraints;

    [TestFixture]
    public class When_using_the_subscription_manager
    {
        #region Setup/Teardown

        [SetUp]
        public void Setup()
        {
            _mocks = new MockRepository();
            _serviceBus = _mocks.CreateMock<IServiceBus>();
            _managerEndpoint = _mocks.CreateMock<IMessageQueueEndpoint>();
            _client = new SubscriptionManagerClient(_serviceBus, _managerEndpoint);
        }

        #endregion

        private MockRepository _mocks;
        private SubscriptionManagerClient _client;
        private IServiceBus _serviceBus;
        private IMessageQueueEndpoint _managerEndpoint;

        [Test]
        public void The_client_should_request_an_update_at_startup()
        {
            IServiceBusAsyncResult asyncResult = _mocks.CreateMock<IServiceBusAsyncResult>();

            using (_mocks.Record())
            {
                Expect.Call(_serviceBus.Request<CacheUpdateRequest>(_managerEndpoint, (AsyncCallback)null, (object)null, null))
                    .IgnoreArguments()
                    .Constraints(Is.Equal(_managerEndpoint),
                        Is.Anything(),
                        Is.Anything(),
                        Is.Matching<CacheUpdateRequest[]>(
                            delegate(CacheUpdateRequest[] obj) { return true; }))
                    .Return(asyncResult);
            }

            using(_mocks.Playback())
            {
                _client.Start();
            }
        }
    }
}