namespace MassTransit.Riders
{
    using System;
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
        readonly List<IRider> _riders;

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
            return new RiderConnectHandle(_connectable.Connect(rider), () => _riders.Remove(rider));
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
            readonly Action _onDisconnect;

            public RiderConnectHandle(ConnectHandle handle, Action onDisconnect)
            {
                _handle = handle;
                _onDisconnect = onDisconnect;
            }

            public void Dispose()
            {
                _handle.Disconnect();
            }

            public void Disconnect()
            {
                _handle.Disconnect();
                _onDisconnect();
            }
        }
    }
}
