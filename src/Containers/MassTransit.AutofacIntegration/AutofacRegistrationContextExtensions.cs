namespace MassTransit
{
    using System;
    using Autofac;


    public static class AutofacRegistrationContextExtensions
    {
        [Obsolete("Please use Container.Resolve<T> instead")]
        public static T Resolve<T>(this IRegistrationContext<IComponentContext> context)
        {
            return context.Container.Resolve<T>();
        }
    }
}
