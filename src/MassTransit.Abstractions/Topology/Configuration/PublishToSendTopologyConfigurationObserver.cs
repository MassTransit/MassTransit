namespace MassTransit.Configuration
{
    using System;
    using Middleware;


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


        class Proxy<TMessage> :
            IMessagePublishTopology<TMessage>
            where TMessage : class
        {
            readonly IMessageSendTopology<TMessage> _topology;

            public Proxy(IMessageSendTopology<TMessage> topology)
            {
                _topology = topology;
            }

            public void Apply(ITopologyPipeBuilder<PublishContext<TMessage>> builder)
            {
                var sendBuilder = new Builder(builder);

                _topology.Apply(sendBuilder);
            }

            public bool TryGetPublishAddress(Uri baseAddress, out Uri? publishAddress)
            {
                publishAddress = null;
                return false;
            }


            class Builder :
                ITopologyPipeBuilder<SendContext<TMessage>>
            {
                readonly ITopologyPipeBuilder<PublishContext<TMessage>> _builder;

                public Builder(ITopologyPipeBuilder<PublishContext<TMessage>> builder)
                {
                    _builder = builder;
                }

                public void AddFilter(IFilter<SendContext<TMessage>> filter)
                {
                    var splitFilter = new SplitFilter<PublishContext<TMessage>, SendContext<TMessage>>(filter, MergeContext, FilterContext);

                    _builder.AddFilter(splitFilter);
                }

                public bool IsDelegated => _builder.IsDelegated;
                public bool IsImplemented => _builder.IsImplemented;

                public ITopologyPipeBuilder<SendContext<TMessage>> CreateDelegatedBuilder()
                {
                    return new ChildBuilder<SendContext<TMessage>>(this, IsImplemented, true);
                }

                static SendContext<TMessage> FilterContext(PublishContext<TMessage> context)
                {
                    return context;
                }

                static PublishContext<TMessage> MergeContext(PublishContext<TMessage> input, SendContext context)
                {
                    return context.GetPayload<PublishContext<TMessage>>();
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
