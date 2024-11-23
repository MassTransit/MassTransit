namespace MassTransit.Metadata
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Internals;


    public class ImplementedMessageTypeCache<TMessage> :
        IImplementedMessageTypeCache<TMessage>
        where TMessage : class
    {
        readonly CachedType[] _implementedTypes;

        ImplementedMessageTypeCache()
        {
            _implementedTypes = GetMessageTypes()
                .Select(x => Activation.Activate(x.Type, new Factory(), x.Direct))
                .ToArray();
        }

        void IImplementedMessageTypeCache<TMessage>.EnumerateImplementedTypes(IImplementedMessageType implementedMessageType)
        {
            for (var i = 0; i < _implementedTypes.Length; i++)
            {
                if (_implementedTypes[i].MessageType == typeof(TMessage))
                    continue;

                _implementedTypes[i].ImplementsType(implementedMessageType);
            }
        }

        public void Method1()
        {
        }

        public void Method2()
        {
        }

        public void Method3()
        {
        }

        /// <summary>
        /// Enumerate the implemented message types
        /// </summary>
        /// <param name="implementedMessageType">The interface reference to invoke for each type</param>
        public static void EnumerateImplementedTypes(IImplementedMessageType implementedMessageType)
        {
            Cached.Instance.Value.EnumerateImplementedTypes(implementedMessageType);
        }

        static IEnumerable<ImplementedType> GetMessageTypes()
        {
            return GetMessageTypes(new HashSet<Type>(), typeof(TMessage), true);
        }

        static IEnumerable<ImplementedType> GetMessageTypes(HashSet<Type> used, Type messageType, bool direct)
        {
            if (messageType.ClosesType(typeof(Fault<>), out Type[] arguments))
            {
                foreach (var faultMessageType in GetMessageTypes(used, arguments[0], direct))
                {
                    var faultInterfaceType = typeof(Fault<>).MakeGenericType(faultMessageType.Type);
                    if (faultInterfaceType != typeof(TMessage) && used.Add(faultInterfaceType))
                        yield return new ImplementedType(faultInterfaceType, faultMessageType.Direct);
                }
            }

            var baseType = messageType.BaseType;
            if (baseType != null && baseType != typeof(object) && MessageTypeCache.IsValidMessageType(baseType))
            {
                if (used.Add(baseType))
                    yield return new ImplementedType(baseType, direct);

                HashSet<Type> baseUsed = [..used];
                List<Type> implementedTypes = [];

                foreach (var baseMessageType in GetMessageTypes(baseUsed, baseType, false))
                {
                    var type = baseMessageType.Type;

                    if (baseUsed.Add(type))
                        implementedTypes.Add(type);
                }

                foreach (var implementedType in implementedTypes)
                {
                    if (used.Add(implementedType))
                        yield return new ImplementedType(implementedType, direct);
                }
            }

            Type[]? interfaces = messageType.GetInterfaces();

            for (var index = 0; index < interfaces.Length; index++)
            {
                var interfaceType = interfaces[index];

                if (MessageTypeCache.IsValidMessageType(interfaceType))
                {
                    foreach (var baseInterfaceType in GetMessageTypes(used, interfaceType, false))
                    {
                        if (used.Add(baseInterfaceType.Type))
                            yield return new ImplementedType(baseInterfaceType.Type, direct);
                    }

                    if (used.Add(interfaceType))
                        yield return new ImplementedType(interfaceType, direct);
                }
            }
        }


        readonly struct Factory :
            IActivationType<CachedType, bool>
        {
            public CachedType ActivateType<T>(bool direct)
                where T : class
            {
                return new TypeAdapter<T>(direct);
            }
        }


        struct ImplementedType
        {
            /// <summary>
            /// The implemented type
            /// </summary>
            public readonly Type Type;

            /// <summary>
            /// True if the interface is directly implemented by the type
            /// </summary>
            public readonly bool Direct;

            public ImplementedType(Type type, bool direct)
            {
                Type = type;
                Direct = direct;
            }
        }


        interface CachedType
        {
            Type MessageType { get; }
            bool Direct { get; }
            void ImplementsType(IImplementedMessageType implementedMessageType);
        }


        static class Cached
        {
            internal static readonly Lazy<IImplementedMessageTypeCache<TMessage>> Instance = new(() => new ImplementedMessageTypeCache<TMessage>());
        }


        class TypeAdapter<TAdapter> :
            CachedType
            where TAdapter : class
        {
            public TypeAdapter(bool direct)
            {
                Direct = direct;
            }

            public bool Direct { get; }
            public Type MessageType => typeof(TAdapter);

            public void ImplementsType(IImplementedMessageType implementedMessageType)
            {
                implementedMessageType.ImplementsMessageType<TAdapter>(Direct);
            }

            public void Method1()
            {
            }

            public void Method2()
            {
            }

            public void Method3()
            {
            }
        }
    }
}
