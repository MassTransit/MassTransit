using System;
using System.Threading;
using System.Threading.Tasks;

namespace MassTransit.Transports.OnRamp
{
    public class OnRampInstanceState
    {
        private readonly IInstanceIdGenerator _instanceIdGenerator;

        public OnRampInstanceState(IInstanceIdGenerator instanceIdGenerator)
        {
            _instanceIdGenerator = instanceIdGenerator;
        }

        public string InstanceId { get; private set; }
        public bool FirstCheckin { get; set; } = true;
        public CancellationTokenSource BusHealthToken { get; set; }
        public bool BusHealthy  { get; set; } = false;
        public DateTime LastCheckin { get; set; } = DateTime.UtcNow;

        public async Task Initialize()
        {
            // We know it's not initialized if the InstanceId is null
            if(InstanceId == null)
            {
                InstanceId = await _instanceIdGenerator.GenerateInstanceId();
            }
        }
    }
}
