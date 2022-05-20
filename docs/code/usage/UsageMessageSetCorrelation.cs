namespace UsageMessageSetCorrelation
{
    using UsageContracts;
    using MassTransit;


    public class Program
    {
        public static void Main()
        {
            // Use the OrderId as the message CorrelationId
            GlobalTopology.Send.UseCorrelationId<SubmitOrder>(x => x.OrderId);

            // Previous approach, which now calls the new way above
            MessageCorrelation.UseCorrelationId<SubmitOrder>(x => x.OrderId);
        }
    }
}
