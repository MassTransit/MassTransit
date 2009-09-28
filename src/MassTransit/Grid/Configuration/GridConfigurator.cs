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
	using System;
	using Internal;
	using Saga;
	using Sagas;

	public class GridConfigurator :
		IGridConfigurator
	{
		private Action<IServiceBus, IGridControl, IObjectBuilder> _startActions = (x, y, z) => { };
		public bool Proposer { get; private set; }

		public Type ServiceType
		{
			get { return typeof (IGrid); }
		}

		public IBusService Create(IServiceBus bus, IObjectBuilder builder)
		{
			ServiceGrid grid = new ServiceGrid(builder.GetInstance<IEndpointFactory>(),
				builder.GetInstance<ISagaRepository<GridNode>>(),
				builder.GetInstance<ISagaRepository<GridService>>(),
				builder.GetInstance<ISagaRepository<GridServiceNode>>(),
				builder.GetInstance<ISagaRepository<GridMessageNode>>());

			grid.WhenStarted = () => _startActions(bus, grid, builder);

			if(Proposer)
				grid.ProposerUri = bus.ControlBus.Endpoint.Uri;

			return grid;
		}

		public IGridServiceConfigurator<TMessage> For<TMessage>()
			where TMessage : class, CorrelatedBy<Guid>
		{
			var configurator = new GridServiceConfigurator<TMessage>();

			_startActions += (bus, grid, builder) => configurator.Start(bus, grid, builder);

			return configurator;
		}

		public void SetProposer()
		{
			Proposer = true;
		}
	}
}