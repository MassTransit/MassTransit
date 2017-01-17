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
namespace MassTransit.Topology.Observers
{
    using System;
    using Configuration;
    using Context;
    using GreenPipes;
    using GreenPipes.Filters;


    public class PublishToSendTopologyConfigurationObserver :
        IPublishTopologyConfigurationObserver
    {
        readonly ISendTopology _sendTopology;

        public PublishToSendTopologyConfigurationObserver(ISendTopology sendTopology)
        {
            _sendTopology = sendTopology;
        }

        public void MessageTopologyCreated<T>(IMessagePublishTopologyConfigurator<T> configurator)
            where T : class
        {
            IMessageSendTopology<T> messageSendTopology = _sendTopology.GetMessageTopology<T>();

            configurator.AddDelegate(new Proxy<T>(messageSendTopology));
        }


        class Proxy<T> :
            IMessagePublishTopology<T>
            where T : class
        {
            readonly IMessageSendTopology<T> _topology;

            public Proxy(IMessageSendTopology<T> topology)
            {
                _topology = topology;
            }

            public void Apply(ITopologyPipeBuilder<PublishContext<T>> builder)
            {
                var sendBuilder = new Builder(builder);

                _topology.Apply(sendBuilder);
            }

            public bool TryGetPublishAddress(Uri baseAddress, T message, out Uri publishAddress)
            {
                publishAddress = null;
                return false;
            }

            public IMessageEntityNameFormatter<T> EntityNameFormatter => null;


            class Builder :
                ITopologyPipeBuilder<SendContext<T>>
            {
                readonly ITopologyPipeBuilder<PublishContext<T>> _builder;

                public Builder(ITopologyPipeBuilder<PublishContext<T>> builder)
                {
                    _builder = builder;
                }

                public void AddFilter(IFilter<SendContext<T>> filter)
                {
                    var splitFilter = new SplitFilter<PublishContext<T>, SendContext<T>>(filter, MergeContext, FilterContext);

                    _builder.AddFilter(splitFilter);
                }

                public bool IsDelegated => _builder.IsDelegated;
                public bool IsImplemented => _builder.IsImplemented;

                public ITopologyPipeBuilder<SendContext<T>> CreateDelegatedBuilder()
                {
                    return new ChildBuilder<SendContext<T>>(this, IsImplemented, true);
                }

                static SendContext<T> FilterContext(PublishContext<T> context)
                {
                    return context;
                }

                static PublishContext<T> MergeContext(PublishContext<T> input, SendContext context)
                {
                    var result = context as PublishContext<T>;

                    return result ?? new PublishContextProxy<T>(context, input.Message);
                }


                class ChildBuilder<T> :
                    ITopologyPipeBuilder<T>
                    where T : class, PipeContext
                {
                    readonly ITopologyPipeBuilder<T> _builder;

                    public ChildBuilder(ITopologyPipeBuilder<T> builder, bool isImplemented, bool isDelegated)
                    {
                        _builder = builder;

                        IsDelegated = isDelegated;
                        IsImplemented = isImplemented;
                    }

                    public void AddFilter(IFilter<T> filter)
                    {
                        _builder.AddFilter(filter);
                    }

                    public bool IsDelegated { get; }

                    public bool IsImplemented { get; }

                    public ITopologyPipeBuilder<T> CreateDelegatedBuilder()
                    {
                        return new ChildBuilder<T>(this, IsImplemented, true);
                    }
                }
            }
        }
    }
}