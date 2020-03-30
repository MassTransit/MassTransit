namespace MassTransit.Internals.Reflection
{
    using System;
    using System.Diagnostics;
    using System.Linq.Expressions;
    using System.Reflection;
    using System.Threading;
    using System.Threading.Tasks;
    using MassTransit;


    public class WriteProperty<T, TProperty> :
        IWriteProperty<T, TProperty>
        where T : class
    {
        Action<T, TProperty> _setMethod;

        public WriteProperty(Type implementationType, PropertyInfo propertyInfo)
        {
            TargetType = implementationType;

            var setMethod = propertyInfo.GetSetMethod(true);
            if (setMethod == null)
                throw new ArgumentException($"The property does not have an accessible set method: {propertyInfo.Name}");

            // look for <Address>k__BackingField and use a field setter if available

            void SetUsingReflection(T entity, TProperty property) => setMethod.Invoke(entity, new object[] {property});

            void Initialize(T entity, TProperty property)
            {
                Interlocked.Exchange(ref _setMethod, SetUsingReflection);

                SetUsingReflection(entity, property);

                Task.Run(() => GenerateExpressionSetMethod(implementationType, setMethod));
            }

            _setMethod = Initialize;
        }

        public Type TargetType { get; }

        public void Set(T content, TProperty value)
        {
            _setMethod(content, value);
        }

        async Task GenerateExpressionSetMethod(Type implementationType, MethodInfo setMethod)
        {
            try
            {
                var fastSetMethod = CompileSetMethod(implementationType, setMethod);

                Interlocked.Exchange(ref _setMethod, fastSetMethod);
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

        static Action<T, TProperty> CompileSetMethod(Type implementationType, MethodInfo setMethod)
        {
            try
            {
                var instance = Expression.Parameter(typeof(T), "instance");
                var value = Expression.Parameter(typeof(TProperty), "value");
                var cast = Expression.TypeAs(instance, implementationType);

                var call = Expression.Call(cast, setMethod, value);

                var lambdaExpression = Expression.Lambda<Action<T, TProperty>>(call, instance, value);

                return lambdaExpression.CompileFast<Action<T, TProperty>>();
            }
            catch (Exception ex)
            {
                throw new MassTransitException($"Failed to compile SetMethod for property {setMethod.Name} on entity {typeof(T).Name}", ex);
            }
        }
    }
}
