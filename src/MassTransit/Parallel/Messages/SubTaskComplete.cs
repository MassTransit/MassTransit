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
namespace MassTransit.Parallel.Messages
{
	using System;

	[Serializable]
	public class SubTaskComplete<TOutput>
		where TOutput : class
	{
		public SubTaskComplete(string address, int taskLimit, int activeTaskCount, Guid taskId, int subTaskId, TOutput output)
		{
			Address = address;
			ActiveTaskCount = activeTaskCount;
			TaskLimit = taskLimit;
			TaskId = taskId;
			SubTaskId = subTaskId;
			Output = output;
		}

		protected SubTaskComplete()
		{
		}

		public string Address { get; set; }

		public int TaskLimit { get; set; }

		public int ActiveTaskCount { get; set; }

		public TOutput Output { get; set; }

		public Guid TaskId { get; set; }

		public int SubTaskId { get; set; }
	}
}