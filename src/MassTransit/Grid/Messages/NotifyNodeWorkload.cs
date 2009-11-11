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
namespace MassTransit.Grid.Messages
{
	using System;

	public class NotifyNodeWorkload :
		NotifyNodeMessageBase
	{
		public NotifyNodeWorkload(Uri controlUri, Uri dataUri, DateTime created, DateTime lastUpdated, int activeMessageCount, int waitingMessageCount)
			: base(controlUri, dataUri, created, lastUpdated)
		{
			ActiveMessageCount = activeMessageCount;
			WaitingMessageCount = waitingMessageCount;
		}

		public NotifyNodeWorkload(NotifyNodeMessageBase source)
			: base(source)
		{
		}

		protected NotifyNodeWorkload()
		{
		}

		public int ActiveMessageCount { get; set; }
		public int WaitingMessageCount { get; set; }
	}
}