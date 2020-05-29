namespace MassTransit.Tests.Saga
{
    using System;
    using System.Threading.Tasks;
    using MassTransit.Saga;
    using Messages;


    public class InjectingSampleSaga :
        InitiatedBy<InitiateSimpleSaga>,
        ISaga
    {
        public InjectingSampleSaga()
        {
        }

        public InjectingSampleSaga(Guid correlationId)
        {
            CorrelationId = correlationId;
        }

        public string Name { get; private set; }
        public IDependency Dependency { get; set; }

        public async Task Consume(ConsumeContext<InitiateSimpleSaga> context)
        {
            Name = context.Message.Name;
        }

        public Guid CorrelationId { get; set; }
    }


    public interface IDependency
    {
    }
}
