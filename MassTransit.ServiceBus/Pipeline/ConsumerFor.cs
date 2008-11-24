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
namespace MassTransit.ServiceBus.Pipeline
{
	public class ConsumerFor<V> where V : class
	{
		public class Narrows<T> : Consumes<V>.All
			where T : class
		{
			private readonly Consumes<T>.All m_Consumer;

			private Narrows(Consumes<T>.All _Consumer)
			{
				m_Consumer = _Consumer;
			}

			public void Consume(V _Message)
			{
				m_Consumer.Consume(TranslateTo<T>.From(_Message));
			}

			public static Consumes<V>.All Wrap(Consumes<T>.All _Consumer)
			{
				return new Narrows<T>(_Consumer);
			}
		}

		public class Widens<T> : Consumes<V>.All
			where T : class, V
		{
			private readonly Consumes<T>.All m_Consumer;

			private Widens(Consumes<T>.All _Consumer)
			{
				m_Consumer = _Consumer;
			}

			public void Consume(V _Message)
			{
				m_Consumer.Consume(TranslateTo<T>.From(_Message));
			}

			public static Consumes<V>.All Wrap(Consumes<T>.All _Consumer)
			{
				return new Widens<T>(_Consumer);
			}
		}
	}
}