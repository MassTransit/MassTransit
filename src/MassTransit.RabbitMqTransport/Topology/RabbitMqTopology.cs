// Copyright 2007-2017 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.RabbitMqTransport.Topology
{
    using System;
    using System.Net;
    using System.Text;
    using System.Text.RegularExpressions;
    using NewIdFormatters;
    using Transports;
    using Util;


    public class RabbitMqTopology :
        IRabbitMqHostTopology
    {
        static readonly Regex _regex = new Regex(@"^[A-Za-z0-9\-_\.:]+$", RegexOptions.Compiled);

        static readonly INewIdFormatter _formatter = new ZBase32Formatter();

        readonly IExchangeTypeSelector _exchangeTypeSelector;
        readonly Uri _hostAddress;
        readonly IMessageNameFormatter _messageNameFormatter;

        public RabbitMqTopology(IExchangeTypeSelector exchangeTypeSelector, IMessageNameFormatter messageNameFormatter,
            Uri hostAddress)
        {
            _exchangeTypeSelector = exchangeTypeSelector;
            _messageNameFormatter = messageNameFormatter;
            _hostAddress = hostAddress;
        }

        public void ThrowIfEntityNameInvalid(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new RabbitMqAddressException("The entity name must not be null or empty");

            var success = IsValidEntityName(name);
            if (!success)
            {
                throw new RabbitMqAddressException(
                    "The entity name must be a sequence of these characters: letters, digits, hyphen, underscore, period, or colon.");
            }
        }

        public bool IsValidEntityName(string name)
        {
            return _regex.Match(name).Success;
        }

        public SendSettings GetSendSettings(Uri address)
        {
            var name = address.AbsolutePath.Substring(1);
            string[] pathSegments = name.Split('/');
            if (pathSegments.Length == 2)
                name = pathSegments[1];

            if (name == "*")
                throw new ArgumentException("Cannot send to a dynamic address");

            ThrowIfEntityNameInvalid(name);

            var isTemporary = address.Query.GetValueFromQueryString("temporary", false);

            var durable = address.Query.GetValueFromQueryString("durable", !isTemporary);
            var autoDelete = address.Query.GetValueFromQueryString("autodelete", isTemporary);

            var exchangeType = address.Query.GetValueFromQueryString("type") ?? _exchangeTypeSelector.DefaultExchangeType;

            var settings = new RabbitMqSendSettings(name, exchangeType, durable, autoDelete);

            var bindToQueue = address.Query.GetValueFromQueryString("bind", false);
            if (bindToQueue)
            {
                var queueName = WebUtility.UrlDecode(address.Query.GetValueFromQueryString("queue"));
                settings.BindToQueue(queueName);
            }

            var delayedType = address.Query.GetValueFromQueryString("delayedType");
            if (!string.IsNullOrWhiteSpace(delayedType))
                settings.SetExchangeArgument("x-delayed-type", delayedType);

            var bindExchange = address.Query.GetValueFromQueryString("bindexchange");
            if (!string.IsNullOrWhiteSpace(bindExchange))
                settings.BindToExchange(bindExchange);

            return settings;
        }

        public Uri GetDestinationAddress(string exchangeName, Action<IExchangeConfigurator> configure = null)
        {
            var sendSettings = new RabbitMqSendSettings(exchangeName, _exchangeTypeSelector.DefaultExchangeType, true, false);

            configure?.Invoke(sendSettings);

            return sendSettings.GetSendAddress(_hostAddress);
        }

        public Uri GetDestinationAddress(Type messageType, Action<IExchangeConfigurator> configure = null)
        {
            var isTemporary = messageType.IsTemporaryMessageType();

            var durable = !isTemporary;
            var autoDelete = isTemporary;

            var name = _messageNameFormatter.GetMessageName(messageType).ToString();

            var settings = new RabbitMqSendSettings(name, _exchangeTypeSelector.DefaultExchangeType, durable, autoDelete);

            configure?.Invoke(settings);

            return settings.GetSendAddress(_hostAddress);
        }

        public string CreateTemporaryQueueName(string prefix)
        {
            var sb = new StringBuilder(prefix);

            var host = HostMetadataCache.Host;

            foreach (var c in host.MachineName)
            {
                if (char.IsLetterOrDigit(c))
                    sb.Append(c);
                else if (c == '.' || c == '_' || c == '-' || c == ':')
                    sb.Append(c);
            }
            sb.Append('-');
            foreach (var c in host.ProcessName)
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

        public IMessageNameFormatter MessageNameFormatter => _messageNameFormatter;
    }
}