namespace MassTransit.Configuration
{
    using System;
    using ExceptionFilters;


    public abstract class ExceptionSpecification :
        IExceptionConfigurator
    {
        readonly CompositeFilter<Exception> _exceptionFilter;

        protected ExceptionSpecification()
        {
            _exceptionFilter = new CompositeFilter<Exception>();
            Filter = new CompositeExceptionFilter(_exceptionFilter);
        }

        protected IExceptionFilter Filter { get; }

        public void Handle(params Type[] exceptionTypes)
        {
            _exceptionFilter.Includes += exception => Match(exception, exceptionTypes);
        }

        public void Handle<T>()
            where T : Exception
        {
            _exceptionFilter.Includes += exception => Match(exception, typeof(T));
        }

        public void Handle<T>(Func<T, bool> filter)
            where T : Exception
        {
            _exceptionFilter.Includes += exception => Match(exception, filter);
        }

        public void Ignore(params Type[] exceptionTypes)
        {
            _exceptionFilter.Excludes += exception => Match(exception, exceptionTypes);
        }

        public void Ignore<T>()
            where T : Exception
        {
            _exceptionFilter.Excludes += exception => Match(exception, typeof(T));
        }

        public void Ignore<T>(Func<T, bool> filter)
            where T : Exception
        {
            _exceptionFilter.Excludes += exception => Match(exception, filter);
        }

        static bool Match(Exception exception, params Type[] exceptionTypes)
        {
            var baseException = exception.GetBaseException();

            for (var i = 0; i < exceptionTypes.Length; i++)
            {
                if (exceptionTypes[i].IsInstanceOfType(exception))
                    return true;

                if (exceptionTypes[i].IsInstanceOfType(baseException))
                    return true;
            }

            return false;
        }

        static bool Match<T>(Exception exception, Func<T, bool> filter)
            where T : Exception
        {
            if (exception is T ofT)
                return filter(ofT);

            var baseException = exception.GetBaseException();

            return baseException is T exceptionOfT && filter(exceptionOfT);
        }
    }
}
