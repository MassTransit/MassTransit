namespace MassTransit.Context
{
    using System;
    using System.Collections.Generic;


    public class CompensateContextScope<TLog> :
        CourierContextScope,
        CompensateContext<TLog>
        where TLog : class
    {
        readonly CompensateContext<TLog> _context;

        public CompensateContextScope(CompensateContext<TLog> context, params object[] payloads)
            : base(context, payloads)
        {
            _context = context;
        }

        public TLog Log => _context.Log;

        public CompensateActivityContext<TActivity, TLog> CreateActivityContext<TActivity>(TActivity activity)
            where TActivity : class, ICompensateActivity<TLog>
        {
            return new HostCompensateActivityContext<TActivity, TLog>(activity, this);
        }

        public CompensationResult Compensated()
        {
            return _context.Compensated();
        }

        public CompensationResult Compensated(object values)
        {
            return _context.Compensated(values);
        }

        public CompensationResult Compensated(IDictionary<string, object> variables)
        {
            return _context.Compensated(variables);
        }

        public CompensationResult Failed()
        {
            return _context.Failed();
        }

        public CompensationResult Failed(Exception exception)
        {
            return _context.Failed(exception);
        }

        public CompensationResult Result
        {
            get => _context.Result;
            set => _context.Result = value;
        }
    }
}
