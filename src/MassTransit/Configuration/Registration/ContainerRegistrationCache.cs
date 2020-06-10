namespace MassTransit.Registration
{
    using System;


    public class ContainerRegistrationCache :
        RegistrationCache<CachedRegistration>
    {
        public ContainerRegistrationCache(Func<Type, CachedRegistration> missingRegistrationFactory = default)
            : base(missingRegistrationFactory)
        {
        }
    }
}
