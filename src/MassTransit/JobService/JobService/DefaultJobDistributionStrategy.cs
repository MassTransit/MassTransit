#nullable enable
namespace MassTransit.JobService;

using System.Linq;
using System.Threading.Tasks;
using Contracts.JobService;


public class DefaultJobDistributionStrategy :
    IJobDistributionStrategy
{
    public static readonly IJobDistributionStrategy Instance = new DefaultJobDistributionStrategy();

    public async Task<ActiveJob?> IsJobSlotAvailable(ConsumeContext<AllocateJobSlot> context, JobTypeInfo jobTypeInfo)
    {
        var instances = from i in jobTypeInfo.Instances
            join a in jobTypeInfo.ActiveJobs on i.Key equals a.InstanceAddress into ai
            where ai.Count() < jobTypeInfo.ConcurrentJobLimit
            orderby ai.Count(), i.Value.Used
            select new
            {
                Instance = i.Value,
                InstanceAddress = i.Key,
                InstanceCount = ai.Count()
            };

        var firstInstance = instances.FirstOrDefault();
        if (firstInstance == null)
            return null;

        return new ActiveJob
        {
            JobId = context.Message.JobId,
            InstanceAddress = firstInstance.InstanceAddress
        };
    }
}
