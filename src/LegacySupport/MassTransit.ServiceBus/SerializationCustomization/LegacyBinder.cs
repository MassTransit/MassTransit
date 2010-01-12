namespace MassTransit.LegacySupport.SerializationCustomization
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Runtime.Serialization;

    public class LegacyBinder :
        SerializationBinder
    {
        Dictionary<string, Type> _map = new Dictionary<string, Type>();

        public void AddMap(TypeMap map)
        {
            _map.Add(map.WeakTypeName, map.StrongType);
        }

        public override Type BindToType(string assemblyName, string typeName)
        {
            if(_map.ContainsKey(typeName))
                return _map[typeName];

            try
            {
                Assembly ass = Assembly.Load(assemblyName);
                return ass.GetType(typeName);
            }
            catch (Exception ex)
            {   
                throw new LegacySerializationException("Failed serializing {0}, {1}".FormatWith(assemblyName, typeName), ex);
            }
            
        }
    }
}