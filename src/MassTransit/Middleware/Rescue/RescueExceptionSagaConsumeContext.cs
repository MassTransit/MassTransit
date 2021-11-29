namespace MassTransit.Middleware.Rescue
{
    using System;
    using System.Threading.Tasks;
    using Context;
    using Events;


    public class RescueExceptionSagaConsumeContext<TSaga> :
        ConsumeContextProxy,
        ExceptionSagaConsumeContext<TSaga>
        where TSaga : class, ISaga
    {
        readonly SagaConsumeContext<TSaga> _context;
        ExceptionInfo _exceptionInfo;

        public RescueExceptionSagaConsumeContext(SagaConsumeContext<TSaga> context, Exception exception)
            : base(context)
        {
            _context = context;
            Exception = exception;
        }

        public TSaga Saga => _context.Saga;

        public Task SetCompleted()
        {
            return _context.SetCompleted();
        }

        public bool IsCompleted => _context.IsCompleted;

        public Exception Exception { get; }

        public ExceptionInfo ExceptionInfo
        {
            get { return _exceptionInfo ??= new FaultExceptionInfo(Exception); }
        }
    }
}
