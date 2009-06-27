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
namespace MassTransit.Grid
{
	using System;
	using Sagas;

	public delegate void RemoveActiveInterceptor();

	public interface IGridControl :
		IGrid
	{
		void RegisterServiceInterceptor<TService>(GridServiceInterceptor<TService> interceptor)
			where TService : class;

		RemoveActiveInterceptor AddActiveInterceptor(Guid serviceId, Guid correlationId, IGridServiceInteceptor interceptor);


		/// <summary>
		/// Indicates whether this grid node should accept the message from the endpoint
		/// </summary>
		/// <param name="serviceId"></param>
		/// <param name="correlationId"></param>
		/// <returns></returns>
		bool AcceptMessage(Guid serviceId, Guid correlationId);

		/// <summary>
		/// Indicates whether this grid node should consume the message
		/// </summary>
		/// <param name="serviceId"></param>
		/// <param name="correlationId"></param>
		/// <returns></returns>
		bool ConsumeMessage(Guid serviceId, Guid correlationId);

		/// <summary>
		/// Notifies the grid that a message was consumed. The grid will then update the GridMessageNode saga instance
		/// to reflect the completion of the message processing.
		/// </summary>
		/// <param name="correlationId"></param>
		void NotifyMessageComplete(Guid correlationId);
	}
}