namespace MassTransit.Context
{
    using System;
    using System.Diagnostics;
    using Courier;
    using Courier.Contracts;


    public abstract class BaseCourierContext :
        ConsumeContextScope<RoutingSlip>,
        CourierContext
    {
        readonly Guid _executionId;
        readonly Stopwatch _timer;
        readonly DateTime _timestamp;

        protected BaseCourierContext(ConsumeContext<RoutingSlip> consumeContext)
            : base(consumeContext)
        {
            if (consumeContext == null)
                throw new ArgumentNullException(nameof(consumeContext));

            _timer = Stopwatch.StartNew();
            var newId = NewId.Next();

            _executionId = newId.ToGuid();
            _timestamp = newId.Timestamp;

            // TODO move this to the deserializer! One for JSON, one for SystemTextJson
            RoutingSlip = new SanitizedRoutingSlip(consumeContext);

            Publisher = new RoutingSlipEventPublisher(this, RoutingSlip);
        }

        protected IRoutingSlipEventPublisher Publisher { get; }
        protected SanitizedRoutingSlip RoutingSlip { get; }

        DateTime CourierContext.Timestamp => _timestamp;
        TimeSpan CourierContext.Elapsed => _timer.Elapsed;
        Guid CourierContext.TrackingNumber => RoutingSlip.TrackingNumber;
        Guid CourierContext.ExecutionId => _executionId;

        RoutingSlip ConsumeContext<RoutingSlip>.Message => RoutingSlip;

        public abstract string ActivityName { get; }
    }
}
