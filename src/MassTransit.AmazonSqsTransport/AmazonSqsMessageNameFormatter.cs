namespace MassTransit.AmazonSqsTransport
{
    using System;
    using System.Collections.Concurrent;
    using System.Reflection;
    using System.Text;
    using Transports;


    public class AmazonSqsMessageNameFormatter :
        IMessageNameFormatter
    {
        readonly ConcurrentDictionary<Type, string> _cache;
        readonly string _genericArgumentSeparator;
        readonly string _genericTypeSeparator;
        readonly string _namespaceSeparator;
        readonly string _nestedTypeSeparator;

        public AmazonSqsMessageNameFormatter(string genericArgumentSeparator = null, string genericTypeSeparator = null,
            string namespaceSeparator = null, string nestedTypeSeparator = null)
        {
            _genericArgumentSeparator = genericArgumentSeparator ?? "__";
            _genericTypeSeparator = genericTypeSeparator ?? "--";
            _namespaceSeparator = namespaceSeparator ?? "-";
            _nestedTypeSeparator = nestedTypeSeparator ?? "_";

            _cache = new ConcurrentDictionary<Type, string>();
        }

        public MessageName GetMessageName(Type type)
        {
            return new MessageName(_cache.GetOrAdd(type, CreateMessageName));
        }

        string CreateMessageName(Type type)
        {
            if (type.GetTypeInfo().IsGenericTypeDefinition)
                throw new ArgumentException("An open generic type cannot be used as a message name");

            var sb = new StringBuilder("");

            return GetMessageName(sb, type, null);
        }

        string GetMessageName(StringBuilder sb, Type type, string scope)
        {
            var typeInfo = type.GetTypeInfo();
            if (typeInfo.IsGenericParameter)
                return "";

            if (typeInfo.Namespace != null)
            {
                string ns = typeInfo.Namespace.Replace(".", _nestedTypeSeparator);
                if (!ns.Equals(scope))
                {
                    sb.Append(ns);
                    sb.Append(_namespaceSeparator);
                }
            }

            if (typeInfo.IsNested)
            {
                GetMessageName(sb, typeInfo.DeclaringType, typeInfo.Namespace);
                sb.Append(_nestedTypeSeparator);
            }

            if (typeInfo.IsGenericType)
            {
                string name = typeInfo.GetGenericTypeDefinition().Name;

                //remove `1
                int index = name.IndexOf('`');
                if (index > 0)
                    name = name.Remove(index);

                sb.Append(name);
                sb.Append(_genericTypeSeparator);

                Type[] arguments = typeInfo.GetGenericArguments();
                for (int i = 0; i < arguments.Length; i++)
                {
                    if (i > 0)
                        sb.Append(_genericArgumentSeparator);

                    GetMessageName(sb, arguments[i], typeInfo.Namespace);
                }

                sb.Append(_genericTypeSeparator);
            }
            else
                sb.Append(typeInfo.Name);

            return sb.ToString();
        }
    }
}
