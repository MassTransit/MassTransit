// Copyright 2007-2012 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.Serialization
{
    /// <summary>
    /// The supported message serializers contains an implementation for each contentType
    /// </summary>
    public interface ISupportedMessageSerializers
    {
        /// <summary>
        /// Try to return a message serializer for the content type specified
        /// </summary>
        /// <param name="contentType">The content type string from the transport header</param>
        /// <param name="serializer">The serializer</param>
        /// <returns>True if the serializer was found, otherwise false</returns>
        bool TryGetSerializer(string contentType, out IMessageSerializer serializer);
    }
}