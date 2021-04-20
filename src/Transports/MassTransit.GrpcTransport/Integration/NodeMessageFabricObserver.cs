namespace MassTransit.GrpcTransport.Integration
{
    using System;
    using System.Linq;
    using Context;
    using Contexts;
    using Contracts;
    using Fabric;
    using GreenPipes;


    public class NodeMessageFabricObserver :
        IMessageFabricObserver
    {
        readonly IGrpcNode _node;
        readonly INodeCollection _nodes;

        public NodeMessageFabricObserver(INodeCollection nodes)
        {
            _nodes = nodes;
            _node = nodes.HostNode;
        }

        public void ExchangeDeclared(NodeContext context, string name, ExchangeType exchangeType)
        {
            Send(context, new Topology
            {
                Exchange = new Exchange
                {
                    Name = name,
                    Type = exchangeType
                }
            });
        }

        public void ExchangeBindingCreated(NodeContext context, string source, string destination, string routingKey)
        {
            Send(context, new Topology
            {
                ExchangeBind = new ExchangeBind
                {
                    Source = source,
                    Destination = destination,
                    RoutingKey = routingKey.ToNullableString()
                }
            });
        }

        public void QueueDeclared(NodeContext context, string name)
        {
            Send(context, new Topology {Queue = new Queue {Name = name}});
        }

        public void QueueBindingCreated(NodeContext context, string source, string destination)
        {
            Send(context, new Topology
            {
                QueueBind = new QueueBind
                {
                    Source = source,
                    Destination = destination,
                }
            });
        }

        public ConnectHandle ConsumerConnected(NodeContext context, ConnectHandle handle, string queueName)
        {
            return Send(context, new Topology {Consumer = new Consumer {QueueName = queueName}});
        }

        ConnectHandle Send(NodeContext context, Topology topology, ConnectHandle handle = default)
        {
            if (!_nodes.IsHostNode(context))
                return handle;

            try
            {
                handle = _node.AddTopology(topology, handle);

                var transportMessage = new TransportMessage
                {
                    MessageId = NewId.NextGuid().ToString(),
                    Topology = topology
                };

                foreach (var instance in _nodes.Where(x => x.NodeAddress != context.NodeAddress))
                {
                    if (!instance.Writer.TryWrite(transportMessage))
                        LogContext.Error?.Log("Failed to Send Topology {Topology} to {Address}", topology.ChangeCase, instance.NodeAddress);
                }

                return handle;
            }
            catch (Exception exception)
            {
                LogContext.Error?.Log(exception, "Failed to send topology message");
                throw;
            }
        }
    }
}
