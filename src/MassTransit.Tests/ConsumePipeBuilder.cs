namespace MassTransit.Tests
{
    using System;
    using Context.Converters;
    using GreenPipes;
    using GreenPipes.Builders;
    using GreenPipes.Filters;
    using MassTransit.Pipeline;
    using MassTransit.Pipeline.Pipes;


    public class ConsumePipeBuilder :
        IConsumePipeBuilder
    {
        readonly DynamicFilter<ConsumeContext, Guid> _filter;
        readonly PipeBuilder<ConsumeContext> _pipeBuilder;

        public ConsumePipeBuilder()
        {
            _filter = new DynamicFilter<ConsumeContext, Guid>(new ConsumeContextConverterFactory(), GetRequestId);
            _pipeBuilder = new PipeBuilder<ConsumeContext>();
        }

        void IPipeBuilder<ConsumeContext>.AddFilter(IFilter<ConsumeContext> filter)
        {
            _pipeBuilder.AddFilter(filter);
        }

        public IConsumePipe Build()
        {
            _pipeBuilder.AddFilter(_filter);

            return new ConsumePipe(_filter, _pipeBuilder.Build(), true);
        }

        static Guid GetRequestId(ConsumeContext context)
        {
            return context.RequestId ?? Guid.Empty;
        }
    }
}
