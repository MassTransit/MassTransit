namespace MassTransit.Transports
{
    using System.Collections.Generic;


    public interface ITransportSetHeaderAdapter<TValueType>
    {
        void Set(IDictionary<string, TValueType> dictionary, in HeaderValue headerValue);
        void Set<T>(IDictionary<string, TValueType> dictionary, in HeaderValue<T> headerValue);
    }
}
