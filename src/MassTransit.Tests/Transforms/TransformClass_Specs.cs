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
namespace MassTransit.Tests.Transforms
{
    using System.Threading.Tasks;
    using NUnit.Framework;
    using Shouldly;
    using TestFramework;
    using Transformation.TransformConfigurators;


    [TestFixture]
    public class Using_a_class_to_define_a_transform :
        InMemoryTestFixture
    {
        [Test]
        public async Task Should_have_the_message_property()
        {
            await InputQueueSendEndpoint.Send(new A {First = "Hello"});

            ConsumeContext<A> result = await _received;

            result.Message.First.ShouldBe("First");
            result.Message.Second.ShouldBe("Second");
        }

        Task<ConsumeContext<A>> _received;

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            base.ConfigureInMemoryReceiveEndpoint(configurator);

            configurator.UseTransform<A>(x => x.Get<FullTransform>());

            _received = Handled<A>(configurator);
        }


        class FullTransform :
            ConsumeTransformSpecification<A>
        {
            public FullTransform()
            {
                Set(x => x.First, context => "First");
                Set(x => x.Second, context => "Second");
            }
        }


        class A
        {
            public string First { get; set; }
            public string Second { get; set; }
        }
    }
}