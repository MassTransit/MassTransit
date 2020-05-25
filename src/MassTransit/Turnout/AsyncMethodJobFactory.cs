namespace MassTransit.Turnout
{
    using System;
    using System.Threading.Tasks;


    public class AsyncMethodJobFactory<TInput> :
        IJobFactory<TInput>
        where TInput : class
    {
        readonly Func<JobContext<TInput>, Task> _method;

        public AsyncMethodJobFactory(Func<JobContext<TInput>, Task> method)
        {
            _method = method;
        }

        public Task Execute(JobContext<TInput> context)
        {
            return _method(context);
        }
    }
}
