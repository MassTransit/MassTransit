/// Copyright 2007-2008 The Apache Software Foundation.
/// 
/// Licensed under the Apache License, Version 2.0 (the "License"); you may not use 
/// this file except in compliance with the License. You may obtain a copy of the 
/// License at 
/// 
///   http://www.apache.org/licenses/LICENSE-2.0 
/// 
/// Unless required by applicable law or agreed to in writing, software distributed 
/// under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR 
/// CONDITIONS OF ANY KIND, either express or implied. See the License for the 
/// specific language governing permissions and limitations under the License.
namespace MassTransit.ServiceBus.Internal
{
	/// <summary>
	/// Generic interface for mapping envelopes to messages
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public interface IEnvelopeMapper<T>
	{
		/// <summary>
		/// Maps an envelope to a message
		/// </summary>
		/// <param name="envelope"></param>
		/// <returns></returns>
		T ToMessage(IEnvelope envelope);

		/// <summary>
		/// Maps a message to an envelope
		/// </summary>
		/// <param name="message"></param>
		/// <returns></returns>
		IEnvelope ToEnvelope(T message);
	}
}