namespace MassTransit.Caching
{
    using System;


    /// <summary>
    /// If a cached value implments this interface, the cache will attach itself to the
    /// event so the value can signal usage to update the lifetime of the value.
    /// </summary>
    public interface INotifyValueUsed
    {
        /// <summary>
        /// Should be raised by the value when used, to keep it alive in the cache.
        /// </summary>
        event Action Used;
    }
}
