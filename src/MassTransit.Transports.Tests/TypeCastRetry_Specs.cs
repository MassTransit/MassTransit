// Copyright 2007-2018 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.Transports.Tests
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using GreenPipes;
    using NUnit.Framework;
    using Testing;


    public class TypeCastRetry_Specs :
        TransportTest
    {
        public TypeCastRetry_Specs(Type harnessType)
            : base(harnessType)
        {
        }

        ConsumerTestHarness<CreateCommandConsumer> _consumer;

        [Test]
        public async Task Should_receive_the_message()
        {
            Assert.That(_consumer.Consumed.Select<CreateCommand>().Any(), Is.True);
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            _consumer = Harness.Consumer<CreateCommandConsumer>();

            Harness.OnConfigureBus += configurator =>
            {
                var sec5 = TimeSpan.FromSeconds(5);
                configurator.UseRetry(x => x.Exponential(2, sec5, sec5, sec5));
            };

            await Harness.Start();

            await Harness.Bus.Publish(new CreateCommand
            {
                Name = "Test"
            });
        }


        public class CreateCommand :
            Command
        {
            public Guid Id { get; set; }
            public string Name { get; set; }

            public CreateCommand()
            {
                Id = Guid.NewGuid();
            }
        }


        public interface ICommand
        {
        }


        public abstract class Command :
            ICommand
        {
        }


        public class CreateCommandConsumer : IConsumer<CreateCommand>
        {
            public Task Consume(ConsumeContext<CreateCommand> context)
            {
                return Task.CompletedTask;
            }
        }
    }
}