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
namespace MassTransit.Transports.ZeroMQ
{
    using ZMQ;
    using ZeroMq;

    public class ZeroMqTransportFactory :
		ITransportFactory
	{
        //this should own the creation of the Context
        Context _context;

        public ZeroMqTransportFactory(int numberOfIoThreads = 1)
        {
            _context  = new Context(numberOfIoThreads);
        }

        public string Scheme
		{
			get { return "zeromq"; }
		}

		public IDuplexTransport BuildLoopback(ITransportSettings settings)
		{
            return new Transports.Transport(settings.Address,
                ()=> BuildInbound(settings),
                ()=>BuildOutbound(settings));
		}

		public IInboundTransport BuildInbound(ITransportSettings settings)
		{
		    var address = (ZeroMqAddress)settings.Address;
		    var zeroMqConnection = new ZeroMqConnection(_context, address,SocketType.REQ); //what should the type be?
		    var handler = new ConnectionHandlerImpl<ZeroMqConnection>(zeroMqConnection);
		    return new InboundZeroMqTransport(address, handler, true);
		}

		public IOutboundTransport BuildOutbound(ITransportSettings settings)
		{
		    var address = (ZeroMqAddress)settings.Address;
		    var zeroMqConnection = new ZeroMqConnection(_context, address, SocketType.REQ); //what should the type be?
		    var handler = new ConnectionHandlerImpl<ZeroMqConnection>(zeroMqConnection);
			return new OutboundZeroMqTransport(address, handler);
		}

		public IOutboundTransport BuildError(ITransportSettings settings)
		{
			return BuildLoopback(settings); //what the F' should this be.
		}

		public void Dispose()
		{
            if(_context != null)
            {
                _context.Dispose();
                _context = null;
            }
		}
	}
}