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
            await Bus.Publish(new SomethingHappened { Caption = "System Screw Up" });

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
