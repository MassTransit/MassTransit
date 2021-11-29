namespace MassTransit
{
    using System;


    public interface IExceptionConfigurator
    {
        void Handle(params Type[] exceptionTypes);

        void Handle<T>()
            where T : Exception;

        void Handle<T>(Func<T, bool> filter)
            where T : Exception;

        void Ignore(params Type[] exceptionTypes);

        void Ignore<T>()
            where T : Exception;

        void Ignore<T>(Func<T, bool> filter)
            where T : Exception;
    }
}
