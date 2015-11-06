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
namespace MassTransit.Tests
{
    namespace Conventional
    {
        using NUnit.Framework;
        using TestFramework;


        [TestFixture]
        public class Configuring_a_consumer_by_convention :
            InMemoryTestFixture
        {
            [Test]
            public void Should_find_the_message_handlers()
            {
            }

            protected override void ConfigureInputQueueEndpoint(IReceiveEndpointConfigurator configurator)
            {
                base.ConfigureInputQueueEndpoint(configurator);

//                configurator.Register
            }
        }


        public interface IHandler<in T>
        {
            void Handle(T message);
        }


        class CustomHandler :
            IHandler<MessageA>,
            IHandler<MessageB>
        {
            public void Handle(MessageA message)
            {
            }

            public void Handle(MessageB message)
            {
            }
        }

        public interface MessageA
        {
            string Value { get; }
        }


        public interface MessageB
        {
            string Name { get; }
        }
    }
}