namespace MassTransit.ServiceBus.Internal
{
    using System;

    public interface IEndpointResolver
    {
        IEndpoint Resolve(Uri uri);
    }
}