namespace MassTransit.Riders
{
    using System.Threading;
    using System.Threading.Tasks;
    using GreenPipes.Util;


    public class RiderConnectable :
        Connectable<IRider>,
        IRider
    {
        public Task Start(CancellationToken cancellationToken)
        {
            return ForEachAsync(x => x.Start(cancellationToken));
        }

        public Task Stop(CancellationToken cancellationToken)
        {
            return ForEachAsync(x => x.Stop(cancellationToken));
        }
    }
}
