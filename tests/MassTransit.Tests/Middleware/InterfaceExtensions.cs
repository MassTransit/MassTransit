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
            return type.IsGenericTypeDefinition || type.ContainsGenericParameters;
        }

        public static Type GetInterface(this Type type, Type interfaceType)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));
            if (interfaceType == null)
                throw new ArgumentNullException(nameof(interfaceType));

            if (!interfaceType.IsInterface)
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

            if (openType.IsInterface)
            {
                if (!openType.IsOpenGeneric())
                    throw new ArgumentException("The interface type must be an open generic interface: " + openType.Name);

                var interfaceType = type.GetInterface(openType);
                if (interfaceType == null)
                    throw new ArgumentException("The interface type is not implemented by: " + type.Name);

                return interfaceType.GetGenericArguments().Where(x => !x.IsGenericParameter);
            }

            var baseType = type;
            while (baseType != null && baseType != typeof(object))
            {
                if (baseType.IsGenericType && baseType.GetGenericTypeDefinition() == openType)
                    return baseType.GetGenericArguments().Where(x => !x.IsGenericParameter);

                if (!baseType.IsGenericType && baseType == openType)
                    return baseType.GetGenericArguments().Where(x => !x.IsGenericParameter);

                baseType = baseType.BaseType;
            }

            throw new ArgumentException("Could not find open type in type: " + type.Name);
        }
    }
}
