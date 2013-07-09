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
	using Magnum.Cryptography;

	public class PreSharedKeyEncryptedMessageSerializer :
		IMessageSerializer
	{
		const string ContentTypeHeaderValue = "application/vnd.masstransit+psk";

		readonly string _key;
		readonly IMessageSerializer _wrappedSerializer;

		public PreSharedKeyEncryptedMessageSerializer(string key, IMessageSerializer serializer)
		{
			_key = key;
			_wrappedSerializer = serializer;
		}

		public string ContentType
		{
			get { return ContentTypeHeaderValue; }
		}

		public void Serialize<T>(Stream output, ISendContext<T> context)
			where T : class
		{
			try
			{
				using (var clearStream = new MemoryStream())
				{
					_wrappedSerializer.Serialize(clearStream, context);

					using (var readStream = new MemoryStream(clearStream.ToArray(), false))
					{
						using (ICryptographyService cryptographyService = new RijndaelCryptographyService(_key))
						{
							EncryptedStream encryptedStream = cryptographyService.Encrypt(readStream);

							var encryptedMessage = new EncryptedMessageEnvelope
								{
									CipheredMessage = Convert.ToBase64String(encryptedStream.GetBytes()),
									Iv = Convert.ToBase64String(encryptedStream.Iv),
								};

							// Encrypt message and set context
							var encryptedContext = new SendContext<EncryptedMessageEnvelope>(encryptedMessage);
							encryptedContext.SetUsing(context);
							encryptedContext.SetMessageType(typeof (EncryptedMessageEnvelope));

							// Serialize secure message to output
							_wrappedSerializer.Serialize(output, encryptedContext);

							// Set the encrypted context back into the send context
							encryptedContext.SetContentType(ContentTypeHeaderValue);
							context.SetUsing(encryptedContext);
						}
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

		public void Deserialize(IReceiveContext context)
		{
			_wrappedSerializer.Deserialize(context);
			IConsumeContext<EncryptedMessageEnvelope> encryptedContext;
			context.TryGetContext(out encryptedContext);

			if (encryptedContext == null)
				throw new SerializationException("Could not deserialize message.");


			byte[] cipherBytes = Convert.FromBase64String(encryptedContext.Message.CipheredMessage);
			byte[] iv = Convert.FromBase64String(encryptedContext.Message.Iv);

			var cipherStream = new EncryptedStream(cipherBytes, iv);
			using (ICryptographyService cryptographyService = new RijndaelCryptographyService(_key))
			{
				Stream clearStream = cryptographyService.Decrypt(cipherStream);

				context.SetBodyStream(clearStream);

				_wrappedSerializer.Deserialize(context);
			}
		}
	}
}