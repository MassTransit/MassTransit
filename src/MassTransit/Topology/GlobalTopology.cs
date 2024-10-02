namespace MassTransit
{
    using System;
    using System.Collections.Generic;
    using Configuration;
    using Contracts.JobService;
    using Courier.Contracts;
    using Topology;


    /// <summary>
    /// This represents the global topology configuration, which is delegated to by
    /// all topology instances, unless for some radical reason a bus is configured
    /// without any topology delegation.
    /// YES, I hate globals, but they are serving a purpose in that these are really
    /// just defining the default behavior of message types, rather than actually
    /// behaving like the nasty evil global variables.
    /// </summary>
    public class GlobalTopology :
        IGlobalTopology
    {
        readonly HashSet<Type> _notConsumableMessageTypes;
        readonly IPublishTopologyConfigurator _publish;
        readonly ConnectHandle _publishToSendHandle;
        readonly ISendTopologyConfigurator _send;

        GlobalTopology()
        {
            _send = new SendTopology();
            _send.TryAddConvention(new CorrelationIdSendTopologyConvention());

            _publish = new PublishTopology();

            _notConsumableMessageTypes = new HashSet<Type>();

            var observer = new PublishToSendTopologyConfigurationObserver(_send);
            _publishToSendHandle = _publish.ConnectPublishTopologyConfigurationObserver(observer);

            ConfigureRoutingSlipCorrelation();
            ConfigureJobSagaCorrelation();
        }

        public static ISendTopologyConfigurator Send => Cached.Metadata.Value.Send;
        public static IPublishTopologyConfigurator Publish => Cached.Metadata.Value.Publish;

        void IGlobalTopology.SeparatePublishFromSend()
        {
            _publishToSendHandle.Disconnect();
        }

        ISendTopologyConfigurator IGlobalTopology.Send => _send;
        IPublishTopologyConfigurator IGlobalTopology.Publish => _publish;

        bool IGlobalTopology.IsConsumableMessageType(Type type)
        {
            lock (_notConsumableMessageTypes)
            {
                if (_notConsumableMessageTypes.Contains(type))
                    return false;
            }

            return true;
        }

        void IGlobalTopology.MarkMessageTypeNotConsumable(Type type)
        {
            lock (_notConsumableMessageTypes)
                _notConsumableMessageTypes.Add(type);
        }

        /// <summary>
        /// Mark the specified message type such that it will not be configured by the consume topology,
        /// and therefore not bound/subscribed on the message broker.
        /// </summary>
        /// <param name="type"></param>
        public static void MarkMessageTypeNotConsumable(Type type)
        {
            Cached.Metadata.Value.MarkMessageTypeNotConsumable(type);
        }

        public static bool IsConsumableMessageType(Type type)
        {
            return Cached.Metadata.Value.IsConsumableMessageType(type);
        }

        /// <summary>
        /// Call before configuring any topology, so that publish is handled separately
        /// from send. Note, this can cause some really bad things to happen with internal
        /// types so use with caution...
        /// </summary>
        public static void SeparatePublishFromSend()
        {
            Cached.Metadata.Value.SeparatePublishFromSend();
        }

        void ConfigureRoutingSlipCorrelation()
        {
            _send.UseCorrelationId<RoutingSlip>(x => x.TrackingNumber);
            _send.UseCorrelationId<RoutingSlipCompleted>(x => x.TrackingNumber);
            _send.UseCorrelationId<RoutingSlipFaulted>(x => x.TrackingNumber);
            _send.UseCorrelationId<RoutingSlipActivityCompleted>(x => x.ExecutionId);
            _send.UseCorrelationId<RoutingSlipActivityFaulted>(x => x.ExecutionId);
            _send.UseCorrelationId<RoutingSlipActivityCompensated>(x => x.ExecutionId);
            _send.UseCorrelationId<RoutingSlipActivityCompensationFailed>(x => x.ExecutionId);
            _send.UseCorrelationId<RoutingSlipCompensationFailed>(x => x.TrackingNumber);
            _send.UseCorrelationId<RoutingSlipTerminated>(x => x.TrackingNumber);
            _send.UseCorrelationId<RoutingSlipRevised>(x => x.TrackingNumber);
        }

        void ConfigureJobSagaCorrelation()
        {
            _send.UseCorrelationId<AllocateJobSlot>(x => x.JobTypeId);
            _send.UseCorrelationId<JobSlotReleased>(x => x.JobTypeId);
            _send.UseCorrelationId<SetConcurrentJobLimit>(x => x.JobTypeId);

            _send.UseCorrelationId<CancelJob>(x => x.JobId);
            _send.UseCorrelationId<Fault<AllocateJobSlot>>(x => x.Message.JobId);
            _send.UseCorrelationId<Fault<StartJobAttempt>>(x => x.Message.JobId);
            _send.UseCorrelationId<FinalizeJob>(x => x.JobId);
            _send.UseCorrelationId<GetJobState>(x => x.JobId);
            _send.UseCorrelationId<JobAttemptCanceled>(x => x.JobId);
            _send.UseCorrelationId<JobAttemptCompleted>(x => x.JobId);
            _send.UseCorrelationId<JobAttemptFaulted>(x => x.JobId);
            _send.UseCorrelationId<JobAttemptStarted>(x => x.JobId);
            _send.UseCorrelationId<JobCanceled>(x => x.JobId);
            _send.UseCorrelationId<JobCompleted>(x => x.JobId);
            _send.UseCorrelationId<JobRetryDelayElapsed>(x => x.JobId);
            _send.UseCorrelationId<JobSlotAllocated>(x => x.JobId);
            _send.UseCorrelationId<JobSlotUnavailable>(x => x.JobId);
            _send.UseCorrelationId<JobSlotWaitElapsed>(x => x.JobId);
            _send.UseCorrelationId<JobSubmitted>(x => x.JobId);
            _send.UseCorrelationId<RetryJob>(x => x.JobId);
            _send.UseCorrelationId<RunJob>(x => x.JobId);
            _send.UseCorrelationId<SaveJobState>(x => x.JobId);
            _send.UseCorrelationId<SetJobProgress>(x => x.JobId);
            _send.UseCorrelationId<StartJob>(x => x.JobId);

            _send.UseCorrelationId<StartJobAttempt>(x => x.AttemptId);
            _send.UseCorrelationId<FinalizeJobAttempt>(x => x.AttemptId);
            _send.UseCorrelationId<CancelJobAttempt>(x => x.AttemptId);
            _send.UseCorrelationId<Fault<StartJob>>(x => x.Message.AttemptId);
            _send.UseCorrelationId<JobAttemptStatus>(x => x.AttemptId);
            _send.UseCorrelationId<JobStatusCheckRequested>(x => x.AttemptId);
        }


        static class Cached
        {
            internal static readonly Lazy<IGlobalTopology> Metadata = new(() => new GlobalTopology());
        }
    }
}
