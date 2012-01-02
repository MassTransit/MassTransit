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
	using System.Text;
	using System.Threading;
	using Context;
	using log4net;

    public class InboundZeroMqTransport :
		IInboundTransport
	{
		static readonly ILog _log = LogManager.GetLogger(typeof (InboundZeroMqTransport));
	    readonly IZeroMqEndpointAddress _address;
	    ConnectionHandler<ZeroMqConnection> _connectionHandler;
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
                    byte[] result = null;
                    try
                    {
                        result = connection.Socket.Recv(); //we can auto string it here by adding the encoding
                        if(result == null)
                        {
                            Thread.Sleep(10);
                            return;
                        }
                        using(var body = new MemoryStream(result, false))
                        {
                            var context = ReceiveContext.FromBodyStream(body);
                            context.SetMessageId("ZMQ MSG ID");
                            context.SetInputAddress(_address);

                            //handle content type

                            Action<IReceiveContext> receive = callback(context);
                            if(receive == null)
                            {
                                if(_log.IsDebugEnabled)
                                    _log.DebugFormat("SKIP:{0}:{1}", Address,"MESSAGEID");
                            }
                            else
                            {
                                receive(context);
                            }

                            //_consumer.MessageCompleted(result.DeliveryTag);
                        }
                    }
                    catch (Exception ex)
                    {
                        _log.Error("Failed to consume message from endpoint", ex);

//                        if (result != null)
//                            _consumer.MessageFailed(result.DeliveryTag, true);

                        throw;
                    }
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