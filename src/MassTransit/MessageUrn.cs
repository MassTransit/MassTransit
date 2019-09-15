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
        static readonly ConcurrentDictionary<Type, MessageUrn> _cache = new ConcurrentDictionary<Type, MessageUrn>();

        public static MessageUrn ForType(Type type)
        {
            return _cache.GetOrAdd(type, _ => new MessageUrn(GetUrnForType(type)));
        }

        MessageUrn(string uriString)
            : base(uriString)
        {
        }

        protected MessageUrn(SerializationInfo serializationInfo, StreamingContext streamingContext)
            : base(serializationInfo, streamingContext)
        {
        }


        public Type GetType(bool throwOnError = true, bool ignoreCase = true)
        {
            if (Segments.Length == 0)
                return null;

            string[] names = Segments[0].Split(':');
            if (names[0] != "message")
                return null;

            string typeName;

            if (names.Length == 2)
                typeName = names[1];
            else if (names.Length == 3)
                typeName = names[1] + "." + names[2] + ", " + names[1];
            else if (names.Length >= 4)
                typeName = names[1] + "." + names[2] + ", " + names[3];
            else
                return null;

            Type messageType = Type.GetType(typeName, true, true);

            return messageType;
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
                string ns = typeInfo.Namespace;
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
                int index = name.IndexOf('`');
                if (index > 0)
                    name = name.Remove(index);
                //

                sb.Append(name);
                sb.Append('[');

                Type[] arguments = typeInfo.GetGenericArguments();
                for (int i = 0; i < arguments.Length; i++)
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
    }
}
