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
    using System;
    using System.IO;
    using System.Runtime.Serialization;
    using log4net;
    using Magnum.Cryptography;

    public class PreSharedKeyEncryptedMessageSerializer :
        IMessageSerializer
    {
        private readonly string _key;
        private static readonly ILog _log = LogManager.GetLogger(typeof (PreSharedKeyEncryptedMessageSerializer));
        private readonly IMessageSerializer _xmlSerializer;

        public PreSharedKeyEncryptedMessageSerializer(string key)
        {
            _key = key;
            _xmlSerializer = new XmlMessageSerializer();
        }

        public void Serialize<T>(Stream output, T message)
        {
            try
            {
                using (var clearStream = new MemoryStream())
                {
                    _xmlSerializer.Serialize(clearStream, message);

                    clearStream.Seek(0, SeekOrigin.Begin);

                    using (ICryptographyService cryptographyService = new RijndaelCryptographyService(_key))
                    {
                        EncryptedStream encryptedStream = cryptographyService.Encrypt(clearStream);

                        var encryptedMessage = new EncryptedMessageEnvelope
                            {
                                CipheredMessage = Convert.ToBase64String(encryptedStream.GetBytes()),
                                Iv = Convert.ToBase64String(encryptedStream.Iv),
                            };

                        _xmlSerializer.Serialize(output, encryptedMessage);
                    }
                }
            }
            catch (SerializationException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new SerializationException("Failed to serialize message", ex);
            }
        }

        public object Deserialize(Stream input)
        {
            object message = _xmlSerializer.Deserialize(input);
            if (message == null)
                throw new SerializationException("Could not deserialize message.");

            if (message is EncryptedMessageEnvelope)
            {
                var envelope = message as EncryptedMessageEnvelope;

                var cipherBytes = Convert.FromBase64String(envelope.CipheredMessage);
                var iv = Convert.FromBase64String(envelope.Iv);

                var cipherStream = new EncryptedStream(cipherBytes, iv);
                using (ICryptographyService cryptographyService = new RijndaelCryptographyService(_key))
                {
                    var clearStream = cryptographyService.Decrypt(cipherStream);

                    return _xmlSerializer.Deserialize(clearStream);
                }
            }
            return message;
        }
    }

    [Serializable]
    public class EncryptedMessageEnvelope
    {
        public string CipheredMessage { get; set; }
        public string Iv { get; set; }
    }
}