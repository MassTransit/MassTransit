namespace MassTransit.WindsorIntegration.Registration
{
    using System;
    using Castle.MicroKernel;
    using MassTransit.Registration;


    public class WindsorConfigurationServiceProvider :
        IConfigurationServiceProvider
    {
        readonly IKernel _kernel;

        public WindsorConfigurationServiceProvider(IKernel kernel)
        {
            _kernel = kernel;
        }

        public T GetRequiredService<T>()
            where T : class
        {
            return _kernel.Resolve<T>();
        }

        public T GetService<T>()
            where T : class
        {
            return _kernel.HasComponent(typeof(T)) ? _kernel.Resolve<T>() : null;
        }

        public object GetService(Type serviceType)
        {
            return _kernel.HasComponent(serviceType) ? _kernel.Resolve(serviceType) : null;
        }
    }
}
