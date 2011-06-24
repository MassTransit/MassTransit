namespace MassTransit.Tests.Diagnostics
{
	using Magnum.Extensions;
	using Magnum.TestFramework;
	using MassTransit.Diagnostics;
	using MassTransit.Testing;

	[Scenario]
	public class When_tracing_messages_on_the_bus
	{
		HandlerTest<InputMessage> _test;
		FutureMessage<MessageTraceList> _future;
		MessageTraceList _list;

		[When]
		public void A_consumer_is_being_tested()
		{
			_test = TestFactory.ForHandler<InputMessage>()
				.New(x =>
					{
						x.Handler((bus,message) =>
							{
								bus.MessageContext<InputMessage>().Respond(new OutputMessage());
							});

					x.Send(new InputMessage(), c => c.SendResponseTo(_test.Scenario.Bus));
				});

			_test.Execute();

			_test.Received.Any<InputMessage>().ShouldBeTrue();
			_test.Sent.Any<OutputMessage>().ShouldBeTrue();

			_future = new FutureMessage<MessageTraceList>();
			_test.Scenario.Bus.SubscribeHandler<MessageTraceList>(_future.Set);

			_test.Scenario.Bus.ControlBus.Endpoint.Send<GetMessageTraceList>(new GetMessageTraceListImpl { Count = 1 }, x =>
			{
				x.SendResponseTo(_test.Scenario.Bus);
			});

			_future.IsAvailable(8.Seconds()).ShouldBeTrue();
			_list = _future.Message;
		}

		[Finally]
		public void Teardown()
		{
			_test.Dispose();
			_test = null;
		}

		[Then]
		public void Should_have_handled_one_message_in_the_trace_log()
		{
			_list.ShouldNotBeNull();
			_list.Messages.ShouldNotBeNull();
			_list.Messages.Count.ShouldEqual(1);
		}

		[Then]
		public void Should_have_had_one_receiver_in_the_trace_log()
		{
			_list.Messages[0].Receivers.ShouldNotBeNull();
			_list.Messages[0].Receivers.Count.ShouldEqual(1);
		}


		class InputMessage
		{
		}

		class OutputMessage
		{
			
		}
	}
}