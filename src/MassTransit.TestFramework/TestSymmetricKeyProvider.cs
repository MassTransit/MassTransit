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
    using System.Security.Cryptography;
    using Serialization;


    public class TestSymmetricKeyProvider :
        ISymmetricKeyProvider
    {
        readonly TestSymmetricKey _key;

        public TestSymmetricKeyProvider()
        {
            byte[] key;
            byte[] iv;

            using (var provider = new AesCryptoServiceProvider())
            {
                provider.GenerateIV();
                provider.GenerateKey();

                key = provider.Key;
                iv = provider.IV;

                provider.Clear();
            }

            _key = new TestSymmetricKey(key, iv);
        }

        public bool TryGetKey(string id, out SymmetricKey key)
        {
            key = _key;
            return true;
        }


        public class TestSymmetricKey :
            SymmetricKey
        {
            readonly byte[] _iv;
            readonly byte[] _key;

            public TestSymmetricKey(byte[] key, byte[] iv)
            {
                _key = key;
                _iv = iv;
            }

            byte[] SymmetricKey.Key
            {
                get { return _key; }
            }

            byte[] SymmetricKey.IV
            {
                get { return _iv; }
            }
        }
    }
}