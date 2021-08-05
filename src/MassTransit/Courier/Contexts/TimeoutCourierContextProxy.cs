namespace MassTransit.Courier.Contexts
{
    using System;
    using System.Threading;
    using Context;
    using Contracts;


    public abstract class TimeoutCourierContextProxy :
        TimeoutConsumeContext<RoutingSlip>,
        CourierContext
    {
        readonly CourierContext _courierContext;

        protected TimeoutCourierContextProxy(CourierContext courierContext, CancellationToken cancellationToken)
            : base(courierContext, cancellationToken)
        {
            _courierContext = courierContext;
        }

        DateTime CourierContext.Timestamp => _courierContext.Timestamp;
        TimeSpan CourierContext.Elapsed => _courierContext.Elapsed;
        Guid CourierContext.TrackingNumber => _courierContext.TrackingNumber;
        Guid CourierContext.ExecutionId => _courierContext.ExecutionId;
        string CourierContext.ActivityName => _courierContext.ActivityName;
    }
}
