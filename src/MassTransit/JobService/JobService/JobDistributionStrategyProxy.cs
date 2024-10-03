#nullable enable
namespace MassTransit.JobService;

using System.Threading.Tasks;
using Contracts.JobService;


public class JobDistributionStrategyProxy :
    IJobDistributionStrategy
{
    readonly IJobDistributionStrategy _strategy;

    public JobDistributionStrategyProxy()
    {
        _strategy = new DefaultJobDistributionStrategy();
    }

    public JobDistributionStrategyProxy(IJobDistributionStrategy strategy)
    {
        _strategy = strategy;
    }

    public Task<ActiveJob?> IsJobSlotAvailable(ConsumeContext<AllocateJobSlot> context, JobTypeInfo jobTypeInfo)
    {
        return _strategy.IsJobSlotAvailable(context, jobTypeInfo);
    }
}
