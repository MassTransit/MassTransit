// Copyright 2007-2011 Chris Patterson, Dru Sellers, Travis Smith, Simon Guindon, et. al.
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
    public class When_a_byte_array_value_is_serialized
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
                        Contents = new byte[] {0x56, 0x34, 0xf3}                        
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
        public void Should_receive_byte_array()
        {
            _sent.Contents.ShouldEqual(_received.Contents);
        }

        class A
        {
            public byte[] Contents { get; set; }            
        }
    }
}