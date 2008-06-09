namespace MassTransit.ServiceBus.Tests.Subscriptions
{
	using System;
	using MassTransit.ServiceBus.Subscriptions;
	using NUnit.Framework;
	using NUnit.Framework.SyntaxHelpers;
	using Rhino.Mocks;

	[TestFixture]
	public class When_a_handler_unsubscribes_from_the_service_bus
        : Specification
	{
	    private IObjectBuilder _builder;

        protected override void Before_each()
        {
            _builder = StrictMock<IObjectBuilder>();
			_endpoint = _mocks.DynamicMock<IEndpoint>();
			_cache = _mocks.DynamicMock<ISubscriptionCache>();


			_bus = new ServiceBus(_endpoint, _builder, _cache);
        }


		[Test]
		public void The_service_bus_should_continue_to_handle_messages_if_at_least_one_handler_is_available()
		{
			using (_mocks.Record())
			{
				Expect.Call(_endpoint.Uri).Return(_endpointUri).Repeat.Any();
				Expect.Call(delegate { _cache.Add(null); }).IgnoreArguments();

				Expect.Call(delegate { _cache.Add(null); }).IgnoreArguments();

				Expect.Call(delegate { _cache.Remove(null); }).IgnoreArguments();
				//        Expect.Call(delegate { _cache.Remove(null); }).IgnoreArguments();
			}

			using (_mocks.Playback())
			{
				_bus.Subscribe<PingMessage>(HandleAllMessages);
				Assert.That(_bus.Accept(_message), Is.True);

				_bus.Subscribe<PingMessage>(HandleAllMessages, HandleSomeMessagesPredicate);
				Assert.That(_bus.Accept(_message), Is.True);

				_bus.Unsubscribe<PingMessage>(HandleAllMessages);
				Assert.That(_bus.Accept(_message), Is.True);

				_bus.Unsubscribe<PingMessage>(HandleAllMessages, HandleSomeMessagesPredicate);
				Assert.That(_bus.Accept(_message), Is.False);
			}
		}

		[Test]
		public void The_service_bus_should_no_longer_show_the_message_type_as_handled()
		{
			using (_mocks.Record())
			{
				Expect.Call(_endpoint.Uri).Return(_endpointUri).Repeat.Any();
				Expect.Call(delegate { _cache.Add(null); }).IgnoreArguments();

				Expect.Call(delegate { _cache.Remove(null); }).IgnoreArguments();
			}

			using (_mocks.Playback())
			{
				_bus.Subscribe<PingMessage>(HandleAllMessages);
				Assert.That(_bus.Accept(_message), Is.True);

				_bus.Unsubscribe<PingMessage>(HandleAllMessages);
				Assert.That(_bus.Accept(_message), Is.False);
			}
		}

		private MockRepository _mocks = new MockRepository();
		private IEndpoint _endpoint;
		private ISubscriptionCache _cache;
		private ServiceBus _bus;
		private object _message = new PingMessage();
		private Uri _endpointUri = new Uri("msmq://localhost/test");


		private static void HandleAllMessages(IMessageContext<PingMessage> ctx)
		{
		}

		private static bool HandleSomeMessagesPredicate(PingMessage message)
		{
			return true;
		}
	}
}