namespace MassTransit
{
    using Courier.Contracts;


    public class RoutingSlipRequestFaultedException :
        RoutingSlipException
    {
        public RoutingSlipRequestFaultedException(RoutingSlipFaulted faulted)
            : base("The routing slip request faulted")
        {
            Faulted = faulted;
        }

        public RoutingSlipFaulted Faulted { get; }
    }
}
