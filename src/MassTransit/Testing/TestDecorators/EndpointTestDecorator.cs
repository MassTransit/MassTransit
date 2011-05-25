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
namespace MassTransit.Testing.TestDecorators
{
	using System;
	using Context;
	using Serialization;
	using Subjects;
	using TestContexts;
	using Transports;

	public class EndpointTestDecorator :
		IEndpoint
	{
		readonly IEndpoint _endpoint;
		readonly ReceivedMessageList _received;
		readonly SentMessageList _sent;
		EndpointTestContext _testContext;

		public EndpointTestDecorator(IEndpoint endpoint, EndpointTestContext testContext)
		{
			_endpoint = endpoint;
			_testContext = testContext;

			_sent = new SentMessageList();
			_received = new ReceivedMessageList();
		}

		public IReceivedMessageList Received
		{
			get { return _received; }
		}

		public ISentMessageList Sent
		{
			get { return _sent; }
		}

		public void Dispose()
		{
			_endpoint.Dispose();
			_received.Dispose();
			_sent.Dispose();
		}

		public IEndpointAddress Address
		{
			get { return _endpoint.Address; }
		}

		public IInboundTransport InboundTransport
		{
			get { return _endpoint.InboundTransport; }
		}

		public IOutboundTransport OutboundTransport
		{
			get { return _endpoint.OutboundTransport; }
		}

		public IOutboundTransport ErrorTransport
		{
			get { return _endpoint.ErrorTransport; }
		}

		public IMessageSerializer Serializer
		{
			get { return _endpoint.Serializer; }
		}

		public void Send<T>(ISendContext<T> context) where T : class
		{
			var send = new SentMessage<T>(context);
			try
			{
				_endpoint.Send(context);
			}
			catch (Exception ex)
			{
				send.SetException(ex);
				throw;
			}
			finally
			{
				_sent.Add(send);
				_testContext.AddSent(send);
			}
		}

		public void Receive(Func<IReceiveContext, Action<IReceiveContext>> receiver, TimeSpan timeout)
		{
			_endpoint.Receive(receiveContext =>
				{
					Action<IReceiveContext> receive = receiver(receiveContext);
					if (receive == null)
					{
						var skipped = new ReceivedMessage(receiveContext);
						_testContext.AddSkipped(skipped);

						return null;
					}

					return context =>
						{
							var received = new ReceivedMessage(context);
							try
							{
								receive(context);
							}
							catch (Exception ex)
							{
								received.SetException(ex);
								throw;
							}
							finally
							{
								_received.Add(received);
								_testContext.AddReceived(received);
							}
						};
				}, timeout);
		}
	}
}