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
namespace MassTransit.Transports
{
	using System;
	using System.Linq.Expressions;
	using Configuration;
	using Exceptions;
	using Serialization;

    public class EndpointConfigurator :
        EndpointConfiguratorBase
    {
        
    }

	public class EndpointConfiguratorBase :
		IEndpointConfigurator
	{
		protected Type SerializerType { get; private set; }
        protected IMessageSerializer MessageSerializer { get; private set; }
		protected Uri Uri { get; private set; }

		public void SetSerializer<T>()
			where T : IMessageSerializer
		{
			SerializerType = typeof (T);
		}

        public void SetSerializer(IMessageSerializer serializer)
        {
            MessageSerializer = serializer;
        }

        public void SetSerializer(Type serializerType)
        {
            SerializerType = serializerType;
        }

	    public void SetUri(Uri uri)
		{
			Uri = uri;
		}

		protected IMessageSerializer GetSerializer()
		{
            if (MessageSerializer != null) return MessageSerializer;

		    var newExpression = Expression.New(SerializerType);
			Func<IMessageSerializer> maker = Expression.Lambda<Func<IMessageSerializer>>(newExpression).Compile();

			IMessageSerializer serializer = maker();

			if (serializer == null)
				throw new ConfigurationException("Unable to create message serializer " + SerializerType.FullName);

			return serializer;
		}
	}
}