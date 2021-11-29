namespace MassTransit.AzureCosmos.Saga
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Serialization;


    public class PropertyRenameSerializerContractResolver :
        DefaultContractResolver
    {
        readonly Dictionary<Type, Dictionary<string, string>> _renames;

        public PropertyRenameSerializerContractResolver()
        {
            _renames = new Dictionary<Type, Dictionary<string, string>>();
        }

        public void RenameProperty(Type type, string propertyName, string newJsonPropertyName)
        {
            if (!_renames.ContainsKey(type))
                _renames[type] = new Dictionary<string, string>();

            _renames[type][propertyName] = newJsonPropertyName;
        }

        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            var property = base.CreateProperty(member, memberSerialization);

            if (IsRenamed(property.DeclaringType, property.PropertyName, out var newJsonPropertyName))
                property.PropertyName = newJsonPropertyName;

            return property;
        }

        bool IsRenamed(Type type, string jsonPropertyName, out string newJsonPropertyName)
        {
            if (_renames.TryGetValue(type, out Dictionary<string, string> renames)
                && renames.TryGetValue(jsonPropertyName, out newJsonPropertyName))
                return true;

            newJsonPropertyName = null;
            return false;
        }
    }
}
