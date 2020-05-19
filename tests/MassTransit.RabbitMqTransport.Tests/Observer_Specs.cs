// Copyright 2007-2017 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.RabbitMqTransport.Tests
{
    using System;
    using System.Threading.Tasks;
    using NUnit.Framework;
    using ObserverableMessages;

    namespace ObserverableMessages
    {
        class SomethingHappened
        {
            public string Caption { get; set; }
        }
    }


    [TestFixture]
    public class Publishing_messages_with_an_observer :
        RabbitMqTestFixture
    {
        [Test]
        public async Task Should_be_received()
        {
            await Bus.Publish(new SomethingHappened {Caption = "System Screw Up"});

            await _observer.Received;
        }

        EventObserver _observer;

        protected override void ConfigureRabbitMqReceiveEndpoint(IRabbitMqReceiveEndpointConfigurator configurator)
        {
            _observer = new EventObserver(GetTask<SomethingHappened>());

            configurator.Observer(_observer);
        }


        class EventObserver :
            IObserver<ConsumeContext<SomethingHappened>>
        {
            readonly TaskCompletionSource<SomethingHappened> _completed;

            public EventObserver(TaskCompletionSource<SomethingHappened> completed)
            {
                _completed = completed;
            }

            public Task<SomethingHappened> Received => _completed.Task;

            public void OnNext(ConsumeContext<SomethingHappened> context)
            {
                _completed.TrySetResult(context.Message);
            }

            public void OnError(Exception error)
            {
            }

            public void OnCompleted()
            {
            }
        }
    }
}