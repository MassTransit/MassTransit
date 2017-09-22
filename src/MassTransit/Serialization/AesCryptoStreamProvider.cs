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
    using System;
    using System.IO;
    using System.Runtime.Serialization;
    using System.Security.Cryptography;
    using GreenPipes;


    public class AesCryptoStreamProvider :
        ICryptoStreamProvider
    {
        readonly string _defaultKeyId;
        readonly ISymmetricKeyProvider _keyProvider;
        readonly PaddingMode _paddingMode;

        public AesCryptoStreamProvider(ISymmetricKeyProvider keyProvider, string defaultKeyId, PaddingMode paddingMode = PaddingMode.PKCS7)
        {
            _paddingMode = paddingMode;
            _keyProvider = keyProvider;
            _defaultKeyId = defaultKeyId;
        }

        Stream ICryptoStreamProvider.GetEncryptStream(Stream stream, string keyId, CryptoStreamMode streamMode)
        {
            if (stream == null)
                throw new ArgumentNullException(nameof(stream));

            keyId = keyId ?? _defaultKeyId;

            SymmetricKey key;
            if (!_keyProvider.TryGetKey(keyId, out key))
                throw new SerializationException("Encryption Key not found: " + keyId);

            ICryptoTransform encryptor = CreateEncryptor(key.Key, key.IV);

            return new DisposingCryptoStream(stream, encryptor, streamMode);
        }

        Stream ICryptoStreamProvider.GetDecryptStream(Stream stream, string keyId, CryptoStreamMode streamMode)
        {
            if (stream == null)
                throw new ArgumentNullException(nameof(stream));

            keyId = keyId ?? _defaultKeyId;

            SymmetricKey key;
            if (!_keyProvider.TryGetKey(keyId, out key))
                throw new SerializationException("Encryption Key not found: " + keyId);

            ICryptoTransform encryptor = CreateDecryptor(key.Key, key.IV);

            return new DisposingCryptoStream(stream, encryptor, streamMode);
        }

        public void Probe(ProbeContext context)
        {
            context.Add("defaultKeyId", _defaultKeyId);
            context.Add("paddingMode", _paddingMode.ToString());
        }

        ICryptoTransform CreateDecryptor(byte[] key, byte[] iv)
        {
            using (var provider = CreateAes())
            {
                return provider.CreateDecryptor(key, iv);
            }
        }

        public ICryptoTransform CreateEncryptor(byte[] key, byte[] iv)
        {
            using (var provider = CreateAes())
            {
                return provider.CreateEncryptor(key, iv);
            }
        }

        Aes CreateAes()
            => new AesCryptoServiceProvider {Padding = _paddingMode};

        class DisposingCryptoStream :
            CryptoStream
        {
            Stream _stream;
            ICryptoTransform _transform;

            public DisposingCryptoStream(Stream stream, ICryptoTransform transform, CryptoStreamMode mode)
                : base(stream, transform, mode)
            {
                _stream = stream;
                _transform = transform;
            }

            protected override void Dispose(bool disposing)
            {
                if (!disposing)
                    return;

                base.Dispose(true);

                if (_stream != null)
                {
                    _stream.Dispose();
                    _stream = null;
                }

                if (_transform != null)
                {
                    _transform.Dispose();
                    _transform = null;
                }
            }
        }
    }
}