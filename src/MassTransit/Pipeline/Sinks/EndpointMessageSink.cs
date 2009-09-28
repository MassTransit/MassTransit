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
namespace MassTransit.Pipeline.Sinks
{
	using System;
	using System.Collections.Generic;
	using Internal;

	/// <summary>
	/// A message sink that sends to an endpoint
	/// </summary>
	/// <typeparam name="TMessage"></typeparam>
	public class EndpointMessageSink<TMessage> :
		IPipelineSink<TMessage>
		where TMessage : class
	{
		private readonly IEndpoint _endpoint;

		public EndpointMessageSink(IEndpoint endpoint)
		{
			_endpoint = endpoint;
		}

		public Uri Address
		{
			get { return _endpoint.Uri; }
		}

		public IEnumerable<Action<TMessage>> Enumerate(TMessage message)
		{
			yield return x =>
				{
					if(OutboundMessage.Context.WasEndpointAlreadySent(_endpoint.Uri))
						return;

					_endpoint.Send(x);

					OutboundMessage.Context.NotifyForMessageConsumer(message, _endpoint);
				}; 
		}

		public bool Inspect(IPipelineInspector inspector)
		{
			inspector.Inspect(this);

			// since this is the end of the line, we don't visit this one I suppose
			return true;
		}

		public void Dispose()
		{
		}
	}
}