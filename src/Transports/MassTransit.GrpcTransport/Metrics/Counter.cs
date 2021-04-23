namespace MassTransit.GrpcTransport.Metrics
{
    using System.Threading;


    public class Counter :
        Metric
    {
        long _count;

        public void Add()
        {
            Interlocked.Increment(ref _count);
        }
    }
}
