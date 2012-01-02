namespace MassTransit.Transports.ZeroMq
{
    using System.IO;
    using ZMQ;

    public class OutboundZeroMqTransport :
        IOutboundTransport
    {
        readonly IZeroMqEndpointAddress _address;
        ConnectionHandler<ZeroMqConnection> _connectionHandler;

        public OutboundZeroMqTransport(IZeroMqEndpointAddress address, ConnectionHandler<ZeroMqConnection> connectionHandler)
        {
            _address = address;
            _connectionHandler = connectionHandler;
        }


        public IEndpointAddress Address
        {
            get { return _address; }
        }

        public void Send(ISendContext context)
        {
            _connectionHandler.Use(connection =>
                {
                    try
                    {
                    //build up basic zmq envelope
                    if(context.ExpirationTime.HasValue)
                    {
                        
                    }

                    using(var body = new MemoryStream())
                    {
                        
                        context.SerializeTo(body);
                        connection.Socket.Send(body.ToArray());
                    }

                    }
                    catch (System.Exception ex)
                    {
                        throw;
                    }
                });
        }

        public void Dispose()
        {
            //no-op
        }

    }
}