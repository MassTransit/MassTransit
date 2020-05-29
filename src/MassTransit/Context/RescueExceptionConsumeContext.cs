namespace MassTransit.Context
{
    using System;
    using Events;


    public class RescueExceptionConsumeContext<TMessage> :
        ConsumeContextProxy<TMessage>,
        ExceptionConsumeContext<TMessage>
        where TMessage : class
    {
        readonly Exception _exception;
        ExceptionInfo _exceptionInfo;

        public RescueExceptionConsumeContext(ConsumeContext<TMessage> context, Exception exception)
            : base(context)
        {
            _exception = exception;
        }

        Exception ExceptionConsumeContext.Exception => _exception;

        ExceptionInfo ExceptionConsumeContext.ExceptionInfo
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


    public class RescueExceptionConsumeContext :
        ConsumeContextProxy,
        ExceptionConsumeContext
    {
        readonly Exception _exception;
        ExceptionInfo _exceptionInfo;

        public RescueExceptionConsumeContext(ConsumeContext context, Exception exception)
            : base(context)
        {
            _exception = exception;
        }

        Exception ExceptionConsumeContext.Exception => _exception;

        ExceptionInfo ExceptionConsumeContext.ExceptionInfo
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
