namespace MassTransit.Conductor.Distribution
{
    using System.Threading.Tasks;
    using Contexts;

    public class RoundRobinDistributionStrategy : ServiceDistributionStrategy<object>
    {
        private int _currentIndex = 0;
        private object lockObject;

        public override Task<ServiceInstanceContext> GetNode(object message)
        {
            if (ServiceInstances.Count == 0)
            {
                return Task.FromResult<ServiceInstanceContext>(null);
            }

            int index;
            lock (lockObject)
            {
                _currentIndex = (_currentIndex + 1) % ServiceInstances.Count;
                index = _currentIndex;
            }

            return Task.FromResult(ServiceInstances[index]);
        }
    }
}
