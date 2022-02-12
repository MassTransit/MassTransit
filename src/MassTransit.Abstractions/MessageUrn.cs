namespace MassTransit
{
    using System;
    using System.Collections.Concurrent;
    using System.Reflection;
    using System.Runtime.Serialization;
    using System.Text;


    [Serializable]
    public class MessageUrn :
        Uri
    {
        static readonly ConcurrentDictionary<Type, Cached> _cache = new ConcurrentDictionary<Type, Cached>();

        MessageUrn(string uriString)
            : base(uriString)
        {
        }

        protected MessageUrn(SerializationInfo serializationInfo, StreamingContext streamingContext)
            : base(serializationInfo, streamingContext)
        {
        }

        public static MessageUrn ForType<T>()
        {
            return MessageUrnCache<T>.Urn;
        }

        public static string ForTypeString<T>()
        {
            return MessageUrnCache<T>.UrnString;
        }

        public static MessageUrn ForType(Type type)
        {
            if (type.ContainsGenericParameters)
                throw new ArgumentException("A message type may not contain generic parameters", nameof(type));

            return _cache.GetOrAdd(type, _ => (Cached)Activator.CreateInstance(typeof(Cached<>).MakeGenericType(type))).Urn;
        }

        public static string ForTypeString(Type type)
        {
            return _cache.GetOrAdd(type, _ => (Cached)Activator.CreateInstance(typeof(Cached<>).MakeGenericType(type))).UrnString;
        }

        public void Deconstruct(out string? name, out string? ns, out string? assemblyName)
        {
            name = null;
            ns = null;
            assemblyName = null;

            if (Segments.Length > 0)
            {
                string[] names = Segments[0].Split(':');
                if (string.Compare(names[0], "message", StringComparison.OrdinalIgnoreCase) == 0)
                {
                    if (names.Length == 2)
                        name = names[1];
                    else if (names.Length == 3)
                    {
                        name = names[2];
                        ns = names[1];
                    }
                    else if (names.Length >= 4)
                    {
                        name = names[2];
                        ns = names[1];
                        assemblyName = names[3];
                    }
                }
            }
        }

        static string GetUrnForType(Type type)
        {
            var sb = new StringBuilder("urn:message:");

            return GetMessageName(sb, type, true);
        }

        static string GetMessageName(StringBuilder sb, Type type, bool includeScope)
        {
            var typeInfo = type.GetTypeInfo();
            if (typeInfo.IsGenericParameter)
                return "";

            if (includeScope && typeInfo.Namespace != null)
            {
                var ns = typeInfo.Namespace;
                sb.Append(ns);

                sb.Append(':');
            }

            if (typeInfo.IsNested)
            {
                GetMessageName(sb, typeInfo.DeclaringType, false);
                sb.Append('+');
            }

            if (typeInfo.IsGenericType)
            {
                var name = typeInfo.GetGenericTypeDefinition().Name;

                //remove `1
                var index = name.IndexOf('`');
                if (index > 0)
                    name = name.Remove(index);
                //

                sb.Append(name);
                sb.Append('[');

                Type[] arguments = typeInfo.GetGenericArguments();
                for (var i = 0; i < arguments.Length; i++)
                {
                    if (i > 0)
                        sb.Append(',');

                    sb.Append('[');
                    GetMessageName(sb, arguments[i], true);
                    sb.Append(']');
                }

                sb.Append(']');
            }
            else
                sb.Append(typeInfo.Name);

            return sb.ToString();
        }


        static class MessageUrnCache<T>
        {
            internal static readonly MessageUrn Urn;
            internal static readonly string UrnString;

            static MessageUrnCache()
            {
                Urn = new MessageUrn(GetUrnForType(typeof(T)));
                UrnString = Urn.ToString();
            }
        }


        interface Cached
        {
            MessageUrn Urn { get; }
            string UrnString { get; }
        }


        class Cached<T> :
            Cached
        {
            public MessageUrn Urn => MessageUrnCache<T>.Urn;
            public string UrnString => MessageUrnCache<T>.UrnString;
        }
    }
}
