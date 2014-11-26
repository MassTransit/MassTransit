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
namespace MassTransit.Tests.Serialization
{
    using Magnum.TestFramework;
    using MassTransit.Testing;

    [Scenario]
    public class When_a_message_has_no_default_constructor
    {
        A _received;
        A _sent;
        HandlerTest<A> _test;

        [When]
        public void A_message_has_no_default_constructor()
        {
            _test = TestFactory.ForHandler<A>()
                .New(x =>
                    {
                        _sent = new A("Dru", "Sellers");
                        x.Send(_sent);

                        x.Handler(async (context ) => { _received = context.Message; });
                    });

            _test.Execute();

            _test.Received.Any<A>().ShouldBeTrue();
        }

        [Finally]
        public void Teardown()
        {
            _test.Dispose();
            _test = null;
        }

        [Then]
        public void Should_be_able_to_serialize_the_message()
        {
            _received.ShouldNotBeNull();
        }

        [Then]
        public void Should_match_the_name()
        {
            _received.Name.ShouldEqual("Dru");
        }

        [Then]
        public void Should_match_the_value()
        {
            _received.Value.ShouldEqual("Sellers");
        }

        class A
        {
            public A(string name, string value)
            {
                Name = name;
                Value = value;
            }

            public string Name { get; private set; }
            public string Value { get; private set; }
        }
    }
}