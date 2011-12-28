// Copyright 2007-2011 Chris Patterson, Dru Sellers, Travis Smith, et. al.
//  
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use 
// this file except in compliance with the License. You may obtain a copy of the 
// License at 
// 
//     http://www.apache.org/licenses/LICENSE-2.0 
// 
// Unless required by applicable law or agreed to in writing, software distributed 
// under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR 
// CONDITIONS OF ANY KIND, either express or implied. See the License for the 
// specific language governing permissions and limitations under the License.
namespace MassTransit.Tests.Diagnostics
{
    using Magnum.Extensions;
    using Magnum.TestFramework;
    using MassTransit.Diagnostics;
    using MassTransit.Diagnostics.Tracing;
    using MassTransit.Testing;

    [Scenario]
    public class When_tracing_messages_on_the_bus
    {
        FutureMessage<ReceivedMessageTraceList> _future;
        ReceivedMessageTraceList _list;
        HandlerTest<BusTestScenario, InputMessage> _test;

        [When]
        public void A_consumer_is_being_tested()
        {
            _test = TestFactory.ForHandler<InputMessage>()
                .New(x =>
                    {
                        x.Handler((context, message) => context.Respond(new OutputMessage()));

                        x.Send(new InputMessage(), (scenario,context) => context.SendResponseTo(scenario.Bus));
                        x.Send(new InputMessage(), (scenario,context) => context.SendResponseTo(scenario.Bus));
                    });

            _test.Execute();

            _test.Received.Any<InputMessage>().ShouldBeTrue();
            _test.Sent.Any<OutputMessage>().ShouldBeTrue();

            _future = new FutureMessage<ReceivedMessageTraceList>();
            _test.Scenario.Bus.GetMessageTrace(_test.Scenario.Bus.ControlBus.Endpoint, _future.Set);

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
            _list.Messages.Count.ShouldEqual(2);

            ReceivedMessageTraceDetail message = _list.Messages[0];

            message.ContentType.ShouldEqual("application/vnd.masstransit+xml");
            message.DestinationAddress.ShouldEqual(_test.Scenario.Bus.Endpoint.Address.Uri);
            message.ResponseAddress.ShouldEqual(_test.Scenario.Bus.Endpoint.Address.Uri);
        }

        [Then]
        public void Should_have_had_one_receiver_in_the_trace_log()
        {
            _list.Messages[0].Receivers.ShouldNotBeNull();
            _list.Messages[0].Receivers.Count.ShouldEqual(1);
        }

        [Then]
        public void Should_have_sent_messages_in_the_trace_log()
        {
            _list.Messages[0].SentMessages.ShouldNotBeNull();
        }


        class InputMessage
        {
        }

        class OutputMessage
        {
        }
    }
}