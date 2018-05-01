namespace MassTransit.Internals.Reflection
{
    using System;
    using System.Diagnostics;
    using System.Linq.Expressions;
    using System.Reflection;
    using System.Threading;
    using System.Threading.Tasks;
    using MassTransit;


    public class ReadProperty<T, TProperty> :
        IReadProperty<T, TProperty>
    {
        Func<T, TProperty> _getMethod;

        public ReadProperty(Type implementationType, string propertyName)
        {
            if (typeof(T).GetTypeInfo().IsValueType)
                throw new ArgumentException("The message type must be a reference type");

            var propertyInfo = implementationType.GetProperty(propertyName);
            if (propertyInfo == null)
                throw new ArgumentException("The implementation does not have a property named: " + propertyName);

            var getMethod = propertyInfo.GetGetMethod(true);
            if (getMethod == null)
                throw new ArgumentException("The property does not have an accessible get method");

            Name = propertyName;

            TProperty GetUsingReflection(T entity) => (TProperty)getMethod.Invoke(entity, null);

            TProperty Initialize(T entity)
            {
                Interlocked.Exchange(ref _getMethod, GetUsingReflection);

                Task.Factory.StartNew(() => GenerateExpressionGetMethod(implementationType, getMethod),
                    CancellationToken.None, TaskCreationOptions.None, TaskScheduler.Default);

                return GetUsingReflection(entity);
            }

            _getMethod = Initialize;
        }

        public string Name { get; }

        public TProperty Get(T content)
        {
            return _getMethod(content);
        }

        async Task GenerateExpressionGetMethod(Type implementationType, MethodInfo getMethod)
        {
            await Task.Yield();

            try
            {
                var method = CompileGetMethod(implementationType, getMethod);

                Interlocked.Exchange(ref _getMethod, method);
            }
            #if NETCORE
            catch (Exception)
            {
            }
            #else
            catch (Exception ex)
            {
                if (Trace.Listeners.Count > 0)
                    Trace.WriteLine(ex.Message);
            }
            #endif
        }

        static Func<T, TProperty> CompileGetMethod(Type implementationType, MethodInfo getMethod)
        {
            try
            {
                var instance = Expression.Parameter(typeof(T), "instance");
                var cast = Expression.TypeAs(instance, implementationType);

                var call = Expression.Call(cast, getMethod);

                var lambdaExpression = Expression.Lambda<Func<T, TProperty>>(call, instance);

                return ExpressionCompiler.Compile<Func<T, TProperty>>(lambdaExpression);
            }
            catch (Exception ex)
            {
                throw new MassTransitException($"Failed to compile get method for property {getMethod.Name} on entity {typeof(T).Name}", ex);
            }
        }
    }
}