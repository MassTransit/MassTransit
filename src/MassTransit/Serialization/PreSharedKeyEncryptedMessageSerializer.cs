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
	using System;
	using System.IO;
	using System.Runtime.Serialization;
	using Context;
	using log4net;
	using Magnum.Cryptography;

	public class PreSharedKeyEncryptedMessageSerializer :
		IMessageSerializer
	{
		static readonly ILog _log = LogManager.GetLogger(typeof (PreSharedKeyEncryptedMessageSerializer));
		readonly string _key;
		readonly IMessageSerializer _wrappedSerializer;

		public PreSharedKeyEncryptedMessageSerializer(string key, IMessageSerializer serializer)
		{
			_key = key;
			_wrappedSerializer = serializer;
		}

		public void Serialize<T>(Stream output, ISendContext<T> context)
			where T : class
		{
			try
			{
				using (var clearStream = new MemoryStream())
				{
					_wrappedSerializer.Serialize(clearStream, context);

					clearStream.Seek(0, SeekOrigin.Begin);

					using (ICryptographyService cryptographyService = new RijndaelCryptographyService(_key))
					{
						EncryptedStream encryptedStream = cryptographyService.Encrypt(clearStream);

						var encryptedMessage = new EncryptedMessageEnvelope
							{
								CipheredMessage = Convert.ToBase64String(encryptedStream.GetBytes()),
								Iv = Convert.ToBase64String(encryptedStream.Iv),
							};

						var encryptedContext = new SendContext<EncryptedMessageEnvelope>(encryptedMessage);
						encryptedContext.SetUsing(context);
						encryptedContext.SetMessageType(typeof (EncryptedMessageEnvelope));

						_wrappedSerializer.Serialize(output, encryptedContext);
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

		public object Deserialize(IReceiveContext context)
		{
			object message = _wrappedSerializer.Deserialize(context);
			if (message == null)
				throw new SerializationException("Could not deserialize message.");

			if (message is EncryptedMessageEnvelope)
			{
				var envelope = message as EncryptedMessageEnvelope;

				byte[] cipherBytes = Convert.FromBase64String(envelope.CipheredMessage);
				byte[] iv = Convert.FromBase64String(envelope.Iv);

				var cipherStream = new EncryptedStream(cipherBytes, iv);
				using (ICryptographyService cryptographyService = new RijndaelCryptographyService(_key))
				{
					Stream clearStream = cryptographyService.Decrypt(cipherStream);

					var encryptedContext = new ConsumeContext(clearStream);

					return _wrappedSerializer.Deserialize(encryptedContext);
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