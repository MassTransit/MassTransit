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
namespace MassTransit.Distributor.Messages
{
	/// <summary>
	/// Presents the workload of a worker to interested consumers
	/// </summary>
	/// <typeparam name="T">The type of message being handled</typeparam>
	public interface Workload<T>
	{
		/// <summary>
		/// The number of messages pending for consumption by this worker
		/// </summary>
		int Pending { get; }

		/// <summary>
		/// The maximum number of messages that should be pending for this worker in the queue
		/// </summary>
		int PendingLimit { get; }

		/// <summary>
		/// The number of messages currently being processed by this worker
		/// </summary>
		int InProgress { get; }

		/// <summary>
		/// The maximum number of messages that can be processed at a time by this worker
		/// </summary>
		int InProgressLimit { get; }
	}
}