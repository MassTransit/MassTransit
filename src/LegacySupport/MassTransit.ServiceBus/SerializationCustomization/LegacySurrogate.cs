namespace MassTransit.LegacySupport.SerializationCustomization
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using System.Runtime.Serialization;
    using Magnum.Reflection;

    public class LegacySurrogate :
        ISerializationSurrogate
    {

        public LegacySurrogate(string otherAssemblyName, string otherTypeName, Type newType)
        {
            OtherAssemblyName = otherAssemblyName;
            OtherTypeName = otherTypeName;
            SurrogateType = newType;
            Members = new List<MemberInfo>(FormatterServices.GetSerializableMembers(newType));
        }

        public string OtherAssemblyName { get; set; }
        public string OtherTypeName { get; set; }
        public List<MemberInfo> Members { get; set; }

        public Type SurrogateType
        {
            get; private set;
        }

        public void GetObjectData(object obj, SerializationInfo info, StreamingContext context)
        {

            info.AssemblyName = OtherAssemblyName;
            info.FullTypeName = OtherTypeName;

            foreach (FieldInfo fp in Members)
            {
                var propName = fp.Name;
                var value = fp.GetValue(obj);

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