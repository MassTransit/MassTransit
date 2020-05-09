namespace MassTransit
{
    using System;
    using Microsoft.Extensions.DependencyInjection;


    public static class DependencyInjectionRegistrationContextExtensions
    {
        [Obsolete("Please use Container.GetService<T> instead")]
        public static T GetService<T>(this IRegistrationContext<IServiceProvider> registrationContext)
        {
            return registrationContext.Container.GetService<T>();
        }
        [Obsolete("Please use Container.GetRequiredService<T> instead")]
        public static T GetRequiredService<T>(this IRegistrationContext<IServiceProvider> registrationContext)
        {
            return registrationContext.Container.GetRequiredService<T>();
        }
    }
}
