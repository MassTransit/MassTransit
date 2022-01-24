namespace MassTransit.RabbitMqTransport
{
    using System;
    using System.Text;
    using Configuration;
    using RabbitMQ.Client;


    public static class RabbitMqExtensions
    {
        /// <summary>
        /// Close and dispose of a RabbitMQ channel without throwing any exceptions
        /// </summary>
        /// <param name="model">The channel (can be null)</param>
        /// <param name="replyCode"></param>
        /// <param name="message">Message for channel closure</param>
        public static void Cleanup(this IModel model, ushort replyCode = 200, string message = "Unknown")
        {
            if (model != null)
            {
                try
                {
                    if (model.IsOpen)
                        model.Close(replyCode, message);
                }
                catch (Exception)
                {
                }

                model.Dispose();
            }
        }

        /// <summary>
        /// Close and dispose of a RabbitMQ connection without throwing any exceptions
        /// </summary>
        /// <param name="connection">The channel (can be null)</param>
        /// <param name="replyCode"></param>
        /// <param name="message">Message for channel closure</param>
        public static void Cleanup(this IConnection connection, ushort replyCode = 200, string message = "Unknown")
        {
            if (connection != null)
            {
                try
                {
                    if (connection.IsOpen)
                        connection.Close(replyCode, message);
                }
                catch (Exception)
                {
                }
            }
        }

        public static string ToDescription(this RabbitMqHostSettings settings)
        {
            var sb = new StringBuilder();

            if (!string.IsNullOrWhiteSpace(settings.Username))
                sb.Append(settings.Username).Append('@');

            sb.Append(settings.Host);

            ClusterNode? actualHost = settings.EndpointResolver?.LastHost;
            if (actualHost != null)
                sb.Append('(').Append(actualHost).Append(')');
            if (settings.Port != -1)
                sb.Append(':').Append(settings.Port);

            if (string.IsNullOrWhiteSpace(settings.VirtualHost))
                sb.Append('/');
            else if (settings.VirtualHost.StartsWith("/"))
                sb.Append(settings.VirtualHost);
            else
                sb.Append("/").Append(settings.VirtualHost);

            return sb.ToString();
        }

        public static string ToDescription(this RabbitMqHostSettings settings, ConnectionFactory connectionFactory)
        {
            var sb = new StringBuilder();

            if (!string.IsNullOrWhiteSpace(connectionFactory.UserName))
                sb.Append(connectionFactory.UserName).Append('@');

            sb.Append(settings.Host);

            ClusterNode? actualHost = settings.EndpointResolver?.LastHost;
            if (actualHost != null)
                sb.Append('(').Append(actualHost).Append(')');
            if (settings.Port != -1)
                sb.Append(':').Append(settings.Port);

            if (string.IsNullOrWhiteSpace(settings.VirtualHost))
                sb.Append('/');
            else if (settings.VirtualHost.StartsWith("/"))
                sb.Append(settings.VirtualHost);
            else
                sb.Append("/").Append(settings.VirtualHost);

            return sb.ToString();
        }
    }
}
