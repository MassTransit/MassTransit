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
namespace MassTransit.Serialization.Custom
{
	using System.IO;

	public interface IXmlSerializer
	{
		void Serialize<T>(Stream stream, T message) where T : class;
		void Serialize<T>(Stream stream, T message, SerializerTypeMapper typeMapper) where T : class;
	    void Serialize<T>(TextWriter textWriter, T message) where T : class;
        byte[] Serialize<T>(T message) where T : class;

        object Deserialize(Stream input);
	    object Deserialize(TextReader textReader);
		object Deserialize(byte[] data);
	}
}