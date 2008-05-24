namespace MassTransit.ServiceBus
{
    using System;

    public class ActivatorObjectBuilder : IObjectBuilder
    {
        public object Build(Type objectType)
        {
            return Activator.CreateInstance(objectType);
        }

        public T Build<T>(Type type) where T : class
        {
            return Build(type) as T;
        }
    }
}