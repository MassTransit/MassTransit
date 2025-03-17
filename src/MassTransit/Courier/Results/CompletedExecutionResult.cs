namespace MassTransit.Courier.Results
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Contracts;
    using Middleware;


    class CompletedExecutionResult<TArguments> :
        BaseExecutionResult<TArguments>,
        CompletedActivityOptions
        where TArguments : class
    {
        readonly Uri _compensationAddress;
        IDictionary<string, object> _data;

        public CompletedExecutionResult(ExecuteContext<TArguments> context, IRoutingSlipEventPublisher publisher, Activity activity, RoutingSlip routingSlip,
            Uri compensationAddress)
            : base(context, publisher, activity, routingSlip)
        {
            _compensationAddress = compensationAddress;
        }

        public void SetLog<TLog>(TLog log)
            where TLog : class
        {
            if (log == null)
                return;

            IEnumerable<KeyValuePair<string, object>> dictionary = Context.SerializerContext.ToDictionary(log);
            SetLog(dictionary);
        }

        public void SetLog(object values)
        {
            IEnumerable<KeyValuePair<string, object>> objectAsDictionary = Context.SerializerContext.ToDictionary(values);
            SetLog(objectAsDictionary);
        }

        public void SetLog(IEnumerable<KeyValuePair<string, object>> values)
        {
            _data ??= new Dictionary<string, object>();
            foreach (KeyValuePair<string, object> value in values)
                _data[value.Key] = value.Value;
        }

        public override async Task Evaluate()
        {
            var builder = CreateRoutingSlipBuilder(RoutingSlip);

            Build(builder);

            var routingSlip = builder.Build();

            await PublishActivityEvents(routingSlip, builder).ConfigureAwait(false);

            if (HasNextActivity(routingSlip))
            {
                if (Delay.HasValue)
                {
                    void AddForwarderAddress(ConsumeContext consumeContext, SendContext sendContext)
                    {
                        var forwarderAddress = consumeContext.ReceiveContext.InputAddress ?? consumeContext.DestinationAddress;
                        if (forwarderAddress != null && forwarderAddress != Context.DestinationAddress)
                            sendContext.Headers.Set(MessageHeaders.ForwarderAddress, forwarderAddress.ToString());
                    }

                    await Context.ScheduleSend(routingSlip.GetNextExecuteAddress(), Delay.Value, routingSlip, new CopyContextPipe(Context, AddForwarderAddress))
                        .ConfigureAwait(false);
                }
                else
                {
                    var endpoint = await Context.GetSendEndpoint(routingSlip.GetNextExecuteAddress()).ConfigureAwait(false);

                    await Context.Forward(endpoint, routingSlip).ConfigureAwait(false);
                }
            }
            else
            {
                var completedTimestamp = Context.Timestamp + Duration;
                var completedDuration = completedTimestamp - RoutingSlip.CreateTimestamp;

                await Publisher.PublishRoutingSlipCompleted(completedTimestamp, completedDuration, routingSlip.Variables).ConfigureAwait(false);
            }
        }

        protected virtual Task PublishActivityEvents(RoutingSlip routingSlip, RoutingSlipBuilder builder)
        {
            return Publisher.PublishRoutingSlipActivityCompleted(Context.ActivityName, Context.ExecutionId, Context.Timestamp, Duration,
                routingSlip.Variables, Activity.Arguments, _data);
        }

        static bool HasNextActivity(RoutingSlip routingSlip)
        {
            return routingSlip.Itinerary.Any();
        }

        protected virtual void Build(RoutingSlipBuilder builder)
        {
            builder.AddActivityLog(Context.Host, Activity.Name, Context.ExecutionId, Context.Timestamp, Duration);

            if (Variables?.Any() ?? false)
                builder.SetVariables(Variables);

            if (_compensationAddress != null && _data is not null && _data.Count > 0)
                builder.AddCompensateLog(Context.ExecutionId, _compensationAddress, _data);
        }

        protected virtual RoutingSlipBuilder CreateRoutingSlipBuilder(RoutingSlip routingSlip)
        {
            return new RoutingSlipBuilder(routingSlip, routingSlip.Itinerary.Skip(1), []);
        }
    }
}
