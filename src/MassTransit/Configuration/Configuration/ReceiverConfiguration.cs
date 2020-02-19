namespace MassTransit.Configuration
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Context;
    using GreenPipes;
    using GreenPipes.Builders;
    using GreenPipes.Configurators;
    using Internals.Extensions;


    public class ReceiverConfiguration :
        EndpointConfiguration
    {
        protected readonly IBuildPipeConfigurator<ReceiveContext> ReceivePipeConfigurator;
        protected readonly IList<IReceiveEndpointSpecification> Specifications;

        protected ReceiverConfiguration(IEndpointConfiguration endpointConfiguration)
            : base(endpointConfiguration)
        {
            Specifications = new List<IReceiveEndpointSpecification>();
            ReceivePipeConfigurator = new PipeConfigurator<ReceiveContext>();

            if (LogContext.Current == null)
                LogContext.ConfigureCurrentLogContext();
        }

        public void AddEndpointSpecification(IReceiveEndpointSpecification specification)
        {
            Specifications.Add(specification);
        }

        public override IEnumerable<ValidationResult> Validate()
        {
            return base.Validate()
                .Concat(Specifications.SelectMany(x => x.Validate()));
        }

        /// <summary>
        /// Configures the error pipe to rethrow exceptions from consumers, suppresses the default error queue behavior
        /// </summary>
        protected void RethrowExceptions()
        {
            ConfigureError(pipe => pipe.UseFilter(new RethrowExceptionFilter()));
        }

        /// <summary>
        /// Configures the dead letter pipe to throw an exception, suppressing the default skipped queue behavior
        /// </summary>
        protected void ThrowOnDeadLetter()
        {
            ConfigureDeadLetter(pipe => pipe.UseFilter(new FaultDeadLetterFilter()));
        }


        class FaultDeadLetterFilter :
            IFilter<ReceiveContext>
        {
            public Task Send(ReceiveContext context, IPipe<ReceiveContext> next)
            {
                throw new MessageNotConsumedException(context.InputAddress, "The message was not consumed");
            }

            public void Probe(ProbeContext context)
            {
                context.CreateScope("fault-not-consumed");
            }
        }


        class RethrowExceptionFilter :
            IFilter<ExceptionReceiveContext>
        {
            public async Task Send(ExceptionReceiveContext context, IPipe<ExceptionReceiveContext> next)
            {
                await context.NotifyFaulted(context.Exception).ConfigureAwait(false);

                context.Exception.Rethrow();
            }

            public void Probe(ProbeContext context)
            {
                context.CreateScope("log-fault");
            }
        }
    }
}
