using System;

namespace MassTransit.ServiceBus
{
    public interface IAddressBook
    {
        IEndpoint Resolve(Type t);
    }
}