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
	using System.Threading;
	using Sagas;

	public class GridServiceProposer
	{
		private long _nextBallotId;

		public void ProposeNextServiceNode(Type commandType, IServiceBus bus)
		{
			Guid commandId = GridService.GenerateIdForType(commandType);

			Prepare prepare = new Prepare
				{
					CorrelationId = commandId,
					BallotId = Interlocked.Increment(ref _nextBallotId),
				};

			bus.Publish(prepare);

			// now we wait to see if we get either a majority of acceptors, or a majority of rejectors

			Accept accept = new Accept
				{
					CorrelationId = commandId,
					BallotId = _nextBallotId,
					Value = bus.Endpoint.Uri.ToString(),
				};

			bus.Publish(accept);

			// now we wait for the acceptors to publish that they have accepted the value

			// once we have a majority, we consider the value updated and have a nice day
		}
	}
}