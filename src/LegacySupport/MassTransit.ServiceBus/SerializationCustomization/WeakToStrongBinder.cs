namespace MassTransit.LegacySupport.SerializationCustomization
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.Serialization;

    public class WeakToStrongBinder :
        SerializationBinder
    {
        List<TypeMap> _map = new List<TypeMap>();

        public void AddMap(TypeMap map)
        {
            _map.Add(map);
        }

        public override Type BindToType(string assemblyName, string typeName)
        {
            if(_map.Any(x=>x.WeakTypeName == typeName))
                return _map.Single(x=>x.WeakTypeName == typeName).StrongType;
            
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