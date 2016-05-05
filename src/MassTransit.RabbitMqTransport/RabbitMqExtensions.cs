// Copyright 2007-2015 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.RabbitMqTransport
{
    using System;
    using System.Text;
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

        public static string ToDebugString(this ConnectionFactory factory)
        {
            var sb = new StringBuilder();

            if (!string.IsNullOrWhiteSpace(factory.UserName))
                sb.Append(factory.UserName).Append('@');

            sb.Append(factory.HostName);
            if (factory.Port != -1)
                sb.Append(':').Append(factory.Port);

            if (string.IsNullOrWhiteSpace(factory.VirtualHost))
                sb.Append('/');
            else if (factory.VirtualHost.StartsWith("/"))
                sb.Append(factory.VirtualHost);
            else
                sb.Append("/").Append(factory.VirtualHost);

            return sb.ToString();
        }
    }
}