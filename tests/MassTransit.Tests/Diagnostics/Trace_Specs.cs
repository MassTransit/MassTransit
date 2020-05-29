namespace MassTransit.Tests.Diagnostics
{
    //    using Magnum.Extensions;
    //    using NUnit.Framework;
    //    using MassTransit.Diagnostics;
    //    using MassTransit.Diagnostics.Tracing;
    //    using MassTransit.Testing;
    //
    //
    //    public class When_tracing_messages_on_the_bus
    //    {
    //        FutureMessage<ReceivedMessageTraceList> _future;
    //        ReceivedMessageTraceList _list;
    //        IHandlerTest<IBusTestScenario, InputMessage> _test;
    //
    //        [SetUp]
    //        public void A_consumer_is_being_tested()
    //        {
    //            _test = TestFactory.ForHandler<InputMessage>()
    //                .New(x =>
    //                    {
    //                        x.Handler((context, message) => context.Respond(new OutputMessage()));
    //
    //                        x.Send(new InputMessage(), (scenario,context) => context.SendResponseTo(scenario.Bus));
    //                        x.Send(new InputMessage(), (scenario,context) => context.SendResponseTo(scenario.Bus));
    //                    });
    //
    //            _test.Execute();
    //
    //            _test.Received.Select<InputMessage>().ShouldBe(true);
    //            _test.Sent.Select<OutputMessage>().ShouldBe(true);
    //
    //            _future = new FutureMessage<ReceivedMessageTraceList>();
    //            _test.Scenario.Bus.GetMessageTrace(_test.Scenario.Bus.Endpoint, _future.Set);
    //
    //            _future.IsAvailable(TimeSpan.FromSeconds(8)).ShouldBe(true);
    //            _list = _future.Message;
    //        }
    //
    //        [TearDown]
    //        public void Teardown()
    //        {
    //            _test.Dispose();
    //            _test = null;
    //        }
    //
    //        [Test]
    //        public void Should_have_handled_one_message_in_the_trace_log()
    //        {
    //            _list.ShouldNotBe(null);
    //            _list.Messages.ShouldNotBe(null);
    //            _list.Messages.Count.ShouldBe(2);
    //
    //            ReceivedMessageTraceDetail message = _list.Messages[0];
    //
    //            message.ContentType.ShouldBe("application/vnd.masstransit+xml");
    //            message.DestinationAddress.ShouldBe(_test.Scenario.Bus.Endpoint.Address.Uri);
    //            message.ResponseAddress.ShouldBe(_test.Scenario.Bus.Endpoint.Address.Uri);
    //        }
    //
    //        [Test]
    //        public void Should_have_had_one_receiver_in_the_trace_log()
    //        {
    //            _list.Messages[0].Receivers.ShouldNotBe(null);
    //            _list.Messages[0].Receivers.Count.ShouldBe(1);
    //        }
    //
    //        [Test]
    //        public void Should_have_sent_messages_in_the_trace_log()
    //        {
    //            _list.Messages[0].SentMessages.ShouldNotBe(null);
    //        }
    //
    //
    //        class InputMessage
    //        {
    //        }
    //
    //        class OutputMessage
    //        {
    //        }
    //    }
}
