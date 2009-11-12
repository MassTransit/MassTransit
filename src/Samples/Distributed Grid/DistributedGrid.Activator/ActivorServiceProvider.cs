using System;
using DistributedGrid.Shared;
using StructureMap;

namespace DistributedGrid.Activator
{
    public class ActivorServiceProvider : ServiceSetup
    {
        public override string ServiceName { get; set; }
        public override string DisplayName { get; set; }
        public override string Description { get; set; }
        public override string SourceQueue { get; set; }
        public override string SubscriptionQueue { get; set; }
        public override Action<ConfigurationExpression> ContainerSetup { get; set; }
    }
}