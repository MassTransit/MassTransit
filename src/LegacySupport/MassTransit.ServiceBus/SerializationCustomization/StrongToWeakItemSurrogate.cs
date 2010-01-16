// Copyright 2007-2008 The Apache Software Foundation.
// 
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use 
// this file except in compliance with the License. You may obtain a copy of the 
// License at 
// 
//     http://www.apache.org/licenses/LICENSE-2.0 
// 
// Unless required by applicable law or agreed to in writing, software distributed 
// under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR 
// CONDITIONS OF ANY KIND, either express or implied. See the License for the 
// specific language governing permissions and limitations under the License.
namespace MassTransit.LegacySupport.SerializationCustomization
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using System.Runtime.Serialization;
    using Magnum.Reflection;

    [DebuggerDisplay("{SurrogateTypeName}")]
    public class StrongToWeakItemSurrogate :
        LegacySurrogate
    {
        public StrongToWeakItemSurrogate(TypeMap map)
        {
            Map = map;
            Members = new List<MemberInfo>(FormatterServices.GetSerializableMembers(Map.StrongType));
        }

        public TypeMap Map { get; set; }
        public List<MemberInfo> Members { get; set; }

        #region LegacySurrogate Members

        public string SurrogateTypeName { get { return Map.StrongType.FullName; } }

        public void GetObjectData(object obj, SerializationInfo info, StreamingContext context)
        {
            info.AssemblyName = Map.WeakAssemblyName;
            info.FullTypeName = Map.WeakTypeName;

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

        #endregion
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