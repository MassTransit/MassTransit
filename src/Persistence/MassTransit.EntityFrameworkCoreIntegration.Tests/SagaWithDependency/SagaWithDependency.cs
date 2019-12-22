namespace MassTransit.EntityFrameworkCoreIntegration.Tests.SagaWithDependency
{
    using System;
    using System.Threading.Tasks;
    using DataAccess;
    using MassTransit.Saga;
    using MassTransit.Tests.Saga.Messages;
    using Messages;


    public class SagaWithDependency :
        InitiatedBy<InitiateSimpleSaga>,
        Orchestrates<UpdateSagaDependency>,
        ISaga
    {
        public bool Completed { get; private set; }
        public bool Initiated { get; private set; }
        public string Name { get; private set; }

        public SagaDependency Dependency { get; set; }

        public Task Consume(ConsumeContext<InitiateSimpleSaga> context)
        {
            CorrelationId = context.Message.CorrelationId;
            Initiated = true;
            Name = context.Message.Name;
            Dependency = new SagaDependency
            {
                SagaInnerDependency = new SagaInnerDependency()
            };

            return Task.CompletedTask;
        }

        public Guid CorrelationId { get; set; }

        public Task Consume(ConsumeContext<UpdateSagaDependency> context)
        {
            Dependency.SagaInnerDependency.Name = context.Message.Name;
            Completed = true;
            return Task.CompletedTask;
        }
    }
}
