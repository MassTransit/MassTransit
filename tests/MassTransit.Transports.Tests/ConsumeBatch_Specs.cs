namespace MassTransit.Transports.Tests
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using NUnit.Framework;
    using Testing;


    public class Consuming_a_batch_of_significant_size :
        TransportTest
    {
        [Test]
        public async Task Should_consume_the_sent_message()
        {
            var limit = 20;

            await Harness.InputQueueSendEndpoint.SendBatch(Enumerable.Range(0, limit).Select(x => new SubmitOrder {Id = NewId.NextGuid()}));

            await Harness.InactivityTask;

            Batch<SubmitOrder> batch = await _batchConsumer[0];

            Assert.That(batch.Length, Is.EqualTo(limit));
            Assert.That(batch.Mode, Is.EqualTo(BatchCompletionMode.Size));
        }

        TestBatchConsumer _batchConsumer;

        public Consuming_a_batch_of_significant_size(Type harnessType)
            : base(harnessType)
        {
        }

        protected override Task Arrange()
        {
            _batchConsumer = new TestBatchConsumer(Harness.GetTask<Batch<SubmitOrder>>());

            var batchOptions = new BatchOptions
            {
                MessageLimit = 20,
                TimeLimit = TimeSpan.FromSeconds(20)
            };

            Harness.Consumer(() => _batchConsumer, configurator =>
            {
                configurator.Options(batchOptions);
            });

            Harness.OnConfigureReceiveEndpoint += x =>
            {
                batchOptions.Configure(Harness.InputQueueName, x);
            };

            return Task.CompletedTask;
        }


        class TestBatchConsumer :
            IConsumer<Batch<SubmitOrder>>
        {
            readonly TaskCompletionSource<Batch<SubmitOrder>>[] _messageTask;

            int _count;

            public TestBatchConsumer(params TaskCompletionSource<Batch<SubmitOrder>>[] messageTask)
            {
                _messageTask = messageTask;
            }

            public Task<Batch<SubmitOrder>> this[int index] => _messageTask[index].Task;

            public Task Consume(ConsumeContext<Batch<SubmitOrder>> context)
            {
                if (_count < _messageTask.Length)
                    _messageTask[_count++].TrySetResult(context.Message);

                return Task.CompletedTask;
            }
        }


        class SubmitOrder
        {
            public Guid Id { get; set; }
        }
    }
}
