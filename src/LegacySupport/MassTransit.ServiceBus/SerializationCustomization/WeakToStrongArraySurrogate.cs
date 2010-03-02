namespace MassTransit.LegacySupport.SerializationCustomization
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Runtime.Serialization;

    [DebuggerDisplay("Weak:{SurrogateTypeName}")]
    public class WeakToStrongArraySurrogate<T> :
        LegacySurrogate
    {
        public WeakToStrongArraySurrogate(string weakAssemblyName, string weakTypeName)
        {
            WeakAssemblyName = weakAssemblyName;
            WeakTypeName = weakTypeName;
            StrongType = typeof (T);
        }

        public string WeakAssemblyName { get; set; }
        public string WeakTypeName { get; set; }
        public Type StrongType { get; set; }

        //from weak to strong
        public void GetObjectData(object obj, SerializationInfo info, StreamingContext context)
        {
            throw new NotImplementedException();
        }

        //strong to weak
        public object SetObjectData(object obj, SerializationInfo info, StreamingContext context, ISurrogateSelector selector)
        {
            if(!obj.GetType().IsArray) throw new LegacySerializationException("not an array");

            var data = (Array) obj;
            var instance = new List<T>();

            foreach (var item in data)
            {
                //convert : how do I access the binder here?
                instance.Add((T)item);
            }
            return instance.ToArray();
        }

        public string SurrogateTypeName
        {
            get { return WeakTypeName; }
        }
    }
}