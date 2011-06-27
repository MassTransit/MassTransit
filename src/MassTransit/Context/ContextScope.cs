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
namespace MassTransit.Context
{
	using System;
	using System.Diagnostics;

	public class ContextScope :
		IDisposable
	{
		readonly Action _restore;

		ContextScope(Action restore)
		{
			_restore = restore;
		}

		public static IDisposable FromPublishContext<T>(IBusPublishContext<T> context)
			where T : class
		{
			var previousContext = ContextStorage.Retrieve<ISendContext>(ContextStorage.OutboundContextKey);

			Debug.Assert(!ReferenceEquals(previousContext, context));

			Action restore = () => ContextStorage.Store(ContextStorage.OutboundContextKey, previousContext);

			var receiveContext = ContextStorage.Retrieve<IReceiveContext>(ContextStorage.InboundContextKey);
			context.SetReceiveContext(receiveContext);

			ContextStorage.Store(ContextStorage.OutboundContextKey, context);

			return new ContextScope(restore);
		}

		public static IDisposable FromSendContext<T>(ISendContext<T> context)
			where T : class
		{
			var previousContext = ContextStorage.Retrieve<ISendContext>(ContextStorage.OutboundContextKey);

			Debug.Assert(!ReferenceEquals(previousContext, context));

			Action restore = () => ContextStorage.Store(ContextStorage.OutboundContextKey, previousContext);

			var receiveContext = ContextStorage.Retrieve<IReceiveContext>(ContextStorage.InboundContextKey);
			context.SetReceiveContext(receiveContext);

			ContextStorage.Store(ContextStorage.OutboundContextKey, context);

			return new ContextScope(restore);
		}

		public static IDisposable FromReceiveContext(IReceiveContext context)
		{
			var previousContext = ContextStorage.Retrieve<IConsumeContext>(ContextStorage.InboundContextKey);

			Debug.Assert(!ReferenceEquals(previousContext, context));

			Action restore = () => ContextStorage.Store(ContextStorage.InboundContextKey, previousContext);

			ContextStorage.Store(ContextStorage.InboundContextKey, context);

			return new ContextScope(restore);
		}

		public static IDisposable FromConsumeContext<T>(IConsumeContext<T> context)
			where T : class
		{
			var previousContext = ContextStorage.Retrieve<IConsumeContext>(ContextStorage.InboundContextKey);

			Debug.Assert(!ReferenceEquals(previousContext, context));

			Action restore = () => ContextStorage.Store(ContextStorage.InboundContextKey, previousContext);

			ContextStorage.Store(ContextStorage.InboundContextKey, context);

			return new ContextScope(restore);
		}

		public void Dispose()
		{
			_restore();
		}
	}
}