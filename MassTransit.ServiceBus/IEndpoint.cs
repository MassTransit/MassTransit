using System;

namespace MassTransit.ServiceBus
{
    public interface IEndpoint :
        IDisposable
    {
		Uri Uri { get; }
    }
}