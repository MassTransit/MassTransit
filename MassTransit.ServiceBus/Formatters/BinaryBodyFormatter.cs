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
namespace MassTransit.ServiceBus.Formatters
{
	using System.Runtime.Serialization;
	using System.Runtime.Serialization.Formatters.Binary;
	using Util;

    public class BinaryBodyFormatter
		: IBodyFormatter
	{
		private static readonly IFormatter _formatter = new BinaryFormatter();

		public void Serialize(IFormattedBody body, object message)
		{
            Check.EnsureSerializable(message);
			_formatter.Serialize(body.BodyStream, message);
		}

		public T Deserialize<T>(IFormattedBody formattedBody) where T : class
		{
			T messages = _formatter.Deserialize(formattedBody.BodyStream) as T;

			return messages;
		}

		object IBodyFormatter.Deserialize(IFormattedBody formattedBody)
		{
			object obj = _formatter.Deserialize(formattedBody.BodyStream);

			return obj;
		}
	}
}