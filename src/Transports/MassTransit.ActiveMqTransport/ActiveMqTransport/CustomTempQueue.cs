using Apache.NMS.ActiveMQ.Commands;

namespace MassTransit.ActiveMqTransport
{
    public class CustomTempQueue : ActiveMQTempQueue
    {
        public CustomTempQueue(string name)
            : base(name)
        {
        }

        public override int GetDestinationType() => ACTIVEMQ_QUEUE;

        public override ActiveMQDestination CreateDestination(string name) => new CustomTempQueue(name);
    }
}
