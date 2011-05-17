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
namespace MassTransit
{
	using System;
	using Context;

	public static class ContextExtensions
	{
		public static IConsumeContext Context(this IServiceBus bus)
		{
			return ContextStorage.Context();
		}

		public static IConsumeContext<T> MessageContext<T>(this IServiceBus bus)
		{
			return ContextStorage.MessageContext<T>();
		}

		public static void Context(this IServiceBus bus, Action<IConsumeContext> contextCallback)
		{
			ContextStorage.Context(contextCallback);
		}

		public static TResult Context<TResult>(this IServiceBus bus, Func<IConsumeContext, TResult> contextCallback)
		{
			return ContextStorage.Context(contextCallback);
		}



//		public static TResult ConsumeContext<TResult>(this IServiceBus bus, Func<IConsumeContext, TResult> accessor)
//		{
//			if (bus == null)
//				throw new ArgumentNullException("bus");
//
//			return bus.Context(accessor);
//		}
//
//		public static void ConsumeContext(this IServiceBus bus, Action<IConsumeContext> accessor)
//		{
//			if (bus == null)
//				throw new ArgumentNullException("bus");
//
//			bus.Context(accessor);
//		}
//
//		public static TResult PublishContext<TResult>(this IServiceBus bus, Func<IPublishContext, TResult> accessor)
//		{
//			if (bus == null)
//				throw new ArgumentNullException("bus");
//
//			return bus.Context(accessor);
//		}
//	
//		public static PublishContext<T> PublishContext<T>(this IServiceBus bus, T message, Action<IPublishContext<T>> contextAction)
//			where T : class
//		{
//			if (bus == null)
//				throw new ArgumentNullException("bus");
//
//			return bus.Context<PublishContext<T>, T>(contextAction);
//		}
//
//		public static void SendContext(this IServiceBus bus, Action<ISendContext> accessor)
//		{
//			if (bus == null)
//				throw new ArgumentNullException("bus");
//
//			bus.Context(accessor);
//		}
	}
}