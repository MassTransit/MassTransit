using System.Threading;
using System.Threading.Tasks;

namespace MassTransit.AmazonSqsTransport
{
    public class Window
    {
        private readonly SemaphoreSlim semaphore; 

        public int AvailableThreads
        {
            get
            {
                return semaphore.CurrentCount;
            }
        }

        public Window(int maxThreads)
        {
            semaphore = new SemaphoreSlim(maxThreads, maxThreads);
        }

        /// <summary>
        /// Open window when any thread finishes
        /// </summary>
        public void Open()
        {
            semaphore.Release(1);
        }

        /// <summary>
        /// Wait if max amount of active threads is reached. 
        /// </summary>
        /// <returns></returns>
        public Task Wait()
        {
            return semaphore.WaitAsync();
        }
    }
}
