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
namespace MassTransit.Serialization
{
    using System.IO;

    /// <summary>
    /// Message Serialization implementors should handle the nitty-gritty
    /// details of writing object instances to streams and reading them back
    /// up from streams.
    /// </summary>
    public interface IMessageSerializer
    {
        /// <summary>
        /// The content type that identifies the message serializer
        /// </summary>
        string ContentType { get; }

        /// <summary>
        /// Serialize the message to the stream
        /// </summary>
        /// <typeparam name="T">The implicit type of the message to serialize</typeparam>
        /// <param name="stream">The stream to write the context to</param>
        /// <param name="context">The context to send</param>
        void Serialize<T>(Stream stream, ISendContext<T> context)
            where T : class;

        /// <summary>
        /// Deserialize a message from the stream by reading the 
        /// bits in the <see cref="IReceiveContext.BodyStream"/>
        /// and hydrating the context with that.
        /// </summary>
        /// <param name="context">The context to deserialize</param>
        /// <returns>An object that was deserialized</returns>
        void Deserialize(IReceiveContext context);
    }
}