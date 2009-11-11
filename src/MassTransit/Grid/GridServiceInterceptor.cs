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

	public interface IGridServiceInteceptor
	{
	}


	public abstract class GridServiceInterceptor<TService> :
		Consumes<TService>.Selected,
		IGridServiceInteceptor
		where TService : class
	{
		static GridServiceInterceptor()
		{
			ServiceId = typeof (TService).ToServiceTypeId();
		}

		protected GridServiceInterceptor(IGridControl grid)
		{
			Grid = grid;
		}

		public static Guid ServiceId { get; private set; }

		protected IGridControl Grid { get; set; }

		public abstract void Consume(TService message);

		public abstract bool Accept(TService message);

		protected RemoveActiveInterceptor NotifyGridOfActiveInterceptor(Guid correlationId)
		{
			return Grid.AddActiveInterceptor(ServiceId, correlationId, this);
		}
	}
}