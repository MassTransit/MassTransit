// Copyright 2007-2014 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit
{
    using System;
    using System.Runtime.Serialization;


    /// <summary>
    /// An endpoint address is really just a URI, nothing more, nothing less
    /// </summary>
    public class EndpointAddress :
        Uri
    {
        public EndpointAddress(string uriString)
            : base(uriString)
        {
        }

        public EndpointAddress(Uri uri)
            : base(uri.ToString())
        {
            
        }

        protected EndpointAddress(SerializationInfo serializationInfo, StreamingContext streamingContext)
            : base(serializationInfo, streamingContext)
        {
        }
    }
}