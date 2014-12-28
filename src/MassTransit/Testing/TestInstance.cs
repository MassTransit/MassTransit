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
namespace MassTransit.Testing
{
	using System;
	using System.Threading.Tasks;


    public interface TestInstance :
		IDisposable
	{
		/// <summary>
		/// Messages that were received by any endpoint during the execution of the test
		/// </summary>
		IReceivedMessageList Received { get; }

		/// <summary>
		/// Messages that were send by any endpoint during the execution of the test
		/// </summary>
		ISentMessageList Sent { get; }

		/// <summary>
		/// Messages that were not received by any handler, consumer, or instance during the execution of the test
		/// </summary>
		IReceivedMessageList Skipped { get; }

		/// <summary>
		/// Messages that were published by an bus (does not mean they were actually sent, just published)
		/// </summary>
		IPublishedMessageList Published { get; }

		/// <summary>
		/// Execute the test actions
		/// </summary>
		void Execute();
	}
}