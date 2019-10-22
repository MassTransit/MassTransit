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
namespace MassTransit.Topology.Topologies
{
    using System.Text;
    using Configuration;
    using Metadata;
    using Util;


    public abstract class HostTopology :
        IHostTopology
    {
        protected static readonly INewIdFormatter Formatter = FormatUtil.Formatter;

        readonly ITopologyConfiguration _configuration;

        protected HostTopology(ITopologyConfiguration configuration)
        {
            _configuration = configuration;
        }

        public virtual string CreateTemporaryQueueName(string prefix)
        {
            var host = HostMetadataCache.Host;

            var sb = new StringBuilder(host.MachineName.Length + host.ProcessName.Length + prefix.Length + 36);

            foreach (var c in host.MachineName)
                if (char.IsLetterOrDigit(c) || c == '_')
                    sb.Append(c);

            sb.Append('_');
            foreach (var c in host.ProcessName)
                if (char.IsLetterOrDigit(c) || c == '_')
                    sb.Append(c);

            sb.AppendFormat("_{0}_", prefix);
            sb.Append(NewId.Next().ToString(Formatter));

            return sb.ToString();
        }

        public IPublishTopology PublishTopology => _configuration.Publish;
        public ISendTopology SendTopology => _configuration.Send;

        public IMessagePublishTopology<T> Publish<T>()
            where T : class
        {
            return _configuration.Publish.GetMessageTopology<T>();
        }

        public IMessageSendTopology<T> Send<T>()
            where T : class
        {
            return _configuration.Send.GetMessageTopology<T>();
        }

        public IMessageTopology<T> Message<T>()
            where T : class
        {
            return _configuration.Message.GetMessageTopology<T>();
        }
    }
}