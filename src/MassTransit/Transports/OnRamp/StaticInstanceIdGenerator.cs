using System.Threading;
using System.Threading.Tasks;

namespace MassTransit.Transports.OnRamp
{
    public class StaticInstanceIdGenerator : IInstanceIdGenerator
    {
        private readonly string _instanceId;

        public StaticInstanceIdGenerator(string instanceId)
        {
            _instanceId = instanceId;
        }

        public Task<string> GenerateInstanceId(CancellationToken cancellationToken = default)
        {
            return Task.FromResult(_instanceId);
        }
    }
}
