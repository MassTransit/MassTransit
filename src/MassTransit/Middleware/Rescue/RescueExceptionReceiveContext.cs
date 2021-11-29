namespace MassTransit.Middleware.Rescue
{
    using System;
    using Context;
    using Events;


    public class RescueExceptionReceiveContext :
        ReceiveContextProxy,
        ExceptionReceiveContext
    {
        ExceptionInfo _exceptionInfo;

        public RescueExceptionReceiveContext(ReceiveContext context, Exception exception)
            : base(context)
        {
            Exception = exception;
            ExceptionTimestamp = DateTime.UtcNow;
        }

        public Exception Exception { get; }
        public DateTime ExceptionTimestamp { get; }

        public ExceptionInfo ExceptionInfo
        {
            get { return _exceptionInfo ??= new FaultExceptionInfo(Exception); }
        }
    }
}
