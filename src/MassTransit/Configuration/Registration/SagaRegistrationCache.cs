namespace MassTransit.Registration
{
    using System;
    using Saga;


    public static class SagaRegistrationCache
    {
        public static void Register(Type sagaType, IContainerRegistrar registrar)
        {
            Cached.Instance.GetOrAdd(sagaType).Register(registrar);
        }

        public static ISagaRegistration CreateRegistration(Type sagaType, IContainerRegistrar registrar)
        {
            return Cached.Instance.GetOrAdd(sagaType).CreateRegistration(registrar);
        }

        /// <summary>
        /// Sets a saga type so that it will not be registered. This is used to allow state machines to register without a conflicting
        /// standard saga from also being registered.
        /// </summary>
        /// <param name="sagaType"></param>
        public static void DoNotRegister(Type sagaType)
        {
            Cached.Instance.GetOrAdd(sagaType).DoNotRegister();
        }

        static CachedRegistration Factory(Type type)
        {
            return (CachedRegistration)Activator.CreateInstance(typeof(CachedRegistration<>).MakeGenericType(type));
        }


        static class Cached
        {
            internal static readonly RegistrationCache<CachedRegistration> Instance = new RegistrationCache<CachedRegistration>(Factory);
        }


        interface CachedRegistration
        {
            void Register(IContainerRegistrar registrar);
            ISagaRegistration CreateRegistration(IContainerRegistrar registrar);
            void DoNotRegister();
        }


        class CachedRegistration<T> :
            CachedRegistration
            where T : class, ISaga
        {
            bool _doNotRegister;
            IContainerRegistrar _registrar;

            public void Register(IContainerRegistrar registrar)
            {
                _registrar = registrar;
                if (_doNotRegister)
                    return;

                _registrar.RegisterSaga<T>();
            }

            public ISagaRegistration CreateRegistration(IContainerRegistrar registrar)
            {
                Register(registrar);

                return new SagaRegistration<T>();
            }

            public void DoNotRegister()
            {
                _doNotRegister = true;
            }
        }
    }
}
