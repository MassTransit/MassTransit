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
namespace MassTransit.Distributor
{
	/// <summary>
	/// These settings are used to configure a distributed consumer if the default
	/// settings are not sufficient.
	/// </summary>
	public class WorkerSettings
	{
		public WorkerSettings()
		{
			InProgressLimit = 4;
			PendingLimit = 16;
		}

		/// <summary>
		/// A recommended limit of pending messages for this worker. This is not a hard limit,
		/// the queue depth may at times exceed this value.
		/// </summary>
		public int PendingLimit { get; set; }

		/// <summary>
		/// A recommended limit of concurrent messages being consumed by the worker. This too is not a hard
		/// limit, just a recommendation that will most likely be enforced but not guaranteed.
		/// </summary>
		public int InProgressLimit { get; set; }
	}
}