namespace MassTransit.LegacySupport.SerializationCustomization
{
    using System;

    public class MessageMap
    {
        public MessageMap(string assemblyName, string fullTypeName, Type newType)
        {
            FullTypeName = fullTypeName;
            AssemblyName = assemblyName;
            NewType = newType;
        }

        public string FullTypeName { get; private set; }
        public string AssemblyName { get; private set; }
        public Type NewType { get; private set; }
    }
}