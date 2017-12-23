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


    [TestFixture]
    public class Setting_a_property_on_the_original_message :
        InMemoryTestFixture
    {
        [Test]
        public async Task Should_have_the_message_property()
        {
            await InputQueueSendEndpoint.Send(new A {First = "Hello"});

            ConsumeContext<A> result = await _received;

            result.Message.First.ShouldBe("Hello");
            result.Message.Second.ShouldBe("World");
        }

        Task<ConsumeContext<A>> _received;

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            base.ConfigureInMemoryReceiveEndpoint(configurator);

            configurator.UseTransform<A>(t =>
            {
                // Replace modifies the original message, versus Set which leaves the original message unmodified
                t.Replace(x => x.Second, context => "World");
            });

            _received = Handled<A>(configurator);
        }


        class A
        {
            public string First { get; set; }
            public string Second { get; set; }
        }
    }


    [TestFixture]
    public class Setting_a_property_on_a_new_message :
        InMemoryTestFixture
    {
        [Test]
        public async Task Should_have_the_message_property()
        {
            await InputQueueSendEndpoint.Send(new A {First = "Hello"});

            ConsumeContext<A> result = await _received;

            result.Message.First.ShouldBe("Hello");
            result.Message.Second.ShouldBe("World");
        }

        Task<ConsumeContext<A>> _received;

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            base.ConfigureInMemoryReceiveEndpoint(configurator);

            configurator.UseTransform<A>(t =>
            {
                t.Set(x => x.Second, context => "World");
            });

            _received = Handled<A>(configurator);
        }


        class A
        {
            public string First { get; set; }
            public string Second { get; set; }
        }
    }


    [TestFixture]
    public class Setting_a_property_on_the_original_message_interface :
        InMemoryTestFixture
    {
        [Test]
        public async Task Should_have_the_message_property()
        {
            Task<ConsumeContext<IA>> unmodified = ConnectPublishHandler<IA>();

            await Bus.Publish(new A {First = "Hello"});

            ConsumeContext<IA> result = await _received;
            ConsumeContext<IA> original = await unmodified;
            IA tweaked = await _tweaked.Task;

            result.Message.First.ShouldBe("Hello");
            result.Message.Second.ShouldBe("World");
            tweaked.Second.ShouldBe("World");

            original.Message.First.ShouldBe("Hello");
            original.Message.Second.ShouldBe(null);
        }

        Task<ConsumeContext<IA>> _received;
        TaskCompletionSource<IA> _tweaked;

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            base.ConfigureInMemoryReceiveEndpoint(configurator);

            _received = Handled<IA>(configurator);

            _tweaked = GetTask<IA>();

            configurator.Handler<IA>(async context => _tweaked.TrySetResult(context.Message), x => x.UseTransform(t => t.Replace(p => p.Second, _ => "World")));
        }


        public interface IA
        {
            string First { get; }
            string Second { get; }
        }


        class A : IA
        {
            public string First { get; set; }
            public string Second { get; set; }
        }
    }

    [TestFixture]
    public class Setting_a_property_on_the_message_interface :
        InMemoryTestFixture
    {
        [Test]
        public async Task Should_have_the_message_property()
        {
            Task<ConsumeContext<IA>> unmodified = ConnectPublishHandler<IA>();

            await Bus.Publish(new A {First = "Hello"});

            ConsumeContext<IA> result = await _received;
            ConsumeContext<IA> original = await unmodified;
            IA tweaked = await _tweaked.Task;

            result.Message.First.ShouldBe("Hello");
            result.Message.Second.ShouldBe(null);

            tweaked.Second.ShouldBe("World");

            original.Message.First.ShouldBe("Hello");
            original.Message.Second.ShouldBe(null);
        }

        Task<ConsumeContext<IA>> _received;
        TaskCompletionSource<IA> _tweaked;

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            base.ConfigureInMemoryReceiveEndpoint(configurator);

            _received = Handled<IA>(configurator);

            _tweaked = GetTask<IA>();

            configurator.Handler<IA>(async context => _tweaked.TrySetResult(context.Message), x =>
            {
                x.UseTransform(t =>
                {
                    t.Set(p => p.Second, context => "World");
                });
            });
        }


        public interface IA
        {
            string First { get; }
            string Second { get; }
        }


        class A : IA
        {
            public string First { get; set; }
            public string Second { get; set; }
        }
    }
}