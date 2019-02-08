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
    using System;
    using System.Threading;
    using Conventions;
    using GreenPipes;
    using Observers;


    /// <summary>
    /// This represents the global topology configuration, which is delegated to by
    /// all topology instances, unless for some radical reason a bus is configured
    /// without any topology delegation.
    /// 
    /// YES, I hate globals, but they are serving a purpose in that these are really
    /// just defining the default behavior of message types, rather than actually
    /// behaving like the nasty evil global variables.
    /// </summary>
    public class GlobalTopology :
        IGlobalTopology
    {
        readonly IPublishTopologyConfigurator _publish;
        readonly ConnectHandle _publishToSendHandle;
        readonly ISendTopologyConfigurator _send;

        GlobalTopology()
        {
            _send = new SendTopology();
            _send.AddConvention(new CorrelationIdSendTopologyConvention());

            _publish = new PublishTopology();

            var observer = new PublishToSendTopologyConfigurationObserver(_send);
            _publishToSendHandle = _publish.ConnectPublishTopologyConfigurationObserver(observer);
        }

        public static ISendTopologyConfigurator Send => Cached.Metadata.Value.Send;
        public static IPublishTopologyConfigurator Publish => Cached.Metadata.Value.Publish;

        void IGlobalTopology.SeparatePublishFromSend()
        {
            _publishToSendHandle.Disconnect();
        }

        ISendTopologyConfigurator IGlobalTopology.Send => _send;
        IPublishTopologyConfigurator IGlobalTopology.Publish => _publish;

        /// <summary>
        /// Call before configuring any topology, so that publish is handled separately
        /// from send. Note, this can cause some really bad things to happen with internal
        /// types so use with caution...
        /// </summary>
        public static void SeparatePublishFromSend()
        {
            Cached.Metadata.Value.SeparatePublishFromSend();
        }


        static class Cached
        {
            internal static readonly Lazy<IGlobalTopology> Metadata = new Lazy<IGlobalTopology>(() => new GlobalTopology(), LazyThreadSafetyMode.PublicationOnly);
        }
    }
}