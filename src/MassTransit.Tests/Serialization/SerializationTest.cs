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
namespace MassTransit.Tests.Serialization
{
	using System.Diagnostics;
	using System.IO;
	using System.Text;
	using MassTransit.Serialization;
	using NUnit.Framework;

	public abstract class SerializationTest<TSerializer> where TSerializer : IMessageSerializer, new()
	{
		private IMessageSerializer _serializer;

		[TestFixtureSetUp]
		public void Setup()
		{
			_serializer = new TSerializer();
		}

		protected T SerializeAndReturn<T>(T obj)
			where T : class
		{
			byte[] serializedMessageData;

			using (var output = new MemoryStream())
			{
				_serializer.Serialize(output, obj.ToSendContext());

				serializedMessageData = output.ToArray();

		//		Trace.WriteLine(Encoding.UTF8.GetString(serializedMessageData));
			}

			using (var input = new MemoryStream(serializedMessageData))
			{
				var result = _serializer.Deserialize(input.ToReceiveContext()) as T;

				return result;
			}
		}
	}
}