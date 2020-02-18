namespace MassTransit.Context
{
    using System;
    using Events;


    public class RescueExceptionConsumerConsumeContext<TConsumer> :
        ConsumeContextProxy,
        ExceptionConsumerConsumeContext<TConsumer>
        where TConsumer : class
    {
        readonly ConsumerConsumeContext<TConsumer> _context;
        readonly Exception _exception;
        ExceptionInfo _exceptionInfo;

        public RescueExceptionConsumerConsumeContext(ConsumerConsumeContext<TConsumer> context, Exception exception)
            : base(context)
        {
            _context = context;
            _exception = exception;
        }

        public TConsumer Consumer => _context.Consumer;

        Exception ExceptionConsumerConsumeContext<TConsumer>.Exception => _exception;

        ExceptionInfo ExceptionConsumerConsumeContext<TConsumer>.ExceptionInfo
        {
            get
            {
                if (_exceptionInfo != null)
                    return _exceptionInfo;

                _exceptionInfo = new FaultExceptionInfo(_exception);

                return _exceptionInfo;
            }
        }
    }
}
