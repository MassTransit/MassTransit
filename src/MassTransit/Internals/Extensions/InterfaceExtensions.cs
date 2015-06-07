// Copyright 2007-2015 Chris Patterson, Dru Sellers, Travis Smith, et. al.
//  
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use
// this file except in compliance with the License. You may obtain a copy of the 
// License at 
// 
//     http://www.apache.org/licenses/LICENSE-2.0 
// 
// Unless required by applicable law or agreed to in writing, software distributed
// under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR 
// CONDITIONS OF ANY KIND, either express or implied. See the License for the 
// specific language governing permissions and limitations under the License.
namespace MassTransit.Internals.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using Reflection;


    public static class InterfaceExtensions
    {
        static readonly InterfaceReflectionCache _cache;

        static InterfaceExtensions()
        {
            _cache = new InterfaceReflectionCache();
        }

        public static bool HasInterface<T>(this object obj)
        {
            if (obj == null)
                throw new ArgumentNullException("obj");
            Type type = obj.GetType();

            return HasInterface(type, typeof(T));
        }

        public static bool HasInterface<T>(this Type type)
        {
            return HasInterface(type, typeof(T));
        }

        public static bool HasInterface(this Type type, Type interfaceType)
        {
            if (type == null)
                throw new ArgumentNullException("type");
            if (interfaceType == null)
                throw new ArgumentNullException("interfaceType");

            TypeInfo interfaceTypeInfo = interfaceType.GetTypeInfo();
            if (!interfaceTypeInfo.IsInterface)
                throw new ArgumentException("The interface type must be an interface: " + interfaceType.Name);

            if (interfaceTypeInfo.IsGenericTypeDefinition)
                return _cache.GetGenericInterface(type, interfaceType) != null;

            return interfaceTypeInfo.IsAssignableFrom(type.GetTypeInfo());
        }

        public static bool HasInterface(this object obj, Type interfaceType)
        {
            if (obj == null)
                throw new ArgumentNullException("obj");
            Type type = obj.GetType();

            return HasInterface(type, interfaceType);
        }

        public static Type GetInterface<T>(this object obj)
        {
            if (obj == null)
                throw new ArgumentNullException("obj");
            Type type = obj.GetType();

            return GetInterface(type, typeof(T));
        }

        public static Type GetInterface<T>(this Type type)
        {
            return GetInterface(type, typeof(T));
        }

        public static Type GetInterface(this Type type, Type interfaceType)
        {
            if (type == null)
                throw new ArgumentNullException("type");
            if (interfaceType == null)
                throw new ArgumentNullException("interfaceType");

            TypeInfo interfaceTypeInfo = interfaceType.GetTypeInfo();
            if (!interfaceTypeInfo.IsInterface)
                throw new ArgumentException("The interface type must be an interface: " + interfaceType.Name);

            return _cache.Get(type, interfaceType);
        }

        public static bool ClosesType(this Type type, Type openType)
        {
            if (type == null)
                throw new ArgumentNullException("type");
            if (openType == null)
                throw new ArgumentNullException("openType");

            if (!openType.IsOpenGeneric())
                throw new ArgumentException("The interface type must be an open generic interface: " + openType.Name);

            if (openType.GetTypeInfo().IsInterface)
            {
                if (!openType.IsOpenGeneric())
                    throw new ArgumentException("The interface type must be an open generic interface: " + openType.Name);

                Type interfaceType = type.GetInterface(openType);
                if (interfaceType == null)
                    return false;

                TypeInfo typeInfo = interfaceType.GetTypeInfo();
                return !typeInfo.IsGenericTypeDefinition && !typeInfo.ContainsGenericParameters;
            }

            Type baseType = type;
            while (baseType != null && baseType != typeof(object))
            {
                TypeInfo baseTypeInfo = baseType.GetTypeInfo();
                if (baseTypeInfo.IsGenericType && baseTypeInfo.GetGenericTypeDefinition() == openType)
                    return !baseTypeInfo.IsGenericTypeDefinition && !baseTypeInfo.ContainsGenericParameters;

                if (!baseTypeInfo.IsGenericType && baseType == openType)
                    return true;

                baseType = baseTypeInfo.BaseType;
            }

            return false;
        }

        public static IEnumerable<Type> GetClosingArguments(this Type type, Type openType)
        {
            if (type == null)
                throw new ArgumentNullException("type");
            if (openType == null)
                throw new ArgumentNullException("openType");

            if (!openType.IsOpenGeneric())
                throw new ArgumentException("The interface type must be an open generic interface: " + openType.Name);

            if (openType.GetTypeInfo().IsInterface)
            {
                if (!openType.IsOpenGeneric())
                    throw new ArgumentException("The interface type must be an open generic interface: " + openType.Name);

                Type interfaceType = type.GetInterface(openType);
                if (interfaceType == null)
                    throw new ArgumentException("The interface type is not implemented by: " + type.Name);

                return interfaceType.GetTypeInfo().GetGenericArguments().Where(x => !x.IsGenericParameter);
            }

            Type baseType = type;
            while (baseType != null && baseType != typeof(object))
            {
                TypeInfo baseTypeInfo = baseType.GetTypeInfo();
                if (baseTypeInfo.IsGenericType && baseType.GetGenericTypeDefinition() == openType)
                    return baseTypeInfo.GetGenericArguments().Where(x => !x.IsGenericParameter);

                if (!baseTypeInfo.IsGenericType && baseType == openType)
                    return baseTypeInfo.GetGenericArguments().Where(x => !x.IsGenericParameter);

                baseType = baseTypeInfo.BaseType;
            }

            throw new ArgumentException("Could not find open type in type: " + type.Name);
        }

        public static IEnumerable<Type> GetClosingArguments(this object obj, Type openType)
        {
            if (obj == null)
                throw new ArgumentNullException("obj");

            Type objectType = obj.GetType();

            return GetClosingArguments(objectType, openType);
        }
    }
}