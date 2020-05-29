namespace MassTransit.Context
{
    using System;
    using Events;


    public class RescueExceptionReceiveContext :
        ReceiveContextProxy,
        ExceptionReceiveContext
    {
        readonly Exception _exception;
        readonly DateTime _exceptionTimestamp;
        ExceptionInfo _exceptionInfo;

        public RescueExceptionReceiveContext(ReceiveContext context, Exception exception)
            : base(context)
        {
            _exception = exception;

            _exceptionTimestamp = DateTime.UtcNow;
        }

        Exception ExceptionReceiveContext.Exception => _exception;
        DateTime ExceptionReceiveContext.ExceptionTimestamp => _exceptionTimestamp;

        ExceptionInfo ExceptionReceiveContext.ExceptionInfo
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
