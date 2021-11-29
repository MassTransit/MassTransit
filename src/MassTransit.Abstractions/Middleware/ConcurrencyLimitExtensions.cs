#nullable enable
namespace MassTransit
{
    using System;
    using System.Threading.Tasks;
    using Contracts;


    public static class ConcurrencyLimitExtensions
    {
        /// <summary>
        /// Set the concurrency limit of the filter
        /// </summary>
        /// <param name="pipe"></param>
        /// <param name="concurrencyLimit"></param>
        /// <returns></returns>
        public static Task SetConcurrencyLimit(this IPipe<CommandContext> pipe, int concurrencyLimit)
        {
            return pipe.SendCommand<SetConcurrencyLimit>(new Limit(concurrencyLimit));
        }


        class Limit :
            SetConcurrencyLimit
        {
            public Limit(int concurrencyLimit)
            {
                ConcurrencyLimit = concurrencyLimit;

                Timestamp = DateTime.UtcNow;
            }

            public DateTime? Timestamp { get; }
            public string? Id => null;
            public int ConcurrencyLimit { get; }
        }
    }
}
