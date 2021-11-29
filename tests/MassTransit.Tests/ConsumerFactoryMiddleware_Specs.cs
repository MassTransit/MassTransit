namespace MassTransit.Tests
{
    using System;
    using System.Threading.Tasks;
    using NUnit.Framework;
    using TestFramework;
    using TestFramework.Messages;


    [TestFixture]
    public class Configuring_a_filter_on_a_consumer_factory :
        InMemoryTestFixture
    {
        [Test]
        public async Task Should_invoke_the_filter()
        {
            await _requestClient.GetResponse<PongMessage>(new PingMessage());
        }

        IRequestClient<PingMessage> _requestClient;

        [OneTimeSetUp]
        public void Setup()
        {
            _requestClient = CreateRequestClient<PingMessage>();
        }

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            base.ConfigureInMemoryReceiveEndpoint(configurator);

            configurator.Consumer<Consumer>(x =>
            {
                x.UseFilter(new UnitOfWorkFilter<Consumer>());
            });
        }


        class Consumer :
            IConsumer<PingMessage>
        {
            public async Task Consume(ConsumeContext<PingMessage> context)
            {
                var transaction = context.GetPayload<IUnitOfWork>();

                Console.WriteLine("Using transaction");

                await context.RespondAsync(new PongMessage(context.Message.CorrelationId));
            }
        }


        class UnitOfWorkFilter<T> :
            IFilter<ConsumerConsumeContext<T>>
            where T : class
        {
            public async Task Send(ConsumerConsumeContext<T> context, IPipe<ConsumerConsumeContext<T>> next)
            {
                var unitOfWork = context.GetOrAddPayload<IUnitOfWork>(() => new UnitOfWork());

                try
                {
                    await next.Send(context).ConfigureAwait(false);

                    unitOfWork.Commit();
                }
                catch
                {
                    unitOfWork.Abandon();
                    throw;
                }
            }

            public void Probe(ProbeContext context)
            {
                context.CreateFilterScope("unitOfWork");
            }
        }


        class UnitOfWork :
            IUnitOfWork
        {
            public UnitOfWork()
            {
                Transaction = new Transaction();
            }

            public ITransaction Transaction { get; }

            public void Abandon()
            {
                Console.WriteLine("Abandoning Work");
            }

            public void Commit()
            {
                Console.WriteLine("Committing Work");
            }
        }


        interface IUnitOfWork
        {
            ITransaction Transaction { get; }

            void Abandon();
            void Commit();
        }


        interface ITransaction
        {
        }


        class Transaction :
            ITransaction
        {
        }
    }
}
