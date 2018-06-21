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
    /// <summary>
    /// Returns the symmetric key used to encrypt or decrypt messages
    /// </summary>
    public interface ISymmetricKeyProvider
    {
        /// <summary>
        /// Return the specified key, if found. When using Symmetric key encryption, the default key is used
        /// unless the transport header contains a specific key identifier for the message.
        /// </summary>
        /// <param name="id">The key id</param>
        /// <param name="key">The symmetric key</param>
        /// <returns></returns>
        bool TryGetKey(string id, out SymmetricKey key);
    }
}