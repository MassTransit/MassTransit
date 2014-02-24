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
    using Logging;
    using Util;
    using Magnum.Extensions;

	/// <summary>
	/// API-oriented class.
	/// </summary>
	/// <typeparam name="TMessage">The message type to consume.</typeparam>
	[UsedImplicitly]
	public static class Consumes<TMessage>
		where TMessage : class
	{
		static readonly All _null;

		static Consumes()
		{
			_null = new NullConsumer();
		}

		public static All Null
		{
			get { return _null; }
		}

		class NullConsumer :
			All
		{
			static readonly ILog _log = Logger.Get(typeof (NullConsumer));
			readonly string _message;

			public NullConsumer()
			{
				_message = "A message of type " + typeof (TMessage).ToShortTypeName() + " was discarded: (NullConsumer)";
			}

			public void Consume(TMessage message)
			{
				_log.Warn(_message);
			}
		}

		/// <summary>
		/// Declares a Consume method for the message type TMessage which is called
		/// whenever a a message is received of the specified type.
		/// </summary>
		public interface All :
			IMessageConsumer<TMessage>
		{
		}

		/// <summary>
		/// Declares a Consume method for the message type TMessage wrapped in the 
		/// consume context
		/// </summary>
		public interface Context :
			IConsumer<TMessage>
		{
		}
	}
}