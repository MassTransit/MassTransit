namespace MassTransit.Courier
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Context;
    using Contracts;
    using Results;


    public class HostCompensateContext<TLog> :
        BaseCourierContext,
        CompensateContext<TLog>
        where TLog : class
    {
        readonly ActivityLog _activityLog;
        readonly CompensateLog _compensateLog;

        public HostCompensateContext(ConsumeContext<RoutingSlip> context)
            : base(context)
        {
            if (RoutingSlip.CompensateLogs.Count == 0)
                throw new ArgumentException("The routingSlip must contain at least one activity log");

            _compensateLog = RoutingSlip.CompensateLogs.Last();

            _activityLog = RoutingSlip.ActivityLogs.SingleOrDefault(x => x.ExecutionId == _compensateLog.ExecutionId);
            if (_activityLog == null)
            {
                throw new RoutingSlipException("The compensation log did not have a matching activity log entry: "
                    + _compensateLog.ExecutionId);
            }

            Log = RoutingSlip.GetCompensateLogData<TLog>();
        }

        public override string ActivityName => _activityLog.Name;
        public TLog Log { get; }

        public CompensateActivityContext<TActivity, TLog> CreateActivityContext<TActivity>(TActivity activity)
            where TActivity : class, ICompensateActivity<TLog>
        {
            return new HostCompensateActivityContext<TActivity, TLog>(activity, this);
        }

        public CompensationResult Result { get; set; }

        CompensationResult CompensateContext.Compensated()
        {
            return new CompensatedCompensationResult<TLog>(this, Publisher, _compensateLog, RoutingSlip);
        }

        CompensationResult CompensateContext.Compensated(object values)
        {
            if (values == null)
                throw new ArgumentNullException(nameof(values));

            return new CompensatedWithVariablesCompensationResult<TLog>(this, Publisher, _compensateLog, RoutingSlip,
                RoutingSlipBuilder.GetObjectAsDictionary(values));
        }

        CompensationResult CompensateContext.Compensated(IDictionary<string, object> variables)
        {
            if (variables == null)
                throw new ArgumentNullException(nameof(variables));

            return new CompensatedWithVariablesCompensationResult<TLog>(this, Publisher, _compensateLog, RoutingSlip, variables);
        }

        CompensationResult CompensateContext.Failed()
        {
            return Failed(new RoutingSlipException("The routing slip compensation failed"));
        }

        public CompensationResult Failed(Exception exception)
        {
            return new FailedCompensationResult<TLog>(this, Publisher, _compensateLog, RoutingSlip, exception);
        }
    }
}
