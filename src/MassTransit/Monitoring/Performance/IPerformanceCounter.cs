namespace MassTransit.Monitoring.Performance
{
    using System;


    public interface IPerformanceCounter :
        IDisposable
    {
        void Increment();
        void IncrementBy(long val);
        void Set(long val);
    }
}
