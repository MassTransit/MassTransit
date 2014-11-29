// Copyright 2007-2014 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.Transports.RabbitMq
{
    using System;
    using System.Globalization;
    using System.Text.RegularExpressions;
    using Configuration;
    using RabbitMQ.Client;
    using Util;


    public static class RabbitMqAddressExtensions
    {
        static readonly Regex _regex = new Regex(@"^[A-Za-z0-9\-_\.:]+$");

        public static ReceiveSettings GetReceiveSettings(this Uri address)
        {
            if (string.Compare("rabbitmq", address.Scheme, StringComparison.OrdinalIgnoreCase) != 0)
                throw new RabbitMqAddressException("The invalid scheme was specified: " + address.Scheme);

            var connectionFactory = new ConnectionFactory
            {
                HostName = address.Host,
                UserName = "guest",
                Password = "guest",
            };

            if (address.IsDefaultPort)
                connectionFactory.Port = 5672;
            else if (!address.IsDefaultPort)
                connectionFactory.Port = address.Port;

//            if (!string.IsNullOrEmpty(address.UserInfo))
//            {
//                string[] parts = address.UserInfo.Split(':');
//                connectionFactory.UserName = parts[0];
//
//                if (parts.Length >= 2)
//                    connectionFactory.Password = parts[1];
//            }

            string name = address.AbsolutePath.Substring(1);
            string[] pathSegments = name.Split('/');
            if (pathSegments.Length == 2)
            {
                connectionFactory.VirtualHost = pathSegments[0];
                name = pathSegments[1];
            }

            ushort heartbeat = address.Query.GetValueFromQueryString("heartbeat", connectionFactory.RequestedHeartbeat);
            connectionFactory.RequestedHeartbeat = heartbeat;

            if (name == "*")
            {
                string uri = address.GetLeftPart(UriPartial.Path);
                if (uri.EndsWith("*"))
                {
                    name = NewId.Next().ToString("NS");
                    uri = uri.Remove(uri.Length - 1) + name;

                    var builder = new UriBuilder(uri);
                    builder.Query = string.IsNullOrEmpty(address.Query) ? "" : address.Query.Substring(1);

                    address = builder.Uri;
                }
                else
                    throw new InvalidOperationException("Uri is not properly formed");
            }
            else
                VerifyQueueOrExchangeNameIsLegal(name);


            ushort prefetch = address.Query.GetValueFromQueryString("prefetch", (ushort)Math.Max(Environment.ProcessorCount, 16));
            int timeToLive = address.Query.GetValueFromQueryString("ttl", 0);

            bool isTemporary = address.Query.GetValueFromQueryString("temporary", false);

            bool durable = address.Query.GetValueFromQueryString("durable", !isTemporary);
            bool exclusive = address.Query.GetValueFromQueryString("exclusive", isTemporary);
            bool autoDelete = address.Query.GetValueFromQueryString("autodelete", isTemporary);

            ReceiveSettings settings = new RabbitMqReceiveSettings
            {
                AutoDelete = autoDelete,
                Durable = durable,
                Exclusive = exclusive,
                QueueName = name,
                PrefetchCount = prefetch,
            };

            if (timeToLive > 0)
                settings.QueueArguments.Add("x-message-ttl", timeToLive.ToString("F0", CultureInfo.InvariantCulture));

            return settings;
        }

        public static SendSettings GetSendSettings(this Uri address)
        {
            if (string.Compare("rabbitmq", address.Scheme, StringComparison.OrdinalIgnoreCase) != 0)
                throw new RabbitMqAddressException("The invalid scheme was specified: " + address.Scheme);

            string name = address.AbsolutePath.Substring(1);
            string[] pathSegments = name.Split('/');
            if (pathSegments.Length == 2)
            {
                name = pathSegments[1];
            }


            if (name == "*")
                throw new ArgumentException("Cannot send to a dynamic address");

            VerifyQueueOrExchangeNameIsLegal(name);

            bool isTemporary = address.Query.GetValueFromQueryString("temporary", false);

            bool durable = address.Query.GetValueFromQueryString("durable", !isTemporary);
            bool autoDelete = address.Query.GetValueFromQueryString("autodelete", isTemporary);

            SendSettings settings = new RabbitMqSendSettings(name, ExchangeType.Fanout, durable, autoDelete);

            return settings;
        }

        public static ConnectionFactory GetConnectionFactory(this RabbitMqHostSettings settings)
        {
            var factory = new ConnectionFactory()
            {
                AutomaticRecoveryEnabled = true,
                NetworkRecoveryInterval = TimeSpan.FromSeconds(1),
                TopologyRecoveryEnabled = true,

                HostName = settings.Host,
                Port = settings.Port,
                VirtualHost = settings.VirtualHost,
                RequestedHeartbeat = settings.Heartbeat
            };

            if (!string.IsNullOrWhiteSpace(settings.Username))
                factory.UserName = settings.Username;
            if (!string.IsNullOrWhiteSpace(settings.Password))
                factory.Password = settings.Password;

            return factory;
        }

        public static RabbitMqHostSettings GetHostSettings(this Uri address)
        {
            if (string.Compare("rabbitmq", address.Scheme, StringComparison.OrdinalIgnoreCase) != 0)
                throw new RabbitMqAddressException("The invalid scheme was specified: " + address.Scheme);

            var hostSettings = new HostSettings
            {
                Host = address.Host, 
                Username = "", 
                Password = ""
            };

            hostSettings.Port = address.IsDefaultPort ? 5672 : address.Port;

            if (!string.IsNullOrEmpty(address.UserInfo))
            {
                string[] parts = address.UserInfo.Split(':');
                hostSettings.Username = parts[0];

                if (parts.Length >= 2)
                    hostSettings.Password = parts[1];
            }

            string name = address.AbsolutePath.Substring(1);

            string[] pathSegments = name.Split('/');
            if (pathSegments.Length == 2)
                hostSettings.VirtualHost = pathSegments[0];

            hostSettings.Heartbeat = address.Query.GetValueFromQueryString("heartbeat", (ushort)0);

            return hostSettings;
        }

        static void VerifyQueueOrExchangeNameIsLegal(string path)
        {
            Match match = _regex.Match(path);
            if (!match.Success)
            {
                throw new RabbitMqAddressException(
                    "The path can be empty, or a sequence of these characters: letters, digits, hyphen, underscore, period, or colon.");
            }
        }


        class HostSettings :
            RabbitMqHostSettings
        {
            public string Host { get; set; }
            public int Port { get; set; }
            public string VirtualHost { get; set; }
            public string Username { get; set; }
            public string Password { get; set; }
            public ushort Heartbeat { get; set; }
        }
    }
}