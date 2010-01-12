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
    using System.Reflection;
    using System.Runtime.Serialization;

    public class WeakToStrongListSurrogate<LIST,ITEM> :
        LegacySurrogate where LIST : List<ITEM>
    {
        public WeakToStrongListSurrogate(string weakAssemblyName, string weakTypeName)
        {
            WeakTypeName = weakTypeName;
            WeakAssemblyName = weakAssemblyName;
            StrongType = typeof(LIST);
        }

        public string WeakAssemblyName { get; set; }
        public string WeakTypeName { get; set; }
        public Type StrongType { get; set; }
        public List<MemberInfo> Members { get; set; }

        #region LegacySurrogate Members

        public string SurrogateTypeName{ get { return WeakTypeName; } }

        public void GetObjectData(object obj, SerializationInfo info, StreamingContext context)
        {
            throw new NotImplementedException();
        }

        public object SetObjectData(object obj, SerializationInfo info, StreamingContext context, ISurrogateSelector selector)
        {
            if (!obj.GetType().FullName.Equals(SurrogateTypeName))
                throw new Exception("something");

            var value = info.GetValue("_items", typeof(object[]));
            var data =  (ITEM[])value;
            return new List<ITEM>(data);
        }

        #endregion
    }
}