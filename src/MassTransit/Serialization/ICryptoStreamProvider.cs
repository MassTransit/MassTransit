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
namespace MassTransit.Serialization
{
    using System.IO;
    using System.Security.Cryptography;
    using GreenPipes;


    /// <summary>
    /// Provides a crypto stream for the purpose of encrypting or decrypting 
    /// </summary>
    public interface ICryptoStreamProvider :
        IProbeSite
    {
        /// <summary>
        /// Returns a stream with the encryption bits in place to ensure proper message encryption
        /// </summary>
        /// <param name="stream">The original stream to which the encrypted message content is written</param>
        /// <param name="keyId">The encryption key identifier</param>
        /// <param name="streamMode"></param>
        /// <returns>A stream for serializing the message which will be encrypted</returns>
        Stream GetEncryptStream(Stream stream, string keyId, CryptoStreamMode streamMode);

        /// <summary>
        /// Returns a stream for decrypting the message
        /// </summary>
        /// <param name="stream">The input stream of the encrypted message</param>
        /// <param name="keyId">The encryption key identifier</param>
        /// <param name="streamMode"></param>
        /// <returns>A stream for deserializing the encrypted message</returns>
        Stream GetDecryptStream(Stream stream, string keyId, CryptoStreamMode streamMode);
    }
}