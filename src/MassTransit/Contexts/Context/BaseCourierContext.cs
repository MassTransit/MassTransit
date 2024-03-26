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

            RoutingSlip = new SanitizedRoutingSlip(consumeContext);

            // ReSharper disable once VirtualMemberCallInConstructor
            Publisher = new RoutingSlipEventPublisher(this, RoutingSlip, CancellationToken);
        }

        protected IRoutingSlipEventPublisher Publisher { get; }
        protected SanitizedRoutingSlip RoutingSlip { get; }

        DateTime CourierContext.Timestamp => _timestamp;
        TimeSpan CourierContext.Elapsed => _timer.Elapsed;
        Guid CourierContext.TrackingNumber => RoutingSlip.TrackingNumber;
        Guid CourierContext.ExecutionId => _executionId;

        RoutingSlip ConsumeContext<RoutingSlip>.Message => RoutingSlip;

        public abstract string ActivityName { get; }

        public void Method4()
        {
        }

        public void Method5()
        {
        }

        public void Method6()
        {
        }
    }
}
