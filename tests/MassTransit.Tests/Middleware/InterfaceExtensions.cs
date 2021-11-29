namespace MassTransit.Tests.Middleware
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;


    public static class InterfaceExtensions
    {
        static readonly InterfaceReflectionCache _cache;

        static InterfaceExtensions()
        {
            _cache = new InterfaceReflectionCache();
        }

        /// <summary>
        /// Determines if the type is an open generic with at least one unspecified generic argument
        /// </summary>
        /// <param name="type">The type</param>
        /// <returns>True if the type is an open generic</returns>
        public static bool IsOpenGeneric(this Type type)
        {
            var typeInfo = type.GetTypeInfo();
            return typeInfo.IsGenericTypeDefinition || typeInfo.ContainsGenericParameters;
        }

        public static Type GetInterface(this Type type, Type interfaceType)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));
            if (interfaceType == null)
                throw new ArgumentNullException(nameof(interfaceType));

            var interfaceTypeInfo = interfaceType.GetTypeInfo();
            if (!interfaceTypeInfo.IsInterface)
                throw new ArgumentException("The interface type must be an interface: " + interfaceType.Name);

            return _cache.Get(type, interfaceType);
        }

        public static IEnumerable<Type> GetClosingArguments(this Type type, Type openType)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));
            if (openType == null)
                throw new ArgumentNullException(nameof(openType));

            if (!openType.IsOpenGeneric())
                throw new ArgumentException("The interface type must be an open generic interface: " + openType.Name);

            if (openType.GetTypeInfo().IsInterface)
            {
                if (!openType.IsOpenGeneric())
                    throw new ArgumentException("The interface type must be an open generic interface: " + openType.Name);

                var interfaceType = type.GetInterface(openType);
                if (interfaceType == null)
                    throw new ArgumentException("The interface type is not implemented by: " + type.Name);

                return interfaceType.GetTypeInfo().GetGenericArguments().Where(x => !x.IsGenericParameter);
            }

            var baseType = type;
            while (baseType != null && baseType != typeof(object))
            {
                var baseTypeInfo = baseType.GetTypeInfo();
                if (baseTypeInfo.IsGenericType && baseType.GetGenericTypeDefinition() == openType)
                    return baseTypeInfo.GetGenericArguments().Where(x => !x.IsGenericParameter);

                if (!baseTypeInfo.IsGenericType && baseType == openType)
                    return baseTypeInfo.GetGenericArguments().Where(x => !x.IsGenericParameter);

                baseType = baseTypeInfo.BaseType;
            }

            throw new ArgumentException("Could not find open type in type: " + type.Name);
        }
    }
}
