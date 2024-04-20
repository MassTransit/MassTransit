#nullable enable
namespace MassTransit.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using NUnit.Framework;
    using Util;


    [TestFixture]
    public class A_reusable_polling_algorithm
    {
        [Test]
        public async Task Should_be_able_to_control_the_request_flow()
        {
            using var algorithm = new RequestRateAlgorithm(new RequestRateAlgorithmOptions
            {
                PrefetchCount = 100,
                RequestResultLimit = 10
            });

            async Task<IEnumerable<Message>> GetMessages(int resultLimit, CancellationToken cancellationToken)
            {
                await Task.Delay(10, cancellationToken);

                return Enumerable.Range(0, resultLimit).Select(x => new Message());
            }

            async Task ProcessMessage(Message result, CancellationToken cancellationToken)
            {
                await Task.Delay(5, cancellationToken);
            }

            await algorithm.Run(GetMessages, ProcessMessage);
        }

        [Test]
        public async Task Should_be_able_to_control_the_request_flow_by_group()
        {
            using var algorithm = new RequestRateAlgorithm(new RequestRateAlgorithmOptions
            {
                PrefetchCount = 100,
                RequestResultLimit = 10
            });

            IEnumerable<IGrouping<string?, Message>> GroupMessages(IEnumerable<Message> messages)
            {
                return messages.GroupBy(x => x.GroupId, StringComparer.OrdinalIgnoreCase);
            }

            IEnumerable<Message> OrderMessages(IEnumerable<Message> messages)
            {
                return messages.OrderBy(x => x.SequenceNumber);
            }

            async Task<IEnumerable<Message>> GetMessages(int resultLimit, CancellationToken cancellationToken)
            {
                await Task.Delay(10, cancellationToken);

                var sequenceNumber = 0;
                var random = new Random();

                return Enumerable.Range(0, resultLimit).Select(x =>
                {
                    lock (random)
                    {
                        return new Message
                        {
                            SequenceNumber = Interlocked.Increment(ref sequenceNumber),
                            GroupId = random.Next(10).ToString()
                        };
                    }
                });
            }

            async Task ProcessMessage(Message result, CancellationToken cancellationToken)
            {
                await Task.Delay(5, cancellationToken);
            }

            await algorithm.Run(GetMessages, ProcessMessage, GroupMessages, OrderMessages);
            await algorithm.Run(GetMessages, ProcessMessage, GroupMessages, OrderMessages);
            await algorithm.Run(GetMessages, ProcessMessage, GroupMessages, OrderMessages);
            await algorithm.Run(GetMessages, ProcessMessage, GroupMessages, OrderMessages);
            await algorithm.Run(GetMessages, ProcessMessage, GroupMessages, OrderMessages);

            Assert.Multiple(() =>
            {
                Assert.That(algorithm.ActiveRequestCount, Is.EqualTo(0));
                Assert.That(algorithm.MaxActiveRequestCount, Is.EqualTo(10));
            });
        }

        [Test]
        public async Task Should_easily_configure()
        {
            using var state = new RequestRateAlgorithm(new RequestRateAlgorithmOptions
            {
                PrefetchCount = 100,
                RequestResultLimit = 10
            });

            Assert.That(state.RequestCount, Is.EqualTo(1));

            var request = await state.BeginRequest();
            await request.Complete(state.ResultLimit);
            Assert.That(state.RequestCount, Is.EqualTo(6));

            request = await state.BeginRequest();
            await request.Complete(state.ResultLimit);
            Assert.That(state.RequestCount, Is.EqualTo(8));

            request = await state.BeginRequest();
            await request.Complete(state.ResultLimit);
            Assert.That(state.RequestCount, Is.EqualTo(9));

            request = await state.BeginRequest();
            await request.Complete(state.ResultLimit);
            Assert.That(state.RequestCount, Is.EqualTo(10));

            request = await state.BeginRequest();
            await request.Complete(state.ResultLimit);
            Assert.Multiple(() =>
            {
                Assert.That(state.RequestCount, Is.EqualTo(10));

                Assert.That(state.ActiveRequestCount, Is.EqualTo(0));
            });
        }

        [Test]
        public void Should_limit_results_to_prefetch_count()
        {
            using var algorithm = new RequestRateAlgorithm(new RequestRateAlgorithmOptions
            {
                PrefetchCount = 1,
                RequestResultLimit = 10
            });

            Assert.Multiple(() =>
            {
                Assert.That(algorithm.RequestCount, Is.EqualTo(1));
                Assert.That(algorithm.ResultLimit, Is.EqualTo(1));
            });
        }

        [Test]
        public async Task Should_one_ever_have_a_single_request()
        {
            using var algorithm = new RequestRateAlgorithm(new RequestRateAlgorithmOptions
            {
                PrefetchCount = 100,
                RequestResultLimit = 100
            });

            Assert.Multiple(() =>
            {
                Assert.That(algorithm.RequestCount, Is.EqualTo(1));
                Assert.That(algorithm.ResultLimit, Is.EqualTo(100));
            });

            var request = await algorithm.BeginRequest();
            await request.Complete(algorithm.ResultLimit);

            Assert.Multiple(() =>
            {
                Assert.That(algorithm.RequestCount, Is.EqualTo(1));
                Assert.That(algorithm.ResultLimit, Is.EqualTo(100));
            });
        }

        [Test]
        public async Task Should_one_ever_have_a_single_request_even_when_empty()
        {
            using var algorithm = new RequestRateAlgorithm(new RequestRateAlgorithmOptions
            {
                PrefetchCount = 100,
                RequestResultLimit = 100
            });

            Assert.Multiple(() =>
            {
                Assert.That(algorithm.RequestCount, Is.EqualTo(1));
                Assert.That(algorithm.ResultLimit, Is.EqualTo(100));
            });

            var request = await algorithm.BeginRequest();
            await request.Complete(0);

            Assert.Multiple(() =>
            {
                Assert.That(algorithm.RequestCount, Is.EqualTo(1));
                Assert.That(algorithm.ResultLimit, Is.EqualTo(100));
            });
        }


        class Message
        {
            public string? GroupId { get; set; }
            public long SequenceNumber { get; set; }
        }
    }
}
