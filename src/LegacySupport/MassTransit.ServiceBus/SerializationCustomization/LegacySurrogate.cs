namespace MassTransit.LegacySupport.SerializationCustomization
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using System.Runtime.Serialization;
    using Magnum.Reflection;

    public interface LegacySurrogate :
        ISerializationSurrogate
    {
        Type SurrogateType { get; }
    }
    public class LegacySurrogate<T> :
        LegacySurrogate
    {

        public LegacySurrogate(string otherAssemblyName, string otherTypeName)
        {
            OtherAssemblyName = otherAssemblyName;
            OtherTypeName = otherTypeName;

            Properties = new List<FastProperty<T>>();
            foreach (var propertyInfo in typeof(T).GetProperties())
            {
                Properties.Add(new FastProperty<T>(propertyInfo));
            }
        }

        public string OtherAssemblyName { get; set; }
        public string OtherTypeName { get; set; }
        public List<FastProperty<T>> Properties { get; set; }

        public Type SurrogateType
        {
            get { return typeof(T); }
        }

        public void GetObjectData(object obj, SerializationInfo info, StreamingContext context)
        {
            var theReal = (T)obj;

            info.AssemblyName = OtherAssemblyName;
            info.FullTypeName = OtherTypeName;

            foreach (var fp in Properties)
            {
                var propName = fp.IsAutoProperty() ? fp.AutoPropertyFieldName() : fp.Property.Name;
                var value = fp.Get(theReal);

                info.FastInvoke("AddValue", propName, value);
            }
        }

        public object SetObjectData(object obj, SerializationInfo info, StreamingContext context, ISurrogateSelector selector)
        {
            throw new NotImplementedException("I cannot implement this.");
        }
    }

    public static class FastPropertyExtensions
    {
        public static bool IsAutoProperty<T>(this FastProperty<T> fp)
        {
            var g = fp.Property.GetGetMethod();
            return g.GetCustomAttributes(typeof(CompilerGeneratedAttribute), true).Length == 1;
        }

        public static string AutoPropertyFieldName<T>(this FastProperty<T> fp)
        {
            return string.Format("<{0}>k__BackingField", fp.Property.Name);
        }
    }
}