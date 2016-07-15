// Copyright 2007-2015 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
    namespace Polymorphic
    {
        using System.Threading.Tasks;
        using Newtonsoft.Json;
        using NUnit.Framework;
        using TestFramework;

        // message interface
        public interface ITestMessage
        {
            [JsonProperty(TypeNameHandling = TypeNameHandling.Objects)]
            TestBaseClass Data { get; }
        };


        // message implementation
        public class TestMessage : ITestMessage
        {
            public TestBaseClass Data { get; set; }
        }


        // abstract child
        public abstract class TestBaseClass
        {
        }


        // a concrete implementation of abstract child, cannot be deserialized 
        // by default serializer configuration
        public class TestConcreteClass : TestBaseClass
        {
        }


        [TestFixture]
        public class PolymorphicProperty_Specs :
            InMemoryTestFixture
        {
            [Test]
            public async Task Verify_consumed_message_contains_property()
            {
                ITestMessage message = new TestMessage
                {
                    Data = new TestConcreteClass()
                };

                await InputQueueSendEndpoint.Send(message);

                ConsumeContext<ITestMessage> context = await _handled;

                Assert.IsInstanceOf<TestConcreteClass>(context.Message.Data);
            }

            Task<ConsumeContext<ITestMessage>> _handled;

            protected override void ConfigureInputQueueEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
            {
                _handled = Handled<ITestMessage>(configurator);
            }
        }
    }
}