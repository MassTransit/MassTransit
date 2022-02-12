namespace MassTransit.Internals
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.CompilerServices;


    public static class TypeExtensions
    {
        static readonly TypeNameFormatter _typeNameFormatter = new TypeNameFormatter();

        /// <summary>
        /// Returns an easy-to-read type name from the specified Type
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static string GetTypeName(this Type type)
        {
            return _typeNameFormatter.GetTypeName(type);
        }

        public static IEnumerable<PropertyInfo> GetAllProperties(this Type type)
        {
            var typeInfo = type.GetTypeInfo();

            return GetAllProperties(typeInfo);
        }

        public static IEnumerable<PropertyInfo> GetAllProperties(this TypeInfo typeInfo)
        {
            if (typeInfo.BaseType != null)
            {
                foreach (var prop in GetAllProperties(typeInfo.BaseType))
                    yield return prop;
            }

            List<PropertyInfo> properties = typeInfo.DeclaredMethods
                .Where(x => x.IsSpecialName && x.Name.StartsWith("get_") && !x.IsStatic)
                .Select(x => typeInfo.GetDeclaredProperty(x.Name.Substring("get_".Length)))
                .ToList();

            if (typeInfo.IsInterface)
            {
                IEnumerable<PropertyInfo> sourceProperties = properties
                    .Concat(typeInfo.ImplementedInterfaces.SelectMany(x => x.GetTypeInfo().DeclaredProperties));

                foreach (var prop in sourceProperties)
                    yield return prop;

                yield break;
            }

            foreach (var info in properties)
                yield return info;
        }

        public static IEnumerable<Type> GetAllInterfaces(this Type type)
        {
            var typeInfo = type.GetTypeInfo();

            return GetAllInterfaces(typeInfo);
        }

        public static IEnumerable<Type> GetAllInterfaces(this TypeInfo typeInfo)
        {
            if (typeInfo.IsInterface)
                yield return typeInfo;

            foreach (var interfaceType in typeInfo.GetInterfaces())
                yield return interfaceType;
        }

        public static IEnumerable<PropertyInfo> GetAllStaticProperties(this Type type)
        {
            var info = type.GetTypeInfo();

            if (info.BaseType != null)
            {
                foreach (var prop in GetAllStaticProperties(info.BaseType))
                    yield return prop;
            }

            IEnumerable<PropertyInfo> props = info.DeclaredMethods
                .Where(x => x.IsSpecialName && x.Name.StartsWith("get_") && x.IsStatic)
                .Select(x => info.GetDeclaredProperty(x.Name.Substring("get_".Length)));

            foreach (var propertyInfo in props)
                yield return propertyInfo;
        }

        public static IEnumerable<PropertyInfo> GetStaticProperties(this Type type)
        {
            var info = type.GetTypeInfo();

            return info.DeclaredMethods
                .Where(x => x.IsSpecialName && x.Name.StartsWith("get_") && x.IsStatic)
                .Select(x => info.GetDeclaredProperty(x.Name.Substring("get_".Length)));
        }

        /// <summary>
        /// Determines if a type is neither abstract nor an interface and can be constructed.
        /// </summary>
        /// <param name="type">The type to check</param>
        /// <returns>True if the type can be constructed, otherwise false.</returns>
        public static bool IsConcrete(this Type type)
        {
            var typeInfo = type.GetTypeInfo();
            return !typeInfo.IsAbstract && !typeInfo.IsInterface;
        }

        public static bool IsInterfaceOrConcreteClass(this Type type)
        {
            var typeInfo = type.GetTypeInfo();
            if (typeInfo.IsInterface)
                return true;

            return typeInfo.IsClass && !typeInfo.IsAbstract;
        }

        /// <summary>
        /// Determines if a type can be constructed, and if it can, additionally determines
        /// if the type can be assigned to the specified type.
        /// </summary>
        /// <param name="type">The type to evaluate</param>
        /// <param name="assignableType">The type to which the subject type should be checked against</param>
        /// <returns>
        /// True if the type is concrete and can be assigned to the assignableType, otherwise false.
        /// </returns>
        public static bool IsConcreteAndAssignableTo(this Type type, Type assignableType)
        {
            return IsConcrete(type) && assignableType.GetTypeInfo().IsAssignableFrom(type.GetTypeInfo());
        }

        /// <summary>
        /// Determines if a type can be constructed, and if it can, additionally determines
        /// if the type can be assigned to the specified type.
        /// </summary>
        /// <param name="type">The type to evaluate</param>
        /// <typeparam name="T">The type to which the subject type should be checked against</typeparam>
        /// <returns>
        /// True if the type is concrete and can be assigned to the assignableType, otherwise false.
        /// </returns>
        public static bool IsConcreteAndAssignableTo<T>(this Type type)
        {
            return IsConcrete(type) && typeof(T).GetTypeInfo().IsAssignableFrom(type.GetTypeInfo());
        }

        /// <summary>
        /// Determines if the type is a nullable type
        /// </summary>
        /// <param name="type">The type</param>
        /// <param name="underlyingType">The underlying type of the nullable</param>
        /// <returns>True if the type can be null</returns>
        public static bool IsNullable(this Type type, out Type? underlyingType)
        {
            var typeInfo = type.GetTypeInfo();
            var isNullable = typeInfo.IsGenericType && typeInfo.GetGenericTypeDefinition() == typeof(Nullable<>);

            underlyingType = isNullable ? Nullable.GetUnderlyingType(type) : null;
            return isNullable;
        }

        /// <summary>
        /// Determines if the type is an open generic with at least one unspecified generic argument
        /// </summary>
        /// <param name="type">The type</param>
        /// <returns>True if the type is an open generic</returns>
        public static bool IsOpenGeneric(this Type type)
        {
            return type.GetTypeInfo().IsOpenGeneric();
        }

        /// <summary>
        /// Determines if the TypeInfo is an open generic with at least one unspecified generic argument
        /// </summary>
        /// <param name="typeInfo">The TypeInfo</param>
        /// <returns>True if the TypeInfo is an open generic</returns>
        public static bool IsOpenGeneric(this TypeInfo typeInfo)
        {
            return typeInfo.IsGenericTypeDefinition || typeInfo.ContainsGenericParameters;
        }

        /// <summary>
        /// Determines if a type can be null
        /// </summary>
        /// <param name="type">The type</param>
        /// <returns>True if the type can be null</returns>
        public static bool CanBeNull(this Type type)
        {
            var typeInfo = type.GetTypeInfo();

            return !typeInfo.IsValueType
                || type == typeof(string)
                || typeInfo.IsGenericType && typeInfo.GetGenericTypeDefinition() == typeof(Nullable<>);
        }

        /// <summary>
        /// Returns the first attribute of the specified type for the object specified
        /// </summary>
        /// <typeparam name="T">The type of attribute</typeparam>
        /// <param name="provider">An attribute provider, which can be a MethodInfo, PropertyInfo, Type, etc.</param>
        /// <returns>The attribute instance if found, or null</returns>
        public static IEnumerable<T> GetAttribute<T>(this ICustomAttributeProvider provider)
            where T : Attribute
        {
            return provider.GetCustomAttributes(typeof(T), true)
                .Cast<T>();
        }

        /// <summary>
        /// Determines if the target has the specified attribute
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="provider"></param>
        /// <returns></returns>
        public static bool HasAttribute<T>(this ICustomAttributeProvider provider)
            where T : Attribute
        {
            return provider.GetAttribute<T>().Any();
        }

        /// <summary>
        /// Returns true if the type is an anonymous type
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool IsAnonymousType(this Type type)
        {
            return type.GetTypeInfo().IsAnonymousType();
        }

        /// <summary>
        /// Returns true if the TypeInfo is an anonymous type
        /// </summary>
        /// <param name="typeInfo"></param>
        /// <returns></returns>
        public static bool IsAnonymousType(this TypeInfo typeInfo)
        {
            return typeInfo.HasAttribute<CompilerGeneratedAttribute>() && typeInfo.FullName.Contains("AnonymousType");
        }

        /// <summary>
        /// Returns true if the type is an FSharp type (maybe?)
        /// </summary>
        /// <param name="typeInfo"></param>
        /// <returns></returns>
        public static bool IsFSharpType(this TypeInfo typeInfo)
        {
            IEnumerable<Attribute> attributes = typeInfo.GetCustomAttributes();

            return attributes.Any(attribute => attribute.GetType().FullName == "Microsoft.FSharp.Core.CompilationMappingAttribute");
        }

        /// <summary>
        /// Returns true if the type is contained within the namespace
        /// </summary>
        /// <param name="type"></param>
        /// <param name="nameSpace"></param>
        /// <returns></returns>
        public static bool IsInNamespace(this Type type, string nameSpace)
        {
            var subNameSpace = nameSpace + ".";
            return type.Namespace != null && (type.Namespace.Equals(nameSpace) || type.Namespace.StartsWith(subNameSpace));
        }

        /// <summary>
        /// True if the type is a value type, or an object type that is treated as a value by MassTransit
        /// </summary>
        /// <param name="valueType"></param>
        /// <returns></returns>
        public static bool IsValueTypeOrObject(this Type valueType)
        {
            if (valueType.GetTypeInfo().IsValueType
                || valueType == typeof(string)
                || valueType == typeof(Uri)
                || valueType == typeof(Version)
                || typeof(Exception).IsAssignableFrom(valueType))
                return true;

            return false;
        }
    }
}
