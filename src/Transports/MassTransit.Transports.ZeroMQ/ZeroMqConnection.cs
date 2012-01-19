// Copyright 2007-2011 Chris Patterson, Dru Sellers, Travis Smith, et. al.
//  
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use 
// this file except in compliance with the License. You may obtain a copy of the 
// License at 
// 
//     http://www.apache.org/licenses/LICENSE-2.0 
// 
// Unless required by applicable law or agreed to in writing, software distributed 
// under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR 
// CONDITIONS OF ANY KIND, either express or implied. See the License for the 
// specific language governing permissions and limitations under the License.
namespace MassTransit.Transports.ZeroMq
{
    using System.Diagnostics;
    using Util;
    using ZMQ;
    using log4net;

    [DebuggerDisplay("Connected:{_connected}")]
    public class ZeroMqConnection :
        Connection
    {
        static readonly ILog _log = LogManager.GetLogger(typeof (ZeroMqConnection));
        Context _context;
        SocketType _socketType;
        Socket _socket;
        ZeroMqAddress _address;

		[UsedImplicitly]
        bool _connected;

        public ZeroMqConnection(Context context,
            ZeroMqAddress address,
            SocketType socketType)
        {
            _context = context;
            _address = address;
            _socketType = socketType;
        }

        public void Dispose()
        {
            Disconnect();
        }

        public Socket Socket
        {
            get { return _socket; }
        }
        public void Connect()
        {
            Disconnect();

            _socket = _context.Socket(_socketType);
            
            //this needs to be configurable maybe the '/queue' part of the uri?
            _socket.Identity = new byte[]{12,12};

            var addr = _address.RebuiltUri.ToString();

            if (addr.EndsWith("/")) //TODO: log this as an issue with ZMQ?
                addr = addr.Substring(0, addr.Length - 1);

            _socket.Connect(addr);
            _connected = true;
        }

        public void Disconnect()
        {
            try
            {
                if (_socket != null)
                {
                    //_socket.Unsubscribe(); needed? 
                    _socket.Dispose();
                }
                _socket = null;
            }
            catch (System.Exception ex)
            {
                _log.Warn("Faild to close ZeroMq connection.", ex);
            	throw;
            }
            _connected = false;
        }
    }
}