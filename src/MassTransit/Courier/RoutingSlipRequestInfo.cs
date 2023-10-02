#nullable enable
namespace MassTransit.Courier
{
    using System;
    using System.Collections.Generic;


    public readonly struct RoutingSlipRequestInfo<T>
        where T : class
    {
        public readonly Guid RequestId;
        public readonly Uri ResponseAddress;
        public readonly Uri? FaultAddress;
        public readonly Uri? RequestAddress;
        public readonly int? RetryAttempt;
        public readonly T Request;

        public RoutingSlipRequestInfo(IObjectDeserializer context, IDictionary<string, object> variables)
        {
            Request = context.GetValue<T>(variables, RoutingSlipRequestVariableNames.Request)
                ?? throw new ArgumentException($"Routing Slip Request variable was not found: {RoutingSlipRequestVariableNames.Request}");

            RequestId = context.GetValue<Guid>(variables, RoutingSlipRequestVariableNames.RequestId)
                ?? throw new ArgumentException($"Routing Slip RequestId variable was not found: {RoutingSlipRequestVariableNames.RequestId}");

            ResponseAddress = context.GetValue<Uri>(variables, RoutingSlipRequestVariableNames.ResponseAddress)
                ?? throw new ArgumentException($"Routing Slip ResponseAddress variable was not found: {RoutingSlipRequestVariableNames.ResponseAddress}");

            FaultAddress = context.GetValue<Uri>(variables, RoutingSlipRequestVariableNames.FaultAddress);

            RequestAddress = context.GetValue<Uri>(variables, RoutingSlipRequestVariableNames.RequestAddress);
            RetryAttempt = context.GetValue<int>(variables, RoutingSlipRequestVariableNames.RetryAttempt);
        }
    }
}
