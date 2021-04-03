namespace MassTransit.Conductor.Distribution
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Contexts;

    public abstract class ServiceDistributionStrategy<TMessage> : IDistributionStrategy<ServiceInstanceContext, TMessage>
    {
        private readonly List<ServiceInstanceContext> _serviceInstances;

        public List<ServiceInstanceContext> ServiceInstances { get => _serviceInstances; }

        public ServiceDistributionStrategy()
        {

        }

        public void Add(ServiceInstanceContext node)
        {
            _serviceInstances.Add(node);
        }

        /// <summary>
        /// Use this method to select a node to send the message to.
        /// The property <see cref="ServiceInstances"/> contains all discovered instances.
        /// Return null if no node is available.
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public abstract Task<ServiceInstanceContext> GetNode(TMessage message);

        public virtual Task<IEnumerable<ServiceInstanceContext>> GetAvailableNodes()
        {
            // all nodes are available by default
            return Task.FromResult<IEnumerable<ServiceInstanceContext>>(ServiceInstances);
        }

        public void Init(IEnumerable<ServiceInstanceContext> nodes)
        {
            _serviceInstances.AddRange(nodes);
        }

        public void Remove(ServiceInstanceContext node)
        {
            _serviceInstances.Remove(node);
        }
    }
}
