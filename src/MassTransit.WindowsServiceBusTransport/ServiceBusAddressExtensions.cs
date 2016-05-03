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
namespace MassTransit.WindowsServiceBusTransport
{
    using System;
    using System.Text;
    using Configuration;
    using Internals.Extensions;
    using Microsoft.ServiceBus.Messaging;
    using NewIdFormatters;
    using Util;


    public static class ServiceBusAddressExtensions
    {
        static readonly INewIdFormatter _formatter = new ZBase32Formatter();

        public static string GetTemporaryQueueName(this IServiceBusBusFactoryConfigurator configurator, string prefix)
        {
            var sb = new StringBuilder();

            var host = HostMetadataCache.Host;

            foreach (var c in host.MachineName)
            {
                if (char.IsLetterOrDigit(c))
                    sb.Append(c);
                else if (c == '_')
                    sb.Append(c);
            }
            sb.Append('_');
            foreach (var c in host.ProcessName)
            {
                if (char.IsLetterOrDigit(c))
                    sb.Append(c);
                else if (c == '_')
                    sb.Append(c);
            }
            sb.AppendFormat("_{0}_", prefix);
            sb.Append(NewId.Next().ToString(_formatter));

            return sb.ToString();
        }

        public static QueueDescription GetQueueDescription(this Uri address)
        {
            if (string.Compare("sb", address.Scheme, StringComparison.OrdinalIgnoreCase) != 0)
                throw new ArgumentException("The invalid scheme was specified: " + address.Scheme);

            var queueName = address.AbsolutePath.Trim('/');

            var queueDescription = Defaults.CreateQueueDescription(queueName);

            int autoDeleteOnIdleSeconds = address.GetValueFromQueryString("autodelete", 0);
            if (autoDeleteOnIdleSeconds > 0)
                queueDescription.AutoDeleteOnIdle = TimeSpan.FromSeconds(autoDeleteOnIdleSeconds);

            return queueDescription;
        }

        public static TopicDescription GetTopicDescription(this Uri address)
        {
            if (string.Compare("sb", address.Scheme, StringComparison.OrdinalIgnoreCase) != 0)
                throw new ArgumentException("The invalid scheme was specified: " + address.Scheme);

            var topicPath = address.AbsolutePath.Trim('/');

            var topicDescription = Defaults.CreateTopicDescription(topicPath);

            topicDescription.DefaultMessageTimeToLive = address.GetValueFromQueryString("ttl", topicDescription.DefaultMessageTimeToLive);
            int autoDeleteOnIdleSeconds = address.GetValueFromQueryString("autodelete", 0);
            if (autoDeleteOnIdleSeconds > 0)
                topicDescription.AutoDeleteOnIdle = TimeSpan.FromSeconds(autoDeleteOnIdleSeconds);

            return topicDescription;
        }
    }
}