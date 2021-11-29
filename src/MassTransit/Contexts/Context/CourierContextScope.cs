namespace MassTransit.Context
{
    using System;
    using Courier.Contracts;


    public abstract class CourierContextScope :
        ConsumeContextScope<RoutingSlip>,
        CourierContext
    {
        readonly CourierContext _courierContext;

        protected CourierContextScope(CourierContext courierContext, params object[] payloads)
            : base(courierContext, payloads)
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
