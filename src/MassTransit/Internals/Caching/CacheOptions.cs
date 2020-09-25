namespace MassTransit.Internals.Caching
{
    using System;


    public class CacheOptions
    {
        public CacheOptions()
        {
            ConcurrencyLevel = Environment.ProcessorCount;
            Capacity = 1000;
        }

        public int ConcurrencyLevel { get; set; }
        public int Capacity { get; set; }
    }
}
