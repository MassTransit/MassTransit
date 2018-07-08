// Copyright 2007-2018 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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


    public class AesCryptoStreamProviderV2 : ICryptoStreamProviderV2
    {
        readonly PaddingMode _paddingMode;
        readonly ISecureKeyProvider _secureKeyProvider;

        public AesCryptoStreamProviderV2(ISecureKeyProvider secureKeyProvider,
            PaddingMode paddingMode = PaddingMode.PKCS7)
        {
            _secureKeyProvider = secureKeyProvider;
            _paddingMode = paddingMode;
        }

        public Stream GetDecryptStream(Stream stream, ReceiveContext context)
        {
            var key = _secureKeyProvider.GetKey(context);

            var iv = new byte[16];
            stream.Read(iv, 0, iv.Length);

            var encryptor = CreateDecryptor(key, iv);

            return new DisposingCryptoStream(stream, encryptor, CryptoStreamMode.Read);
        }

        public Stream GetEncryptStream(Stream stream, SendContext context)
        {
            var key = _secureKeyProvider.GetKey(context);

            var iv = GenerateIv();

            stream.Write(iv, 0, iv.Length);
            var encryptor = CreateEncryptor(key, iv);

            return new DisposingCryptoStream(stream, encryptor, CryptoStreamMode.Write);
        }

        public void Probe(ProbeContext context)
        {
            var scope = context.CreateScope("aes");

            scope.Add("paddingMode", _paddingMode.ToString());

            _secureKeyProvider.Probe(scope);
        }

        ICryptoTransform CreateDecryptor(byte[] key, byte[] iv)
        {
            using (var provider = CreateAes())
            {
                return provider.CreateDecryptor(key, iv);
            }
        }

        byte[] GenerateIv()
        {
            using (var aes = CreateAes())
            {
                aes.GenerateIV();

                return aes.IV;
            }
        }

        ICryptoTransform CreateEncryptor(byte[] key, byte[] iv)
        {
            using (var provider = CreateAes())
            {
                return provider.CreateEncryptor(key, iv);
            }
        }

        Aes CreateAes()
        {
            return new AesCryptoServiceProvider { Padding = _paddingMode };
        }
    }
}
