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
namespace MassTransit.Tests.TestConsumers
{
	using Messages;
	using TestFramework.Messages;


    public class TestMessageConsumer<TMessage> :
		TestConsumerBase<TMessage>,
		Consumes<TMessage>.All
		where TMessage : class
	{
	}

	public class PingPongConsumer :
		Consumes<PingMessage>.All,
		Consumes<PongMessage>.All
	{
		private readonly TestMessageConsumer<PingMessage> _ping = new TestMessageConsumer<PingMessage>();
		private readonly TestMessageConsumer<PongMessage> _pong = new TestMessageConsumer<PongMessage>();

		public TestMessageConsumer<PingMessage> Ping
		{
			get { return _ping; }
		}

		public TestMessageConsumer<PongMessage> Pong
		{
			get { return _pong; }
		}

		public virtual void Consume(PingMessage message)
		{
			_ping.Consume(message);
		}

		public virtual void Consume(PongMessage message)
		{
			_pong.Consume(message);
		}
	}
}