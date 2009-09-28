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
namespace MassTransit.Tests.Pipeline
{
	using Messages;

	public class ParticularConsumer :
		Consumes<PingMessage>.Selected
	{
		private readonly bool _accept;

		protected ParticularConsumer()
		{
			
		}

		public ParticularConsumer(bool accept)
		{
			_accept = accept;
		}

		public PingMessage Consumed { get; private set; }

		public virtual void Consume(PingMessage message)
		{
			Consumed = message;
		}

		public virtual bool Accept(PingMessage message)
		{
			return _accept;
		}
	}
}