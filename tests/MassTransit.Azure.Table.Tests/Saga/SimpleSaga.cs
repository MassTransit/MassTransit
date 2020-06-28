namespace MassTransit.Azure.Table.Tests.Saga
{
    using System;
    using System.Threading.Tasks;
    using MassTransit.Saga;


    public class SimpleSaga :
        InitiatedBy<InitiateSimpleSaga>,
        Orchestrates<CompleteSimpleSaga>,
        ISaga
    {
        public bool Moved { get; private set; }
        public bool Initiated { get; private set; }
        public bool Observed { get; private set; }
        public string Name { get; private set; }

        public async Task Consume(ConsumeContext<InitiateSimpleSaga> context)
        {
            Initiated = true;
            Name = context.Message.Name;
        }

        public Guid CorrelationId { get; set; }

        public async Task Consume(ConsumeContext<CompleteSimpleSaga> message)
        {
            Moved = true;
        }
    }
}
