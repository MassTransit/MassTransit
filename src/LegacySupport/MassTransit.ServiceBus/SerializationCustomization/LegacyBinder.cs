namespace MassTransit.LegacySupport.SerializationCustomization
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.Serialization;

    public class LegacyBinder :
        SerializationBinder
    {
        Dictionary<string, Type> _map = new Dictionary<string, Type>();

        public void AddMap(string oldTypeName, Type newType)
        {
            _map.Add(oldTypeName, newType);
        }

        public override Type BindToType(string assemblyName, string typeName)
        {
            if(_map.ContainsKey(typeName))
                return _map[typeName];

            return Type.GetType("{0}, {1}".FormatWith(assemblyName, typeName));
        }
    }
}