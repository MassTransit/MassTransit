namespace MassTransit.JobService.Configuration
{
    public class JobConsumerConvention :
        IConsumerConvention
    {
        IConsumerMessageConvention IConsumerConvention.GetConsumerMessageConvention<T>()
        {
            return new JobConsumerMessageConvention<T>();
        }
    }
}
