using System;
using MassTransit.Transports;

namespace MassTransit.Configuration
{
    public static class MassTransitTransportFactoryOptionsExtensions
    {
        public static void AddTransportFactory<TTransportFactory>(this BusConfiguration cfg) where TTransportFactory : ITransportFactory
        {
            cfg.AddTransportFactory(typeof(TTransportFactory));
        }

        public static void AddTransportFactory(this BusConfiguration cfg, Type[] transportFactories)
        {
            transportFactories.Each(cfg.AddTransportFactory);
        }
    }
}