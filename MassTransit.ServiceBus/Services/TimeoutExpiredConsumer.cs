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
namespace MassTransit.Services
{
	using System;
	using System.Reflection;
	using MassTransit.ServiceBus.Timeout.Messages;
	using Messages;
	using ServiceBus;

	public class TimeoutExpiredConsumer :
		Consumes<TimeoutExpired>.Selected
	{
		private static readonly MethodInfo _publishMethodInfo = typeof (IServiceBus).GetMethod("Publish", BindingFlags.Public | BindingFlags.Instance);

		private readonly IServiceBus _bus;
		private readonly IDeferredMessageRepository _deferred;

		public TimeoutExpiredConsumer(IServiceBus bus, IDeferredMessageRepository deferred)
		{
			_deferred = deferred;
			_bus = bus;
		}

		public void Consume(TimeoutExpired message)
		{
			DeferMessage deferredMessage = _deferred.Get(message.CorrelationId);

			RepublishMessage(deferredMessage.Message);

			_deferred.Remove(message.CorrelationId);
		}

		public bool Accept(TimeoutExpired message)
		{
			return _deferred.Contains(message.CorrelationId);
		}

		private void RepublishMessage(object message)
		{
			Type objType = message.GetType();

			MethodInfo inv = GetPublishMethod(objType);

			inv.Invoke(_bus, new[] {message});
		}

		private static MethodInfo GetPublishMethod(Type objType)
		{
			return _publishMethodInfo.MakeGenericMethod(objType);
		}
	}
}