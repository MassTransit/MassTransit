namespace RapidTransit
{
    using Autofac;
    using Topshelf;


    public delegate ServiceControl ScopedServiceFactory(ILifetimeScope lifetimeScope);
}