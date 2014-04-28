// Copyright 2007-2014 Chris Patterson, Dru Sellers, Travis Smith, et. al.
//
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use
// this file except in compliance with the License. You may obtain a copy of the 
// License at 
//
//    http://www.apache.org/licenses/LICENSE-2.0 
//
// Unless required by applicable law or agreed to in writing, software distributed
// under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR 
// CONDITIONS OF ANY KIND, either express or implied. See the License for the 
// specific language governing permissions and limitations under the License.
namespace MassTransit
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.Serialization;
    using System.Text;

    [Serializable]
    public class MessageUrn :
        Uri
    {
        [ThreadStatic]
        static IDictionary<Type, string> _cache;

        public MessageUrn(Type type)
            : base(GetUrnForType(type))
        {
        }

        public MessageUrn(string uriString)
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

            var typeName = UrnMessageTypeParser.Parse(Segments[0]);
            Type messageType = Type.GetType(typeName, throwOnError, ignoreCase);

            return messageType;
        }

        static string IsInCache(Type type, Func<Type, string> provider)
        {
            if (_cache == null)
                _cache = new Dictionary<Type, string>();

            string urn;
            if (_cache.TryGetValue(type, out urn))
                return urn;

            urn = provider(type);

            _cache[type] = urn;

            return urn;
        }

        static string GetUrnForType(Type type)
        {
            return IsInCache(type, x =>
                {
                    var sb = new StringBuilder("urn:message:");

                    return GetMessageName(sb, type, true);
                });
        }

        static string GetMessageName(StringBuilder sb, Type type, bool includeScope)
        {
            if (type.IsGenericParameter)
                return "";

            if (includeScope && type.Namespace != null)
            {
                string ns = type.Namespace;
                sb.Append(ns);

                sb.Append(':');
            }

            if (type.IsNested)
            {
                GetMessageName(sb, type.DeclaringType, false);
                sb.Append('+');
            }

            if (type.IsGenericType)
            {
                var name = type.GetGenericTypeDefinition().Name;

                //remove `1
                int index = name.IndexOf('`');
                if (index > 0)
                    name = name.Remove(index);
                //

                sb.Append(name);
                sb.Append('[');

                Type[] arguments = type.GetGenericArguments();
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
                sb.Append(type.Name);

            if (includeScope)
            {
                sb.Append(':');
                sb.Append(type.Assembly.GetName().Name);
            }

            return sb.ToString();
        }

        private static class UrnMessageTypeParser
        {
            private class TypeNameNode
            {
                public TypeNameNode Parent;
                public List<TypeNameNode> Children = new List<TypeNameNode>();
                public string Content { get; set; }

                private string GenericParameters
                {
                    get { return Children.Count > 0 ? "`" + Children.Count : string.Empty; }
                }

                private string GenericArguments
                {
                    get
                    {
                        // no generic type arguments
                        if (Children.Count == 0)
                            return string.Empty;

                        // if any of the type arguments are empty, treat current
                        // type as an open generic - without brackets, just the backtick
                        var childrenNames = Children.Select(c => c.ToString()).ToArray();
                        return childrenNames.Any(c => c.Length == 0)
                            ? string.Empty
                            : "[" + string.Join(",", childrenNames) + "]";
                    }
                }

                private string FormatAsNetTypeName()
                {
                    // I'm just an open generic argument, no antual content!
                    if (string.IsNullOrEmpty(Content))
                        return string.Empty;

                    var nameParts = Content.Split(':');
                    string netTypeName = string.Empty;

                    if (nameParts.Length == 1)
                    {
                        // ClassName -> ClassName`Tn[T1, T2, ..., Tn]
                        netTypeName = nameParts[0] + GenericParameters + GenericArguments;
                    }
                    else if (nameParts.Length == 2)
                    {
                        // Namespace:ClassName -> Namespace.ClassName`Tn[T1, T2, ..., Tn], Namespace
                        netTypeName = nameParts[0] + "." + nameParts[1] + GenericParameters + GenericArguments + ", " + nameParts[0];
                    }
                    else if (nameParts.Length >= 3)
                    {
                        // Namespace:ClassName:AssemblyName -> Namespace.ClassName`Tn[T1, T2, ..., Tn], AssemblyName
                        netTypeName = nameParts[0] + "." + nameParts[1] + GenericParameters + GenericArguments + ", " + nameParts[2];
                    }

                    return netTypeName;
                }

                /// <summary>
                /// Makes sure there is no nested empty parentheses.
                /// </summary>
                public void Flatten()
                {
                    if (string.IsNullOrEmpty(Content) && Parent != null)
                        Parent.Children = Children;

                    foreach (var child in Children)
                        child.Flatten();
                }

                /// <summary>
                /// Returns the node formatted as a .Net type name, including its children.
                /// </summary>
                public override string ToString()
                {
                    var formattedName = FormatAsNetTypeName() ?? string.Empty;

                    // for the root (outer message type) return name without brackets;
                    // if type name is empty (I'm an open generic argument) do so too
                    if (Parent == null || string.IsNullOrEmpty(formattedName))
                        return formattedName;
                    else
                        return "[" + formattedName + "]";
                }
            }

            /// <summary>
            /// Transforms a type name specified in a given URN to a .Net-compatible type name.
            /// </summary>
            public static string Parse(string absolutePath)
            {
                const string preamble = "message:";
                if (absolutePath.StartsWith(preamble))
                    absolutePath = absolutePath.Substring(preamble.Length);

                TypeNameNode root = new TypeNameNode();
                var current = root;

                foreach (char c in absolutePath)
                {
                    switch (c)
                    {
                        case '[': // go one level down
                            var child = new TypeNameNode { Parent = current };
                            current.Children.Add(child);
                            current = child;
                            break;
                        case ']': // go one level up
                            current = current.Parent;
                            break;
                        case ',': // sibling will be caugth by '['
                        case ' ': // ignore whitespace
                            break;
                        default:
                            current.Content += c;
                            break;
                    }
                }

                root.Flatten();
                return root.ToString();
            }
        }
    }
}