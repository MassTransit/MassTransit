namespace MassTransit.Context
{
    using System;
    using System.Threading.Tasks;
    using Events;
    using Saga;


    public class RescueExceptionSagaConsumeContext<TSaga> :
        ConsumeContextProxy,
        ExceptionSagaConsumeContext<TSaga>
        where TSaga : class, ISaga
    {
        readonly SagaConsumeContext<TSaga> _context;
        readonly Exception _exception;
        ExceptionInfo _exceptionInfo;

        public RescueExceptionSagaConsumeContext(SagaConsumeContext<TSaga> context, Exception exception)
            : base(context)
        {
            _context = context;
            _exception = exception;
        }

        public TSaga Saga => _context.Saga;

        public Task SetCompleted()
        {
            return _context.SetCompleted();
        }

        public bool IsCompleted => _context.IsCompleted;

        Exception ExceptionSagaConsumeContext<TSaga>.Exception => _exception;

        ExceptionInfo ExceptionSagaConsumeContext<TSaga>.ExceptionInfo
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
