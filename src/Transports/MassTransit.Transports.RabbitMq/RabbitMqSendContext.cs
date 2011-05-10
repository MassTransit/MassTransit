// Copyright 2007-2011 The Apache Software Foundation.
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
namespace MassTransit.Transports.RabbitMq
{
	using System;
	using System.IO;
	using Magnum;
	using RabbitMQ.Client;

	public class RabbitMqSendContext :
		ISendContext
	{
		private readonly IBasicProperties _properties;
		private MemoryStream _body;

		public RabbitMqSendContext(IModel channel)
		{
			_body = new MemoryStream();
			_properties = channel.CreateBasicProperties();
		}

		public IBasicProperties Properties
		{
			get { return _properties; }
		}

		public Stream Body
		{
			get { return _body; }
		}

		public void MarkRecoverable()
		{
			_properties.DeliveryMode = 2;
		}

		public void SetLabel(string label)
		{
			//_properties.Headers["label"] = label;
		}

		public void SetMessageExpiration(DateTime d)
		{
			_properties.Expiration = (d.Kind == DateTimeKind.Utc ? d - SystemUtil.UtcNow : d - SystemUtil.Now).ToString();
		}

		public void Dispose()
		{
			if (_body != null)
			{
				_body.Dispose();
				_body = null;
			}
		}

		public byte[] GetBytes()
		{
			return _body.ToArray();
		}
	}
}