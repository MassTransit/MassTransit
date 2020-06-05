namespace MassTransit.Riders
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using GreenPipes;
    using GreenPipes.Util;


    public class RiderConnectable :
        IRider
    {
        readonly Connectable<IRider> _connectable;
        readonly IList<IRider> _riders;

        public RiderConnectable()
        {
            _connectable = new Connectable<IRider>();
            _riders = new List<IRider>();
        }

        public Task Start(CancellationToken cancellationToken)
        {
            return _connectable.ForEachAsync(x => x.Start(cancellationToken));
        }

        public Task Stop(CancellationToken cancellationToken)
        {
            return _connectable.ForEachAsync(x => x.Stop(cancellationToken));
        }

        public ConnectHandle Connect(IRider rider)
        {
            _riders.Add(rider);
            return new RiderConnectHandle(_connectable.Connect(rider), rider, _riders);
        }

        public T Get<T>()
            where T : IRider
        {
            return _riders.OfType<T>().FirstOrDefault();
        }


        class RiderConnectHandle :
            ConnectHandle
        {
            readonly ConnectHandle _handle;
            readonly IRider _rider;
            readonly IList<IRider> _riders;

            public RiderConnectHandle(ConnectHandle handle, IRider rider, IList<IRider> riders)
            {
                _handle = handle;
                _rider = rider;
                _riders = riders;
            }

            public void Dispose()
            {
                _handle.Disconnect();
            }

            public void Disconnect()
            {
                _handle.Disconnect();
                _riders.Remove(_rider);
            }
        }
    }
}
