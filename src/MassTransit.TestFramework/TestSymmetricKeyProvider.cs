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
namespace MassTransit.TestFramework
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Security.Cryptography;
    using Serialization;


    /// <summary>
    /// Creates a single symmetric key and returns that key for every id specified
    /// </summary>
    public class TestSymmetricKeyProvider :
        ISymmetricKeyProvider
    {
        readonly Dictionary<string, TestSymmetricKey> _keys;

        public TestSymmetricKeyProvider(params string[] ids)
        {
            _keys = new Dictionary<string, TestSymmetricKey>();

            foreach (var id in ids.Concat(new[] {"default"}).Distinct())
            {
                byte[] key;
                byte[] iv;

                using (var aes = Aes.Create())
                {
                    aes.GenerateKey();
                    aes.GenerateIV();

                    key = aes.Key;
                    iv = aes.IV;
                }

                _keys.Add(id, new TestSymmetricKey(key, iv));
            }
        }

        public bool TryGetKey(string id, out SymmetricKey key)
        {
            TestSymmetricKey symmetricKey;
            bool found = _keys.TryGetValue(id, out symmetricKey);
            if (found)
            {
                key = symmetricKey;
                return true;
            }

            key = null;
            return false;
        }


        class TestSymmetricKey :
            SymmetricKey
        {
            readonly byte[] _iv;
            readonly byte[] _key;

            public TestSymmetricKey(byte[] key, byte[] iv)
            {
                _key = key;
                _iv = iv;
            }

            byte[] SymmetricKey.Key => _key;
            byte[] SymmetricKey.IV => _iv;
        }
    }
}