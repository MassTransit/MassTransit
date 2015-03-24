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
    using System.Collections.Generic;
    using System.Globalization;
    using System.Net.Security;
    using System.Security.Authentication;
    using System.Security.Policy;
    using System.Text.RegularExpressions;
    using Configuration;
    using Pipeline;
    using RabbitMQ.Client;
    using Transports;
    using Util;


    public static class RabbitMqAddressExtensions
    {
        static readonly Regex _regex = new Regex(@"^[A-Za-z0-9\-_\.:]+$");

        public static Uri GetInputAddress(this RabbitMqHostSettings hostSettings, ReceiveSettings receiveSettings)
        {
            var builder = new UriBuilder
            {
                Scheme = "rabbitmq",
                Host = hostSettings.Host,
                Port = hostSettings.Port,
                Path = hostSettings.VirtualHost != "/"
                    ? string.Join("/", hostSettings.VirtualHost, receiveSettings.QueueName)
                    : receiveSettings.QueueName
            };

            builder.Query += string.Join("&", GetQueryStringOptions(receiveSettings));

            return builder.Uri;
        }

        public static Uri GetSendAddress(this RabbitMqHostSettings hostSettings, SendSettings sendSettings)
        {
            var builder = new UriBuilder
            {
                Scheme = "rabbitmq",
                Host = hostSettings.Host,
                Port = hostSettings.Port,
                Path = hostSettings.VirtualHost != "/"
                    ? string.Join("/", hostSettings.VirtualHost, sendSettings.ExchangeName)
                    : sendSettings.ExchangeName
            };

            builder.Query += string.Join("&", GetQueryStringOptions(sendSettings));

            return builder.Uri;
        }

        static IEnumerable<string> GetQueryStringOptions(ReceiveSettings settings)
        {
            if (!settings.Durable)
                yield return "durable=false";
            if (settings.AutoDelete)
                yield return "autodelete=true";
            if (settings.Exclusive)
                yield return "exclusive=true";
            if (settings.PrefetchCount != 0)
                yield return "prefetch=" + settings.PrefetchCount;
        }

        static IEnumerable<string> GetQueryStringOptions(SendSettings settings)
        {
            if (!settings.Durable)
                yield return "durable=false";
            if (settings.AutoDelete)
                yield return "autodelete=true";
            if (settings.BindToQueue)
                yield return "bind=true";
            if (!string.IsNullOrWhiteSpace(settings.QueueName))
                yield return "queue=" + System.Net.WebUtility.UrlEncode(settings.QueueName);
        }

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

        /// <summary>
        /// Return the send settings for the address
        /// </summary>
        /// <param name="address"></param>
        /// <returns></returns>
        public static SendSettings GetSendSettings(this Uri address)
        {
            if (string.Compare("rabbitmq", address.Scheme, StringComparison.OrdinalIgnoreCase) != 0)
                throw new RabbitMqAddressException("The invalid scheme was specified: " + address.Scheme);

            string name = address.AbsolutePath.Substring(1);
            string[] pathSegments = name.Split('/');
            if (pathSegments.Length == 2)
                name = pathSegments[1];


            if (name == "*")
                throw new ArgumentException("Cannot send to a dynamic address");

            VerifyQueueOrExchangeNameIsLegal(name);

            bool isTemporary = address.Query.GetValueFromQueryString("temporary", false);

            bool durable = address.Query.GetValueFromQueryString("durable", !isTemporary);
            bool autoDelete = address.Query.GetValueFromQueryString("autodelete", isTemporary);

            var settings = new RabbitMqSendSettings(name, ExchangeType.Fanout, durable, autoDelete);

            bool bindToQueue = address.Query.GetValueFromQueryString("bind", false);
            if (bindToQueue)
            {
                string queueName = System.Net.WebUtility.UrlDecode(address.Query.GetValueFromQueryString("queue"));
                settings.BindToQueue(queueName);
            }

            return settings;
        }

        public static SendSettings GetSendSettings(this IRabbitMqHost host, Type messageType, IMessageNameFormatter messageNameFormatter)
        {
            bool isTemporary = messageType.IsTemporaryMessageType();

            bool durable = !isTemporary;
            bool autoDelete = isTemporary;

            string name = messageNameFormatter.GetMessageName(messageType).ToString();

            SendSettings settings = new RabbitMqSendSettings(name, ExchangeType.Fanout, durable, autoDelete);

            return settings;
        }

        public static ConnectionFactory GetConnectionFactory(this RabbitMqHostSettings settings)
        {
            var factory = new ConnectionFactory
            {
                AutomaticRecoveryEnabled = true,
                NetworkRecoveryInterval = TimeSpan.FromSeconds(1),
                TopologyRecoveryEnabled = true,
                HostName = settings.Host,
                Port = settings.Port,
                VirtualHost = settings.VirtualHost,
                RequestedHeartbeat = settings.Heartbeat
            };

            factory.Ssl.Enabled = settings.Ssl;
            factory.Ssl.Version = SslProtocols.Tls;
            factory.Ssl.AcceptablePolicyErrors = settings.AcceptablePolicyErrors;
            factory.Ssl.ServerName = settings.SslServerName;
            if (string.IsNullOrWhiteSpace(factory.Ssl.ServerName))
                factory.Ssl.AcceptablePolicyErrors |= SslPolicyErrors.RemoteCertificateNameMismatch;

            if (string.IsNullOrEmpty(settings.ClientCertificatePath))
            {
                if (!string.IsNullOrWhiteSpace(settings.Username))
                    factory.UserName = settings.Username;
                if (!string.IsNullOrWhiteSpace(settings.Password))
                    factory.Password = settings.Password;

                factory.Ssl.CertPath = "";
                factory.Ssl.CertPassphrase = "";
                factory.Ssl.Certs = null;
            }
            else
            {
                factory.Ssl.CertPath = settings.ClientCertificatePath;
                factory.Ssl.CertPassphrase = settings.ClientCertificatePassphrase;
            }

            factory.ClientProperties = factory.ClientProperties ?? new Dictionary<string, object>();

            HostInfo hostInfo = HostMetadataCache.Host;

            factory.ClientProperties["client_api"] = "MassTransit";
            factory.ClientProperties["masstransit_version"] = hostInfo.MassTransitVersion;
            factory.ClientProperties["net_version"] = hostInfo.FrameworkVersion;
            factory.ClientProperties["hostname"] = hostInfo.MachineName;
            factory.ClientProperties["connected"] = DateTimeOffset.Now.ToString("R");
            factory.ClientProperties["process_id"] = hostInfo.ProcessId.ToString();
            factory.ClientProperties["process_name"] = hostInfo.ProcessName;
            if (hostInfo.Assembly != null)
                factory.ClientProperties["assembly"] = hostInfo.Assembly;
            if (hostInfo.AssemblyVersion != null)
                factory.ClientProperties["assembly_version"] = hostInfo.AssemblyVersion;

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
                Password = "",
                Port = address.IsDefaultPort ? 5672 : address.Port
            };

            if (!string.IsNullOrEmpty(address.UserInfo))
            {
                string[] parts = address.UserInfo.Split(':');
                hostSettings.Username = parts[0];

                if (parts.Length >= 2)
                    hostSettings.Password = parts[1];
            }

            string name = address.AbsolutePath.Substring(1);

            string[] pathSegments = name.Split('/');
            hostSettings.VirtualHost = pathSegments.Length == 2 ? pathSegments[0] : "/";

            hostSettings.Heartbeat = address.Query.GetValueFromQueryString("heartbeat", (ushort)0);

            return hostSettings;
        }

        static void VerifyQueueOrExchangeNameIsLegal(string queueName)
        {
            bool success = IsValidQueueName(queueName);
            if (!success)
            {
                throw new RabbitMqAddressException(
                    "The queueName can be empty, or a sequence of these characters: letters, digits, hyphen, underscore, period, or colon.");
            }
        }

        public static bool IsValidQueueName(string queueName)
        {
            Match match = _regex.Match(queueName);
            bool success = match.Success;
            return success;
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
            public bool Ssl { get; private set; }
            public string SslServerName { get; private set; }
            public SslPolicyErrors AcceptablePolicyErrors { get; private set; }
            public string ClientCertificatePath { get; private set; }
            public string ClientCertificatePassphrase { get; private set; }
        }
    }
}