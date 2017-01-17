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
namespace MassTransit.Topology
{
    using System;
    using System.Threading;
    using Configuration;


    public class GlobalMessageTopology<TMessage> :
        IGlobalMessageTopology<TMessage>
        where TMessage : class
    {
        readonly IMessagePublishTopologyConfigurator<TMessage> _publish;
        readonly IMessageSendTopologyConfigurator<TMessage> _send;

        GlobalMessageTopology(IMessageSendTopologyConfigurator<TMessage> send, IMessagePublishTopologyConfigurator<TMessage> publish)
        {
            _send = send;
            _publish = publish;
        }

        public static IMessageSendTopologyConfigurator<TMessage> Send => Cached.Metadata.Value.Send;
        public static IMessagePublishTopologyConfigurator<TMessage> Publish => Cached.Metadata.Value.Publish;

        IMessageSendTopologyConfigurator<TMessage> IGlobalMessageTopology<TMessage>.Send => _send;
        IMessagePublishTopologyConfigurator<TMessage> IGlobalMessageTopology<TMessage>.Publish => _publish;


        static class Cached
        {
            internal static readonly Lazy<IGlobalMessageTopology<TMessage>> Metadata = new Lazy<IGlobalMessageTopology<TMessage>>(() =>
            {
                IMessageSendTopologyConfigurator<TMessage> sendTopology = GlobalTopology.Send.GetMessageTopology<TMessage>();
                IMessagePublishTopologyConfigurator<TMessage> publishTopology = GlobalTopology.Publish.GetMessageTopology<TMessage>();

                return new GlobalMessageTopology<TMessage>(sendTopology, publishTopology);
            }, LazyThreadSafetyMode.PublicationOnly);
        }
    }
}