namespace MassTransit.LegacySupport.SerializationCustomization
{
    using System;
    using System.Runtime.Serialization;

    public class NewToOldSerializationSurrogate :
        ISerializationSurrogate
    {
        public void GetObjectData(object obj, SerializationInfo info, StreamingContext context)
        {
            //cast obj to new
            info.AssemblyName = "Demo.TypeForwarding, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null";
            info.FullTypeName = "Demo.TypeForwarding.Old.Message";
            info.AddValue("Prop Name", "value");
        }

        public object SetObjectData(object obj, SerializationInfo info, StreamingContext context, ISurrogateSelector selector)
        {
            throw new NotImplementedException("shouldn't be needed");
        }
    }
}