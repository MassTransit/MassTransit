namespace MassTransit.Internals.Extensions
{
    using System.Threading.Tasks;


    public class TaskCompletionSourceFactory
    {
        public static TaskCompletionSource<T> New<T>()
        {
        #if NETFRAMEWORK
            return new TaskCompletionSource<T>();
        #elif NETSTANDARD
            return new TaskCompletionSource<T>(TaskCreationOptions.RunContinuationsAsynchronously);
        #endif
        }
    }
}
