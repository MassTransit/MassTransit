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
namespace MassTransit.Grid.Paxos
{
	using System;

	[Serializable]
	public class PaxosMessageBase :
		CorrelatedBy<Guid>
	{
		/// <summary>
		/// The proposal number for this prepare request
		/// </summary>
		public long BallotId { get; set; }

		/// <summary>
		/// The key for this value, unique for each thing being synchronized
		/// </summary>
		public Guid CorrelationId { get; set; }
	}
}