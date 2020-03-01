namespace MassTransit.Azure.ServiceBus.Core.Tests.TurningItOut
{
    using System;
    using System.Threading.Tasks;
    using MassTransit.Saga;
    using NUnit.Framework;
    using Saga;


    [TestFixture]
    [Explicit]
    public class A_successful_job_using_turnout :
        AzureServiceBusTestFixture
    {
        [Test]
        public async Task Should_result_in_complete_state_instance()
        {
            var endpoint = await Bus.GetSendEndpoint(_serviceAddress);

            await endpoint.Send(new LongTimeJob
            {
                Id = "FIRST",
                Duration = TimeSpan.FromSeconds(10)
            });

            ConsumeContext<TurnoutJobCompleted> consumeContext = await _handler;
        }

        Uri _serviceAddress;
        ISagaRepository<TurnoutJobState> _repository;
        TurnoutJobStateMachine _stateMachine;
        Task<ConsumeContext<TurnoutJobCompleted>> _handler;

        protected override void ConfigureServiceBusReceiveEndpoint(IServiceBusReceiveEndpointConfigurator configurator)
        {
            _handler = Handled<TurnoutJobCompleted>(configurator);
        }

        protected override void ConfigureServiceBusBusHost(IServiceBusBusFactoryConfigurator configurator, IServiceBusHost host)
        {
            configurator.UseServiceBusMessageScheduler();

            base.ConfigureServiceBusBusHost(configurator, host);

            configurator.TurnoutEndpoint<LongTimeJob>("service_queue", endpoint =>
            {
                endpoint.SuperviseInterval = TimeSpan.FromSeconds(1);
                endpoint.SetJobFactory(async context =>
                {
                    Console.WriteLine("Starting job: {0}", context.Command.Id);

                    await Task.Delay(context.Command.Duration).ConfigureAwait(false);

                    Console.WriteLine("Finished job: {0}", context.Command.Id);
                });

                _serviceAddress = endpoint.InputAddress;
            });

            _stateMachine = new TurnoutJobStateMachine();
            _repository = new MessageSessionSagaRepository<TurnoutJobState>();


            configurator.ReceiveEndpoint(host, "service_state", endpoint =>
            {
                endpoint.RequiresSession = true;
                endpoint.MessageWaitTimeout = TimeSpan.FromHours(8);

                endpoint.StateMachineSaga(_stateMachine, _repository);
            });
        }
    }
}
