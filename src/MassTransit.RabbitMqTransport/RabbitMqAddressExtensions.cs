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
    using System.Net;
    using System.Net.Security;
    using System.Security.Cryptography.X509Certificates;
    using System.Text;
    using System.Text.RegularExpressions;
    using Configuration;
    using Configuration.Configurators;
    using NewIdFormatters;
    using RabbitMQ.Client;
    using Topology;
    using Util;


    public static class RabbitMqAddressExtensions
    {
        static readonly INewIdFormatter _formatter = new ZBase32Formatter();
        static readonly Regex _regex = new Regex(@"^[A-Za-z0-9\-_\.:]+$");

        public static string GetTemporaryQueueName(this IRabbitMqBusFactoryConfigurator configurator, string prefix)
        {
            var sb = new StringBuilder(prefix);

            var host = HostMetadataCache.Host;

            foreach (char c in host.MachineName)
            {
                if (char.IsLetterOrDigit(c))
                    sb.Append(c);
                else if (c == '.' || c == '_' || c == '-' || c == ':')
                    sb.Append(c);
            }
            sb.Append('-');
            foreach (char c in host.ProcessName)
            {
                if (char.IsLetterOrDigit(c))
                    sb.Append(c);
                else if (c == '.' || c == '_' || c == '-' || c == ':')
                    sb.Append(c);
            }
            sb.Append('-');
            sb.Append(NewId.Next().ToString(_formatter));

            return sb.ToString();
        }

        public static string ToDebugString(this RabbitMqHostSettings settings)
        {
            var sb = new StringBuilder();

            if (!string.IsNullOrWhiteSpace(settings.Username))
                sb.Append(settings.Username).Append('@');

            sb.Append(settings.Host);
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

        public static Uri GetInputAddress(this RabbitMqHostSettings hostSettings, ReceiveSettings receiveSettings)
        {
            var builder = new UriBuilder
            {
                Scheme = "rabbitmq",
                Host = hostSettings.Host,
                Port = hostSettings.Port,
                Path = (string.IsNullOrWhiteSpace(hostSettings.VirtualHost) || hostSettings.VirtualHost == "/")
                    ? receiveSettings.QueueName
                    : string.Join("/", hostSettings.VirtualHost, receiveSettings.QueueName)
            };

            builder.Query += string.Join("&", GetQueryStringOptions(receiveSettings));

            return builder.Uri;
        }

        public static Uri GetQueueAddress(this RabbitMqHostSettings hostSettings, string queueName)
        {
            UriBuilder builder = GetHostUriBuilder(hostSettings, queueName);

            return builder.Uri;
        }

        /// <summary>
        /// Returns a UriBuilder for the host and entity specified
        /// </summary>
        /// <param name="hostSettings">The host settings</param>
        /// <param name="entityName">The entity name (queue/exchange)</param>
        /// <returns>A UriBuilder</returns>
        static UriBuilder GetHostUriBuilder(RabbitMqHostSettings hostSettings, string entityName)
        {
            return new UriBuilder
            {
                Scheme = "rabbitmq",
                Host = hostSettings.Host,
                Port = hostSettings.Port,
                Path = (string.IsNullOrWhiteSpace(hostSettings.VirtualHost) || hostSettings.VirtualHost == "/")
                    ? entityName
                    : string.Join("/", hostSettings.VirtualHost, entityName)
            };
        }

        /// <summary>
        /// Return a send address for the exchange
        /// </summary>
        /// <param name="host">The RabbitMQ host</param>
        /// <param name="exchangeName">The exchange name</param>
        /// <param name="configure">An optional configuration for the exchange to set type, durable, etc.</param>
        /// <returns></returns>
        public static Uri GetSendAddress(this IRabbitMqHost host, string exchangeName, Action<IExchangeConfigurator> configure = null)
        {
            var builder = GetHostUriBuilder(host.Settings, exchangeName);

            var sendSettings = new RabbitMqSendSettings(exchangeName, ExchangeType.Fanout, true, false);

            configure?.Invoke(sendSettings);

            builder.Query += string.Join("&", GetQueryStringOptions(sendSettings));

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
                yield return "queue=" + WebUtility.UrlEncode(settings.QueueName);
            if (settings.ExchangeType != ExchangeType.Fanout)
                yield return "type=" + settings.ExchangeType;
            if (settings.ExchangeArguments != null && settings.ExchangeArguments.ContainsKey("x-delayed-type"))
                yield return "delayedType=" + settings.ExchangeArguments["x-delayed-type"];

            foreach (var binding in settings.ExchangeBindings)
            {
                yield return $"bindexchange={binding.Exchange.ExchangeName}";
            }
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

            string exchangeType = address.Query.GetValueFromQueryString("type") ?? ExchangeType.Fanout;

            var settings = new RabbitMqSendSettings(name, exchangeType, durable, autoDelete);

            bool bindToQueue = address.Query.GetValueFromQueryString("bind", false);
            if (bindToQueue)
            {
                string queueName = WebUtility.UrlDecode(address.Query.GetValueFromQueryString("queue"));
                settings.BindToQueue(queueName);
            }

            string delayedType = address.Query.GetValueFromQueryString("delayedType");
            if (!string.IsNullOrWhiteSpace(delayedType))
                settings.SetExchangeArgument("x-delayed-type", delayedType);

            string bindExchange = address.Query.GetValueFromQueryString("bindexchange");
            if (!string.IsNullOrWhiteSpace(bindExchange))
                settings.BindToExchange(bindExchange);

            return settings;
        }

        [Obsolete]
        public static SendSettings GetSendSettings(this IRabbitMqHost host, Type messageType)
        {
            return GetSendSettings(host.Settings, messageType);
        }

        public static SendSettings GetSendSettings(this RabbitMqHostSettings hostSettings, Type messageType)
        {
            bool isTemporary = messageType.IsTemporaryMessageType();

            bool durable = !isTemporary;
            bool autoDelete = isTemporary;

            string name = hostSettings.MessageNameFormatter.GetMessageName(messageType).ToString();

            return new RabbitMqSendSettings(name, ExchangeType.Fanout, durable, autoDelete);
        }

        public static ConnectionFactory GetConnectionFactory(this RabbitMqHostSettings settings)
        {
            var factory = new ConnectionFactory
            {
                AutomaticRecoveryEnabled = false,
                NetworkRecoveryInterval = TimeSpan.FromSeconds(1),
                TopologyRecoveryEnabled = false,
                HostName = settings.Host,
                Port = settings.Port,
                VirtualHost = settings.VirtualHost ?? "/",
                RequestedHeartbeat = settings.Heartbeat
            };

            if (settings.UseClientCertificateAsAuthenticationIdentity)
            {
                factory.AuthMechanisms.Clear();
                factory.AuthMechanisms.Add(new ExternalMechanismFactory());
                factory.UserName = "";
                factory.Password = "";
            }
            else
            {
                if (!string.IsNullOrWhiteSpace(settings.Username))
                    factory.UserName = settings.Username;
                if (!string.IsNullOrWhiteSpace(settings.Password))
                    factory.Password = settings.Password;
            }

            factory.Ssl.Enabled = settings.Ssl;
            factory.Ssl.Version = settings.SslProtocol;
            factory.Ssl.AcceptablePolicyErrors = settings.AcceptablePolicyErrors;
            factory.Ssl.ServerName = settings.SslServerName;
            factory.Ssl.Certs = settings.ClientCertificate == null ? null : new X509Certificate2Collection {settings.ClientCertificate};
            
            if (string.IsNullOrWhiteSpace(factory.Ssl.ServerName))
                factory.Ssl.AcceptablePolicyErrors |= SslPolicyErrors.RemoteCertificateNameMismatch;

            if (string.IsNullOrEmpty(settings.ClientCertificatePath))
            {
                factory.Ssl.CertPath = "";
                factory.Ssl.CertPassphrase = "";
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

            var hostSettings = new ConfigurationHostSettings
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
            if (string.IsNullOrWhiteSpace(queueName))
                throw new RabbitMqAddressException("The queue name must not be null or empty");

            bool success = IsValidQueueName(queueName);
            if (!success)
            {
                throw new RabbitMqAddressException(
                    "The queueName must be a sequence of these characters: letters, digits, hyphen, underscore, period, or colon.");
            }
        }

        public static bool IsValidQueueName(string queueName)
        {
            Match match = _regex.Match(queueName);
            bool success = match.Success;
            return success;
        }
    }
}