// Copyright 2007-2011 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.Transports
{
    using System;
    using System.Runtime.Serialization;

    /// <summary>
    /// Class encapsulating naming strategies for exchanges corresponding
    /// to message types.
    /// </summary>
    [Serializable]
    public class MessageName :
        ISerializable
    {
        readonly string _name;

        public MessageName(string name)
        {
            _name = name;
        }

        protected MessageName(SerializationInfo info, StreamingContext context)
        {
            _name = info.GetString("Name");
        }

        public string Name
        {
            get { return _name; }
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Name", _name);
        }

        public override string ToString()
        {
            return _name ?? "";
        }
    }
}