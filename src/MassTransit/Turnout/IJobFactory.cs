namespace MassTransit.Turnout
{
    using System.Threading.Tasks;


    public interface IJobFactory<in TJob>
        where TJob : class
    {
        Task Execute(JobContext<TJob> context);
    }
}
