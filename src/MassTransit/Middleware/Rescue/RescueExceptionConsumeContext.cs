namespace MassTransit.Middleware.Rescue
{
    using System;
    using Context;
    using Events;


    public class RescueExceptionConsumeContext<TMessage> :
        ConsumeContextProxy<TMessage>,
        ExceptionConsumeContext<TMessage>
        where TMessage : class
    {
        ExceptionInfo _exceptionInfo;

        public RescueExceptionConsumeContext(ConsumeContext<TMessage> context, Exception exception)
            : base(context)
        {
            Exception = exception;
        }

        public Exception Exception { get; }

        public ExceptionInfo ExceptionInfo
        {
            get { return _exceptionInfo ??= new FaultExceptionInfo(Exception); }
        }
    }


    public class RescueExceptionConsumeContext :
        ConsumeContextProxy,
        ExceptionConsumeContext
    {
        ExceptionInfo _exceptionInfo;

        public RescueExceptionConsumeContext(ConsumeContext context, Exception exception)
            : base(context)
        {
            Exception = exception;
        }

        public Exception Exception { get; }

        public ExceptionInfo ExceptionInfo
        {
            get { return _exceptionInfo ??= new FaultExceptionInfo(Exception); }
        }
    }
}
