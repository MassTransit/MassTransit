namespace MassTransit.Configuration
{
    using System.Collections.Generic;
    using Middleware;


    public class DeadLetterPipeSpecification :
        IPipeSpecification<ReceiveContext>
    {
        readonly IPipe<ReceiveContext> _deadLetterPipe;

        public DeadLetterPipeSpecification(IPipe<ReceiveContext> deadLetterPipe)
        {
            _deadLetterPipe = deadLetterPipe;
        }

        public void Apply(IPipeBuilder<ReceiveContext> builder)
        {
            builder.AddFilter(new DeadLetterFilter(_deadLetterPipe));
        }

        public IEnumerable<ValidationResult> Validate()
        {
            if (_deadLetterPipe == null)
                yield return this.Failure("RescuePipe", "must not be null");
        }
    }
}
