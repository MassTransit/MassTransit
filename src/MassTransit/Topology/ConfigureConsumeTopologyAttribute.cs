namespace MassTransit.Topology
{
    using System;


    /// <summary>
    /// Specify whether the message type should be used to configure the broker topology for the consumer.
    /// if configured.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface)]
    public class ConfigureConsumeTopologyAttribute :
        Attribute
    {
        public ConfigureConsumeTopologyAttribute()
        {
            ConfigureConsumeTopology = true;
        }

        /// <param name="configureConsumeTopology">When false, the consume topology will not be configured</param>
        public ConfigureConsumeTopologyAttribute(bool configureConsumeTopology)
        {
            ConfigureConsumeTopology = configureConsumeTopology;
        }

        public bool ConfigureConsumeTopology { get; }
    }
}
