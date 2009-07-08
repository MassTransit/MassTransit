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
        static readonly ILog _log = LogManager.GetLogger(typeof(PreSharedKeyEncryptedMessageSerializer));
        readonly IMessageSerializer _wrappedSerializer;
        ICryptographyService _cryptography;

        public PreSharedKeyEncryptedMessageSerializer(IMessageSerializer wrappedSerializer, ICryptographyService cryptography)
        {
            _wrappedSerializer = wrappedSerializer;
            _cryptography = cryptography;
        }

        #region IMessageSerializer Members

        public void Serialize<T>(Stream output, T message)
        {
            try
            {
                var plainStream = new MemoryStream();

                //turn into XML
                _wrappedSerializer.Serialize(plainStream, message);

                //encrypt it
                var encryptionResult = _cryptography.Encrypt(plainStream);

                //wrap in envelope
                var cipherBuffer = new byte[encryptionResult.CipherStream.Length];
                encryptionResult.CipherStream.Read(cipherBuffer, 0, cipherBuffer.Length);

                var msg = new EncryptedMessageEnvelope()
                          {
                              CipheredMessage = Convert.ToBase64String(cipherBuffer),
                              IV = Convert.ToBase64String(encryptionResult.Iv)
                          };

                _wrappedSerializer.Serialize(output, msg);
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
            object message = _wrappedSerializer.Deserialize(input);

            if (message == null)
                throw new SerializationException("Could not deserialize message.");

            if (message is EncryptedMessageEnvelope)
            {
                EncryptedMessageEnvelope envelope = message as EncryptedMessageEnvelope;

                var iv = Convert.FromBase64String(envelope.IV);
                var cipherBytes = Convert.FromBase64String(envelope.CipheredMessage);

                var cipherStream = new MemoryStream(cipherBytes);
                var er = new EncryptionStreamResult(cipherStream, iv);
                var decryptedStream = _cryptography.Decrypt(er);

                return _wrappedSerializer.Deserialize(decryptedStream);
            }

            return message;
        }

        #endregion
    }

    [Serializable]
    public class EncryptedMessageEnvelope
    {
        public string CipheredMessage { get; set; }
        public string IV { get; set; }
    }
}