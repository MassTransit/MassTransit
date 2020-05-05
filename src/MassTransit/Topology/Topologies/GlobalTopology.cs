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
            _send.TryAddConvention(new CorrelationIdSendTopologyConvention());

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
