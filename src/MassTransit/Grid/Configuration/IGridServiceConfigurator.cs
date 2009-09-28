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
namespace MassTransit.Grid.Configuration
{
	/// <summary>
	/// Used to configure a service on the grid. A service is a message that is handled by 
	/// some component. The grid will create a intercepter that actually consumes the message. 
	/// The intercepter will update some statistics, notify the other grid nodes of the activity,
	/// and then pass the message to the actual component for processing. When it is done, the 
	/// grid will report back the activity to the rest of the grid.
	/// </summary>
	/// <typeparam name="TService"></typeparam>
	public interface IGridServiceConfigurator<TService>
		where TService : class
	{
		/// <summary>
		/// Use the component for processing the message for this service
		/// </summary>
		/// <typeparam name="TComponent"></typeparam>
		/// <returns></returns>
		IGridServiceConfigurator<TService> Use<TComponent>()
			where TComponent : Consumes<TService>.All;
	}
}