namespace DistributedGrid.Worker
{
    using System;
    using Shared;
    using StructureMap;

    public class WorkerServiceProvider : ServiceSetup
    {
        public override string ServiceName { get; set; }
        public override string DisplayName { get; set; }
        public override string Description { get; set; }
        public override string SourceQueue { get; set; }
        public override string SubscriptionQueue { get; set; }
        public override Action<ConfigurationExpression> ContainerSetup { get; set; }
    }
}