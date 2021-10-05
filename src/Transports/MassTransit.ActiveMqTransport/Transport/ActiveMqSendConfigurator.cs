using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MassTransit.ActiveMqTransport.Transport
{
    public static class ActiveMqSendConfigurator
    {
        public static MsgType MessageType = MsgType.Binary ;

        /// <summary>
        ///
        /// </summary>
        public static readonly Dictionary<string, MsgType> MessageTypesConfigurator = new Dictionary<string, MsgType>();


        public static Task SendAsTextMessage<T>(this IBus bus, T message , CancellationToken cancellationToken = default)
         where T: class
        {
            MessageTypesConfigurator[typeof(T).Name] = MsgType.Text;
            return bus.Send(message,cancellationToken);
        }


        public static void MapTypeToQueue<T>(this IBus bus,  string queueName)
            where T : class
        {
            EndpointConvention.Map<T>(new Uri($"queue:{queueName}"));
        }

        public static Task SendAsObjectMessage<T>(this IBus bus, T message, CancellationToken cancellationToken = default)
            where T: class
        {
            MessageTypesConfigurator[typeof(T).Name] = MsgType.Object;
            return bus.Send(message, cancellationToken);
        }

        public static Task SendAsBinaryMessage<T>(this IBus bus, T message, CancellationToken cancellationToken = default)
            where T: class
        {
            MessageTypesConfigurator[typeof(T).Name] = MsgType.Binary;
            return bus.Send(message, cancellationToken);
        }
    }



}
