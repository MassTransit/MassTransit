namespace MassTransit.Courier.Pipeline
{
    using System;
    using System.Threading.Tasks;
    using GreenPipes;
    using Util;


    /// <summary>
    /// Merges the out-of-band consumer back into the pipe
    /// </summary>
    /// <typeparam name="TActivity"></typeparam>
    /// <typeparam name="TLog"></typeparam>
    public class CompensateActivityMergePipe<TActivity, TLog> :
        IPipe<CompensateActivityContext<TLog>>
        where TActivity : class, CompensateActivity<TLog>
        where TLog : class
    {
        readonly IPipe<CompensateActivityContext<TActivity, TLog>> _output;

        public CompensateActivityMergePipe(IPipe<CompensateActivityContext<TActivity, TLog>> output)
        {
            _output = output;
        }

        void IProbeSite.Probe(ProbeContext context)
        {
            var scope = context.CreateFilterScope("merge");
            scope.Add("activityType", TypeMetadataCache<TActivity>.ShortName);
            scope.Add("logType", TypeMetadataCache<TLog>.ShortName);

            _output.Probe(scope);
        }

        public Task Send(CompensateActivityContext<TLog> context)
        {
            if (context is CompensateActivityContext<TActivity, TLog> compensateActivityContext)
                return _output.Send(compensateActivityContext);

            throw new ArgumentException($"THe context could not be retrieved: {TypeMetadataCache<TActivity>.ShortName}", nameof(context));
        }
    }
}
