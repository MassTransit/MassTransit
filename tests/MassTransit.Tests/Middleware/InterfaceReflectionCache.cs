namespace MassTransit.Tests.Middleware
{
    using System;
    using System.Collections.Concurrent;
    using System.Linq;
    using System.Reflection;


    public class InterfaceReflectionCache
    {
        readonly ConcurrentDictionary<Type, ConcurrentDictionary<Type, Type>> _cache;

        public InterfaceReflectionCache()
        {
            _cache = new ConcurrentDictionary<Type, ConcurrentDictionary<Type, Type>>();
        }

        public Type GetGenericInterface(Type type, Type interfaceType)
        {
            if (!interfaceType.GetTypeInfo().IsGenericTypeDefinition)
            {
                throw new ArgumentException(
                    "The interface must be a generic interface definition: " + interfaceType.Name,
                    nameof(interfaceType));
            }

            // our contract states that we will not return generic interface definitions without generic type arguments
            if (type == interfaceType)
                return null;

            if (type.GetTypeInfo().IsGenericType)
            {
                if (type.GetGenericTypeDefinition() == interfaceType)
                    return type;
            }

            Type[] interfaces = type.GetTypeInfo().ImplementedInterfaces.ToArray();

            return interfaces.Where(t => t.GetTypeInfo().IsGenericType)
                .FirstOrDefault(t => t.GetGenericTypeDefinition() == interfaceType);
        }

        public Type Get(Type type, Type interfaceType)
        {
            ConcurrentDictionary<Type, Type> typeCache = _cache.GetOrAdd(type, x => new ConcurrentDictionary<Type, Type>());

            return typeCache.GetOrAdd(interfaceType, x => GetInterfaceInternal(type, interfaceType));
        }

        Type GetInterfaceInternal(Type type, Type interfaceType)
        {
            if (interfaceType.GetTypeInfo().IsGenericTypeDefinition)
                return GetGenericInterface(type, interfaceType);

            Type[] interfaces = type.GetTypeInfo().ImplementedInterfaces.ToArray();

            return interfaces.FirstOrDefault(t => t == interfaceType);
        }
    }
}
