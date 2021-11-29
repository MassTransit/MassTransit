namespace MassTransit.RetryPolicies.ExceptionFilters
{
    using System;
    using System.Linq;
    using System.Reflection;


    public class HandleExceptionFilter :
        IExceptionFilter
    {
        readonly Type[] _exceptionTypes;

        public HandleExceptionFilter(params Type[] exceptionTypes)
        {
            _exceptionTypes = exceptionTypes;
        }

        void IProbeSite.Probe(ProbeContext context)
        {
            var scope = context.CreateScope("selected");
            scope.Set(new
            {
                ExceptionTypes = _exceptionTypes.Select(x => x.Name).ToArray()
            });
        }

        bool IExceptionFilter.Match(Exception exception)
        {
            for (var i = 0; i < _exceptionTypes.Length; i++)
            {
                if (_exceptionTypes[i].GetTypeInfo().IsInstanceOfType(exception))
                    return true;
            }

            return false;
        }
    }
}
