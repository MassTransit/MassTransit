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
    using System;
    using Magnum.TestFramework;
    using MassTransit.Testing;

    [Scenario]
    public class When_a_date_time_value_is_serialized
    {
        A _received;
        A _sent;
        HandlerTest<A> _test;

        [When]
        public void Setup()
        {
            _test = TestFactory.ForHandler<A>()
                .New(x =>
                    {
                        _sent = new A
                            {
                                Local = new DateTime(2001, 9, 11, 8, 46, 30, DateTimeKind.Local),
                                Universal = new DateTime(2001, 9, 11, 9, 3, 2, DateTimeKind.Local).ToUniversalTime(),
                            };
                        x.Send(_sent);

                        x.Handler((context, message) => { _received = message; });
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
        public void Should_receive_the_local_time_as_local_time()
        {
            _sent.Local.ShouldEqual(_received.Local);
        }

        [Then]
        public void Should_receive_the_universal_time_as_universal_time()
        {
            _sent.Universal.ShouldEqual(_received.Universal);
        }

        class A
        {
            public DateTime Local { get; set; }
            public DateTime Universal { get; set; }
        }
    }
}