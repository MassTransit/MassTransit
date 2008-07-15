/// Copyright 2007-2008 The Apache Software Foundation.
/// 
/// Licensed under the Apache License, Version 2.0 (the "License"); you may not use 
/// this file except in compliance with the License. You may obtain a copy of the 
/// License at 
/// 
///   http://www.apache.org/licenses/LICENSE-2.0 
/// 
/// Unless required by applicable law or agreed to in writing, software distributed 
/// under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR 
/// CONDITIONS OF ANY KIND, either express or implied. See the License for the 
/// specific language governing permissions and limitations under the License.
namespace MassTransit.ServiceBus
{
    using log4net;

    public class Consumes<TMessage> where TMessage : class
	{
		private static readonly Selected _null;

		static Consumes()
		{
			_null = new NullConsumer();
		}

		public static Selected Null
		{
			get { return _null; }
		}

		public interface All
		{
			void Consume(TMessage message);
		}

		public interface For<TCorrelationId> : All, CorrelatedBy<TCorrelationId>
		{
		}

		public class NullConsumer : Selected
		{

            private static readonly ILog _log = LogManager.GetLogger(typeof(NullConsumer));

			public bool Accept(TMessage message)
			{
				return true;
			}

			public void Consume(TMessage message)
			{
                _log.Warn("NullConsumer consumed a message");
			}
		}

		public interface Selected : All
		{
			bool Accept(TMessage message);
		}
	}
}