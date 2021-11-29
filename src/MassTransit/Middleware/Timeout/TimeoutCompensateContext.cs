namespace MassTransit.Middleware.Timeout
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using Context;


    public class TimeoutCompensateContext<TLog> :
        TimeoutCourierContextProxy,
        CompensateContext<TLog>
        where TLog : class
    {
        readonly CompensateContext<TLog> _context;

        public TimeoutCompensateContext(CompensateContext<TLog> context, CancellationToken cancellationToken)
            : base(context, cancellationToken)
        {
            _context = context;
        }

        CompensationResult CompensateContext.Compensated()
        {
            return _context.Compensated();
        }

        CompensationResult CompensateContext.Compensated(object values)
        {
            return _context.Compensated(values);
        }

        CompensationResult CompensateContext.Compensated(IDictionary<string, object> variables)
        {
            return _context.Compensated(variables);
        }

        CompensationResult CompensateContext.Failed()
        {
            return _context.Failed();
        }

        CompensationResult CompensateContext.Failed(Exception exception)
        {
            return _context.Failed(exception);
        }

        CompensationResult CompensateContext.Result
        {
            get => _context.Result;
            set => _context.Result = value;
        }

        TLog CompensateContext<TLog>.Log => _context.Log;

        CompensateActivityContext<TActivity, TLog> CompensateContext<TLog>.CreateActivityContext<TActivity>(TActivity activity)
        {
            return new HostCompensateActivityContext<TActivity, TLog>(activity, this);
        }
    }
}
