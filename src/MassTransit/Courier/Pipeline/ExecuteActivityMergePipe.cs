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
    /// <typeparam name="TArguments"></typeparam>
    public class ExecuteActivityMergePipe<TActivity, TArguments> :
        IPipe<ExecuteActivityContext<TArguments>>
        where TActivity : class, ExecuteActivity<TArguments>
        where TArguments : class
    {
        readonly IPipe<ExecuteActivityContext<TActivity, TArguments>> _output;

        public ExecuteActivityMergePipe(IPipe<ExecuteActivityContext<TActivity, TArguments>> output)
        {
            _output = output;
        }

        void IProbeSite.Probe(ProbeContext context)
        {
            var scope = context.CreateFilterScope("merge");
            scope.Add("activityType", TypeMetadataCache<TActivity>.ShortName);
            scope.Add("argumentType", TypeMetadataCache<TArguments>.ShortName);

            _output.Probe(scope);
        }

        public Task Send(ExecuteActivityContext<TArguments> context)
        {
            if (context is ExecuteActivityContext<TActivity, TArguments> executeActivityContext)
                return _output.Send(executeActivityContext);

            throw new ArgumentException($"THe context could not be retrieved: {TypeMetadataCache<TActivity>.ShortName}", nameof(context));
        }
    }
}
