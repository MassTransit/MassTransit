namespace MassTransit.Internals.Caching
{
    using System;


    public class CacheOptions
    {
        int _capacity;

        public CacheOptions()
        {
            ConcurrencyLevel = Environment.ProcessorCount;
            Capacity = 1000;
        }

        public int ConcurrencyLevel { get; set; }

        public int Capacity
        {
            get => _capacity;
            set => _capacity = Math.Max(8, value);
        }
    }
}
