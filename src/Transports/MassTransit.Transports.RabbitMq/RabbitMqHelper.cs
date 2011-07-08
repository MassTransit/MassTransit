namespace MassTransit.Transports.RabbitMq
{
    using RabbitMQ.Client;

    public static class RabbitMqHelper
    {
        /// <summary>
        /// Binds the queue to the exchange
        /// </summary>
        public static void Bind(IConnection connection, string queue, string exchange)
        {
            using(var m = connection.CreateModel())
            {
                 m.QueueBind(queue, exchange, "", null);
            }
        }
    }
}