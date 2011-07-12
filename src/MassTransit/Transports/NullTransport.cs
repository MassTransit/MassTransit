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
namespace MassTransit.Transports
{
	using log4net;

	public class NullOutboundTransport :
		IOutboundTransport
	{
		static readonly ILog _log = LogManager.GetLogger(typeof (NullOutboundTransport));
		readonly IEndpointAddress _address;

		public NullOutboundTransport(IEndpointAddress address)
		{
			_address = address;
		}

		public void Send(ISendContext context)
		{
			_log.DebugFormat("Discarding message on {0}: {1}", _address, context.MessageType);
		}

		public void Dispose()
		{
		}

		public IEndpointAddress Address
		{
			get { return _address; }
		}
	}
}