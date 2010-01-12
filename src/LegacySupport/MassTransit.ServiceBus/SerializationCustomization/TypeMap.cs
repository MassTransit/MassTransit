namespace MassTransit.LegacySupport.SerializationCustomization
{
    using System;

    public class TypeMap
    {
        public TypeMap(string weakAssemblyName, string weakTypeName, Type strongType)
        {
            WeakTypeName = weakTypeName;
            WeakAssemblyName = weakAssemblyName;
            StrongType = strongType;
        }

        public string WeakTypeName { get; private set; }
        public string WeakAssemblyName { get; private set; }
        public Type StrongType { get; private set; }
    }
}