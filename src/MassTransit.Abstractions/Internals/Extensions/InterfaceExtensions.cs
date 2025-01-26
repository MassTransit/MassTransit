namespace MassTransit.Internals
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
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

            if (!interfaceType.IsInterface)
                throw new ArgumentException("The interface type must be an interface: " + interfaceType.Name);

            if (interfaceType.IsGenericTypeDefinition)
                return _cache.GetGenericInterface(type, interfaceType) != null;

            return interfaceType.IsAssignableFrom(type);
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

            if (!interfaceType.IsInterface)
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

            arguments = [];
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

            if (openType.IsInterface)
            {
                if (!openType.IsOpenGeneric())
                    throw new ArgumentException("The interface type must be an open generic interface: " + openType.Name);

                var interfaceType = type.GetInterface(openType);
                if (interfaceType == null)
                {
                    closedType = default;
                    return false;
                }

                if (interfaceType is { IsGenericTypeDefinition: false, ContainsGenericParameters: false })
                {
                    closedType = interfaceType;
                    return true;
                }

                closedType = default;
                return false;
            }

            var baseType = type;
            while (baseType != null && baseType != typeof(object))
            {
                if (baseType.IsGenericType && baseType.GetGenericTypeDefinition() == openType)
                {
                    if (baseType is { IsGenericTypeDefinition: false, ContainsGenericParameters: false })
                    {
                        closedType = baseType;
                        return true;
                    }

                    closedType = default;
                    return false;
                }

                if (!baseType.IsGenericType && baseType == openType)
                {
                    closedType = baseType;
                    return true;
                }

                baseType = baseType.BaseType;
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
