namespace MassTransit.Conductor
{
    using System;
    using System.Collections.Generic;
    using Directory;


    public interface IServiceRegistration<TResult> :
        IServiceRegistration
        where TResult : class
    {
        void AddProvider<TInput>(IProviderRegistration<TInput, TResult> registration)
            where TInput : class;
    }


    public interface IServiceRegistration
    {
        Type ServiceType { get; }
        IEnumerable<IProviderRegistration> Providers { get; }
    }
}
