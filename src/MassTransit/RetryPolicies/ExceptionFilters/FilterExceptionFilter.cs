namespace MassTransit.RetryPolicies.ExceptionFilters
{
    using System;


    public class FilterExceptionFilter<T> :
        IExceptionFilter
        where T : Exception
    {
        readonly Func<T, bool> _filter;

        public FilterExceptionFilter(Func<T, bool> filter)
        {
            _filter = filter;
        }

        void IProbeSite.Probe(ProbeContext context)
        {
            var scope = context.CreateScope("filter");
            scope.Set(new
            {
                ExceptionType = typeof(T).Name
            });
        }

        bool IExceptionFilter.Match(Exception exception)
        {
            var currentException = exception;
            while (currentException != null)
            {
                if (exception is T ex)
                    return _filter(ex);

                currentException = currentException.GetBaseException();
            }

            return true;
        }
    }
}
