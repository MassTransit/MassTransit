namespace MassTransit.RedisIntegration.Tests
{
    using System;
    using System.Threading.Tasks;


    public class SimpleSaga :
        InitiatedBy<InitiateSimpleSaga>,
        Orchestrates<CompleteSimpleSaga>,
        ISagaVersion
    {
        public bool Moved { get; set; }
        public bool Initiated { get; set; }
        public bool Observed { get; set; }
        public string Name { get; set; }

        public async Task Consume(ConsumeContext<InitiateSimpleSaga> context)
        {
            Initiated = true;
            Name = context.Message.Name;
        }

        public Guid CorrelationId { get; set; }
        public int Version { get; set; }

        public async Task Consume(ConsumeContext<CompleteSimpleSaga> message)
        {
            Moved = true;
        }
    }
}
