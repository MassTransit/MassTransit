namespace MassTransit.Azure.ServiceBus.Core.Topology.Configurators
{
    using System;


    public abstract class EntityConfigurator :
        IEntityConfigurator
    {
        protected EntityConfigurator()
        {
            DefaultMessageTimeToLive = Defaults.DefaultMessageTimeToLive;
        }

        public TimeSpan? AutoDeleteOnIdle { get; set; }

        public TimeSpan? DefaultMessageTimeToLive { get; set; }

        public bool? EnableBatchedOperations { get; set; }

        public string UserMetadata { get; set; }
    }
}
