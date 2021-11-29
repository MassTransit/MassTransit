namespace MassTransit.Util
{
    using System.Collections.Generic;
    using System.Linq;


    public class MultipleConnectHandle :
        ConnectHandle
    {
        readonly ConnectHandle[] _handles;

        public MultipleConnectHandle(IEnumerable<ConnectHandle> handles)
        {
            _handles = handles.ToArray();
        }

        public MultipleConnectHandle(params ConnectHandle[] handles)
        {
            _handles = handles;
        }

        public void Disconnect()
        {
            for (var i = 0; i < _handles.Length; i++)
                _handles[i].Disconnect();
        }

        public void Dispose()
        {
            Disconnect();
        }
    }
}
