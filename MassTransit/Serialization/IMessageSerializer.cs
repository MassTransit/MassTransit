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
namespace MassTransit.Serialization
{
    using System.IO;

    /// <summary>
    /// Message Serialization Methods 
    /// </summary>
    public interface IMessageSerializer
    {
        /// <summary>
        /// Serialize the message to the stream
        /// </summary>
        /// <typeparam name="T">The implicit type of the message to serialize</typeparam>
        /// <param name="output">The stream where the output should be directed</param>
        /// <param name="message">The message to serialize</param>
        void Serialize<T>(Stream output, T message);

        /// <summary>
        /// Deserialize a message from the stream
        /// </summary>
        /// <param name="input">The input stream where the serializer should read from</param>
        /// <returns>An object that was deserialized</returns>
        object Deserialize(Stream input);
    }
}