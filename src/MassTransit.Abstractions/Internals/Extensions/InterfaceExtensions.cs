namespace MassTransit.Internals
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Threading.Tasks;


    public static class InterfaceExtensions
    {
        static readonly InterfaceReflectionCache _cache;

        static InterfaceExtensions()
        {
            _cache = new InterfaceReflectionCache();
        }

        public static bool HasInterface<T>(this Type type)
        {
            return HasInterface(type, typeof(T));
        }

        public static bool HasInterface(this Type type, Type interfaceType)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));

            if (interfaceType == null)
                throw new ArgumentNullException(nameof(interfaceType));

            var interfaceTypeInfo = interfaceType.GetTypeInfo();
            if (!interfaceTypeInfo.IsInterface)
                throw new ArgumentException("The interface type must be an interface: " + interfaceType.Name);

            if (interfaceTypeInfo.IsGenericTypeDefinition)
                return _cache.GetGenericInterface(type, interfaceType) != null;

            return interfaceTypeInfo.IsAssignableFrom(type.GetTypeInfo());
        }

        public static Type? GetInterface<T>(this Type type)
        {
            return GetInterface(type, typeof(T));
        }

        public static Type? GetInterface(this Type type, Type interfaceType)
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

        public static bool IsTask(this Type type, out Type? taskType)
        {
            if (ClosesType(type, typeof(Task<>), out Type? closedType))
            {
                Type[] arguments = closedType!.GetGenericArguments();
                for (var i = 0; i < arguments.Length; i++)
                {
                    if (arguments[i].IsGenericParameter)
                        continue;

                    taskType = arguments[i];
                    return true;
                }
            }

            taskType = default;
            return false;
        }

        public static bool ClosesType(this Type type, Type openType)
        {
            return ClosesType(type, openType, out Type? _);
        }

        public static bool ClosesType(this Type type, Type openType, out Type[] arguments)
        {
            if (ClosesType(type, openType, out Type? closedType))
            {
                arguments = closedType!.GetGenericArguments().Where(x => !x.IsGenericParameter).ToArray();
                return true;
            }

            arguments = Array.Empty<Type>();
            return false;
        }

        public static bool ClosesType(this Type type, Type openType, out Type? closedType)
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
                {
                    closedType = default;
                    return false;
                }

                var typeInfo = interfaceType.GetTypeInfo();
                if (!typeInfo.IsGenericTypeDefinition && !typeInfo.ContainsGenericParameters)
                {
                    closedType = typeInfo;
                    return true;
                }

                closedType = default;
                return false;
            }

            var baseType = type;
            while (baseType != null && baseType != typeof(object))
            {
                var baseTypeInfo = baseType.GetTypeInfo();
                if (baseTypeInfo.IsGenericType && baseTypeInfo.GetGenericTypeDefinition() == openType)
                {
                    if (!baseTypeInfo.IsGenericTypeDefinition && !baseTypeInfo.ContainsGenericParameters)
                    {
                        closedType = baseTypeInfo;
                        return true;
                    }

                    closedType = default;
                    return false;
                }

                if (!baseTypeInfo.IsGenericType && baseType == openType)
                {
                    closedType = baseTypeInfo;
                    return true;
                }

                baseType = baseTypeInfo.BaseType;
            }

            closedType = default;
            return false;
        }

        public static Type GetClosingArgument(this Type type, Type openType)
        {
            return GetClosingArguments(type, openType).Single();
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
