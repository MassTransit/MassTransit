using System;
using DistributedGrid.Shared;
using DistributedGrid.Shared.Messages;
using MassTransit;
using MassTransit.Configuration;
using MassTransit.Grid;
using MassTransit.Grid.Configuration;
using MassTransit.Grid.Paxos;
using MassTransit.Grid.Sagas;
using MassTransit.Saga;
using MassTransit.Services.Subscriptions.Configuration;

namespace DistributedGrid.Worker
{
    public class DoWork :
        IServiceInterface,
        IDisposable,
        Consumes<DoSimpleWorkItem>.All
    {
        public DoWork(IObjectBuilder objectBuilder)
        {
            ObjectBuilder = objectBuilder;

            SetupGridNodeRepository();
            SetupGridAcceptorRepository();
            SetupGridListenerRepository();
            SetupGridMessageNodeRepository();
            SetupGridServiceNodeRepository();
            SetupGridServiceRepository();

            ControlBus = ControlBusConfigurator.New(x =>
            {
                x.SetObjectBuilder(ObjectBuilder);
                
                x.ReceiveFrom(ObjectBuilder.GetInstance<String>("SourceQueue") + "_control");

                x.PurgeBeforeStarting();
            });

            DataBus = ServiceBusConfigurator.New(x =>
            {
                x.SetObjectBuilder(ObjectBuilder);
                x.ConfigureService<SubscriptionClientConfigurator>(y =>
                {
                    // setup endpoint
                    y.SetSubscriptionServiceEndpoint(ObjectBuilder.GetInstance<String>("SubscriptionQueue"));
                });
                x.ReceiveFrom(ObjectBuilder.GetInstance<String>("SourceQueue"));
                x.UseControlBus(ControlBus);
                x.SetConcurrentConsumerLimit(4);

                x.ConfigureService<GridConfigurator>(grid => 
                {
                    grid.SetProposer();
                    grid.For<DoSimpleWorkItem>().Use<DoWork>();
                });
            });
        }


        public void Start()
        {

        }

        public void Stop()
        {

        }

        public IServiceBus DataBus { get; private set; }
        public IControlBus ControlBus { get; private set; }
        public ISagaRepository<Node> GridNodeRepository { get; private set; }
        public ISagaRepository<ServiceType> GridServiceRepository { get; private set; }
        public ISagaRepository<ServiceMessage> GridMessageNodeRepository { get; private set; }
        public ISagaRepository<ServiceNode> GridServiceNodeRepository { get; private set; }
        public ISagaRepository<Learner<AvailableGridServiceNode>> GridListenerRepository { get; private set; }
        public ISagaRepository<Acceptor<AvailableGridServiceNode>> GridAcceptorRepository { get; private set; }
        public IObjectBuilder ObjectBuilder { get; private set; }

        private void SetupGridNodeRepository()
        {
            GridNodeRepository = ObjectBuilder.GetInstance(typeof(ISagaRepository<Node>)) as ISagaRepository<Node>;
        }

        private void SetupGridServiceRepository()
        {
            GridServiceRepository = ObjectBuilder.GetInstance(typeof(ISagaRepository<ServiceType>)) as ISagaRepository<ServiceType>;
        }

        private void SetupGridMessageNodeRepository()
        {
            GridMessageNodeRepository = ObjectBuilder.GetInstance(typeof(ISagaRepository<ServiceMessage>)) as ISagaRepository<ServiceMessage>;
        }

        private void SetupGridListenerRepository()
        {
            GridListenerRepository = ObjectBuilder.GetInstance(typeof(ISagaRepository<Learner<AvailableGridServiceNode>>)) as ISagaRepository<Learner<AvailableGridServiceNode>>;
        }

        private void SetupGridAcceptorRepository()
        {
            GridAcceptorRepository = ObjectBuilder.GetInstance(typeof(ISagaRepository<Acceptor<AvailableGridServiceNode>>)) as ISagaRepository<Acceptor<AvailableGridServiceNode>>;
        }

        private void SetupGridServiceNodeRepository()
        {
            GridServiceNodeRepository = ObjectBuilder.GetInstance(typeof(ISagaRepository<ServiceNode>)) as ISagaRepository<ServiceNode>;
        }

        public void Dispose()
        {
        }

        public void Consume(DoSimpleWorkItem message)
        {
            Console.WriteLine(message.CorrelationId);
            CurrentMessage.Respond(new CompletedSimpleWorkItem(message.CorrelationId, message.CreatedAt));
        }
    }
}