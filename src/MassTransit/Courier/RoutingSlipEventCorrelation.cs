namespace MassTransit.Courier
{
    using Contracts;


    public static class RoutingSlipEventCorrelation
    {
        static readonly object _lock = new object();
        static bool _configured;

        public static void ConfigureCorrelationIds()
        {
            lock (_lock)
            {
                if (_configured)
                    return;

                ConfigureCorrelationIds(GlobalTopology.Send);

                _configured = true;
            }
        }

        public static void ConfigureCorrelationIds(ISendTopology topology)
        {
            topology.UseCorrelationId<RoutingSlipCompleted>(x => x.TrackingNumber);
            topology.UseCorrelationId<RoutingSlipFaulted>(x => x.TrackingNumber);
            topology.UseCorrelationId<RoutingSlipActivityCompleted>(x => x.ExecutionId);
            topology.UseCorrelationId<RoutingSlipActivityFaulted>(x => x.ExecutionId);
            topology.UseCorrelationId<RoutingSlipActivityCompensated>(x => x.ExecutionId);
            topology.UseCorrelationId<RoutingSlipActivityCompensationFailed>(x => x.ExecutionId);
            topology.UseCorrelationId<RoutingSlipCompensationFailed>(x => x.TrackingNumber);
            topology.UseCorrelationId<RoutingSlipTerminated>(x => x.TrackingNumber);
            topology.UseCorrelationId<RoutingSlipRevised>(x => x.TrackingNumber);
        }
    }
}
