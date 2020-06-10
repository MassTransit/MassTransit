namespace MassTransit.Registration
{
    using System;
    using System.Collections.Generic;


    public interface IRegistrationCache<T> :
        IReadOnlyDictionary<Type, T>
    {
    }
}
