// Copyright 2007-2015 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
    using System.IO;
    using System.Security.Cryptography;
    using Serialization;
    using Transformation;


    public static class CryptoStreamProviderExtensions
    {
        /// <summary>
        /// Returns a stream with the encryption bits in place to ensure proper message encryption
        /// </summary>
        /// <param name="provider">The crypto stream provider</param>
        /// <param name="stream">The original stream to which the encrypted message content is written</param>
        /// <param name="context">The second context of the message</param>
        /// <returns>A stream for serializing the message which will be encrypted</returns>
        public static Stream GetEncryptStream(this ICryptoStreamProvider provider, Stream stream, SendContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            object keyIdObj;
            string keyId = context.Headers.TryGetHeader(EncryptedMessageSerializer.EncryptionKeyHeader, out keyIdObj)
                ? keyIdObj.ToString()
                : default(string);

            return provider.GetEncryptStream(stream, keyId, CryptoStreamMode.Write);
        }

        /// <summary>
        /// Returns a stream with the encryption bits in place to ensure proper message encryption
        /// </summary>
        /// <param name="provider">The crypto stream provider</param>
        /// <param name="stream">The original stream to which the encrypted message content is written</param>
        /// <param name="context">The second context of the message</param>
        /// <returns>A stream for serializing the message which will be encrypted</returns>
        public static Stream GetEncryptStream(this ICryptoStreamProvider provider, Stream stream, TransformContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            object keyIdObj;
            string keyId = context.Headers.TryGetHeader(EncryptedMessageSerializer.EncryptionKeyHeader, out keyIdObj)
                ? keyIdObj.ToString()
                : default(string);

            return provider.GetEncryptStream(stream, keyId, CryptoStreamMode.Write);
        }

        /// <summary>
        /// Returns a stream for decrypting the message
        /// </summary>
        /// <param name="provider">The crypto stream provider</param>
        /// <param name="stream">The original stream to which the encrypted message content is written</param>
        /// <param name="context">The second context of the message</param>
        /// <returns>A stream for serializing the message which will be encrypted</returns>
        public static Stream GetDecryptStream(this ICryptoStreamProvider provider, Stream stream, ReceiveContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            object keyIdObj;
            string keyId = context.TransportHeaders.TryGetHeader(EncryptedMessageSerializer.EncryptionKeyHeader, out keyIdObj)
                ? keyIdObj.ToString()
                : default(string);

            return provider.GetDecryptStream(stream, keyId, CryptoStreamMode.Read);
        }

        /// <summary>
        /// Returns a stream for decrypting the message
        /// </summary>
        /// <param name="provider">The crypto stream provider</param>
        /// <param name="stream">The original stream to which the encrypted message content is written</param>
        /// <param name="context">The second context of the message</param>
        /// <returns>A stream for serializing the message which will be encrypted</returns>
        public static Stream GetDecryptStream(this ICryptoStreamProvider provider, Stream stream, TransformContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            object keyIdObj;
            string keyId = context.Headers.TryGetHeader(EncryptedMessageSerializer.EncryptionKeyHeader, out keyIdObj)
                ? keyIdObj.ToString()
                : default(string);

            return provider.GetDecryptStream(stream, keyId, CryptoStreamMode.Read);
        }
    }
}