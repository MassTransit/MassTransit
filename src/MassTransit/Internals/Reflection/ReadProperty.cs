namespace MassTransit.Internals
{
    using System;
    using System.Linq.Expressions;
    using System.Reflection;
    using System.Threading;
    using System.Threading.Tasks;


    public class ReadProperty<T, TProperty> :
        IReadProperty<T, TProperty>
        where T : class
    {
        Func<T, TProperty> _getMethod;

        public ReadProperty(PropertyInfo propertyInfo)
        {
            if (propertyInfo == null)
                throw new ArgumentNullException(nameof(propertyInfo));

            var getMethod = propertyInfo.GetGetMethod(true);
            if (getMethod == null)
                throw new ArgumentException("The property does not have an accessible get method");

            TProperty GetUsingReflection(T entity)
            {
                return (TProperty)getMethod.Invoke(entity, null);
            }

            TProperty Initialize(T entity)
            {
                Interlocked.Exchange(ref _getMethod, GetUsingReflection);

                Task.Run(() => GenerateExpressionGetMethod(getMethod));

                return GetUsingReflection(entity);
            }

            _getMethod = Initialize;
        }

        public TProperty Get(T content)
        {
            return _getMethod(content);
        }

        async Task GenerateExpressionGetMethod(MethodInfo getMethod)
        {
            try
            {
                Func<T, TProperty> method = CompileGetMethod(getMethod);

                Interlocked.Exchange(ref _getMethod, method);
            }
            catch (Exception)
            {
            }
        }

        static Func<T, TProperty> CompileGetMethod(MethodInfo getMethod)
        {
            try
            {
                var instance = Expression.Parameter(typeof(T), "instance");
                var call = Expression.Call(instance, getMethod);

                Expression<Func<T, TProperty>> lambdaExpression = Expression.Lambda<Func<T, TProperty>>(call, instance);

                return lambdaExpression.CompileFast<Func<T, TProperty>>();
            }
            catch (Exception ex)
            {
                throw new MassTransitException($"Failed to compile get method for property {getMethod.Name} on entity {typeof(T).Name}", ex);
            }
        }
    }
}
