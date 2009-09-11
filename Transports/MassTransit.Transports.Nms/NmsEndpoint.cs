// Copyright 2007-2008 The Apache Software Foundation.
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
namespace MassTransit.Transports.Nms
{
	using System;
	using System.Diagnostics;
	using Configuration;
	using Serialization;

	/// <summary>
	/// The NMS endpoint is used to communicate with systems like ActiveMQ via the Apache.NMS
	/// project
	/// </summary>
	[DebuggerDisplay("{Address}")]
	public class NmsEndpoint :
		StreamEndpoint
	{
		public NmsEndpoint(IEndpointAddress address, IMessageSerializer serializer, ITransport transport, ITransport errorTransport)
			: base(address, serializer, transport, errorTransport)
		{
		}

		public static IEndpoint ConfigureEndpoint(Uri uri, Action<IEndpointConfigurator> configurator)
		{
			if (uri.Scheme.ToLowerInvariant() == "activemq")
			{
				IEndpoint endpoint = NmsEndpointConfigurator.New(x =>
					{
						x.SetUri(uri);

						configurator(x);
					});

				return endpoint;
			}

			return null;
		}
	}
}
