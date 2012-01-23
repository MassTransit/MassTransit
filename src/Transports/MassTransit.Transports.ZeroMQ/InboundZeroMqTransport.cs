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
	using System;
	using System.IO;
	using System.Linq;
	using System.Text;
	using System.Threading;
	using Context;
	using Serialization;
	using ZMQ;
	using log4net;

	public class InboundZeroMqTransport :
		IInboundTransport
	{
		static readonly ILog _log = LogManager.GetLogger(typeof (InboundZeroMqTransport));
		readonly IZeroMqEndpointAddress _address;
		readonly ConnectionHandler<ZeroMqConnection> _connectionHandler;
		readonly bool _purgeExistingMessages;
		bool _disposed;

		public InboundZeroMqTransport(IZeroMqEndpointAddress address,
		                              ConnectionHandler<ZeroMqConnection> connectionHandler,
		                              bool purgeExistingMessages)
		{
			_address = address;
			_connectionHandler = connectionHandler;
			_purgeExistingMessages = purgeExistingMessages;
		}


		public IEndpointAddress Address
		{
			get { return _address; }
		}

		public void Receive(Func<IReceiveContext, Action<IReceiveContext>> callback, TimeSpan timeout)
		{
			_connectionHandler.Use(connection =>
				{
					var pollers = (from s in new[] {connection.PullSocket, connection.SubSocket}
					               let pi = s.CreatePollItem(IOMultiPlex.POLLIN)
					               select pi).ToArray();

					PollHandler errorHandler = (socket, revents) =>
						{
							// handle errors
							_log.Fatal("unknown error on zmq... check docs on how to read it");
						};

					Array.ForEach(pollers, p => { p.PollErrHandler += errorHandler; });


					/* Protocol:
					 * 
					 * A wants to publish something. It has looked this node up and want to make sure that it's ready to receive.
					 * We bind our sub socket to its sub socket.
					 * We read its address and bind our sub socket to it.
					 * We tell it to continue to send.
					 */

					pollers[0].PollInHandler += (pullSocket, revts) =>
						{
							var msg = pullSocket.Recv(Encoding.UTF8);

							// bind our sub socket
							connection.SubSocket.Bind(msg.Substring("LISTEN TO ME ".Length));

							// notify that we can consume things on this socket
							var targetSubSocket = connection.Address.SubSocket.ToString();

							if (_purgeExistingMessages)
								connection.PushSocket.Send(string.Format("PURGE {0}", targetSubSocket), Encoding.UTF8);

							connection.PushSocket.Send(string.Format("CONTINUE {0}", targetSubSocket), Encoding.UTF8);
						};
					pollers[1].PollInHandler += (subSocket, revts) =>
						{
							try
							{
								using (var body = new MemoryStream())
								{
									var offset = 0;
									var buffers = subSocket.RecvAll();
									while (buffers.Count > 0)
									{
										var dequeue = buffers.Dequeue();
										body.Write(dequeue, offset, dequeue.Length);
										offset += dequeue.Length;
									}

									// need to read envelope
									// where do we handle the envelope in MT?
									var context = ReceiveContext.FromBodyStream(body);

									var zmqMsgId = "ZMQ MSG ID";
									context.SetMessageId(zmqMsgId);
									context.SetInputAddress(_address);

									//handle content type

									var receive = callback(context);
									if (receive == null && _log.IsDebugEnabled)
										_log.DebugFormat("SKIP:{0}:{1}", Address, zmqMsgId);
									else if (receive != null)
										receive(context);

									connection.PushSocket.Send(string.Format("ACK {0}", zmqMsgId), Encoding.UTF8);
								}
							}
							catch (System.Exception ex)
							{
								_log.Error("Failed to consume message from endpoint", ex);

								//if (result != null)
								//    _consumer.MessageFailed(result.DeliveryTag, true);

								throw;
							}
						};

					if (connection.Context.Poll(pollers, 10*1000) == 0) // in microseconds for 2.1, 3.1 => ms.
						Thread.Sleep(10);
				});
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		void Dispose(bool disposing)
		{
			if (_disposed) return;
			if (disposing)
			{
				//RemoveConsumer();
			}

			_disposed = true;
		}

		~InboundZeroMqTransport()
		{
			Dispose(false);
		}
	}
}