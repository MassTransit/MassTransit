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
namespace MassTransit.Internal
{
	using System;

	public class MessageContextBase
	{
		public Uri DestinationAddress { get; protected set; }
		public Uri ResponseAddress { get; protected set; }
		public Uri FaultAddress { get; protected set; }
		public Uri SourceAddress { get; protected set; }
		public int RetryCount { get; protected set; }
		public object Message { get; protected set; }
		public string MessageType { get; protected set; }

		public virtual void Clear()
		{
			RetryCount = 0;
			Message = null;
			DestinationAddress = null;
			SourceAddress = null;
			ResponseAddress = null;
			FaultAddress = null;
			MessageType = null;
		}
	}
}