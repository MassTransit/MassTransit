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
	using System.Collections.Generic;
	using System.Linq;
	using log4net;
	using Saga;
	using Sagas;

	public class GridServiceComponentInterceptor<TService, TComponent> :
		GridServiceInterceptor<TService>
		where TService : class, CorrelatedBy<Guid>
		where TComponent : Consumes<TService>.All
	{
		private static readonly ILog _log = LogManager.GetLogger(typeof (GridServiceComponentInterceptor<TService, TComponent>).ToFriendlyName());

		private readonly ISagaRepository<GridMessageNode> _messageNodeRepository;
		private readonly ISagaRepository<GridServiceNode> _serviceNodeRepository;
		private readonly Func<TComponent> _getComponent;

		public GridServiceComponentInterceptor(IGridControl grid, 
			ISagaRepository<GridMessageNode> messageNodeRepository, 
			ISagaRepository<GridServiceNode> serviceNodeRepository,
			Func<TComponent> getComponent)
			: base(grid)
		{
			_messageNodeRepository = messageNodeRepository;
			_serviceNodeRepository = serviceNodeRepository;
			_getComponent = getComponent;
		}

		public override void Consume(TService message)
		{
			var messageNode = Grid.GetMessageNode(message.CorrelationId);
			if (messageNode == null)
			{
				_log.ErrorFormat("GRID Consume received unknown message: {0}/{1}", message.CorrelationId, typeof(TService).FullName);
				return;
			}

			if (messageNode.CurrentState == GridMessageNode.Completed)
			{
				_log.DebugFormat("GRID: {0}/{1} discarded, already complete", message.CorrelationId, typeof(TService).FullName);
				return;
			}

			if(!Grid.IsAssignedToMessage(messageNode))
			{
				_log.WarnFormat("GRID: {0}/{1} discarded, should have not been assigned", message.CorrelationId, typeof(TService).FullName);
				return;
			}

			RemoveActiveInterceptor removeActiveInterceptor = NotifyGridOfActiveInterceptor(message.CorrelationId);

			try
			{
				var component = _getComponent();
				component.Consume(message);

				Grid.NotifyMessageComplete(message.CorrelationId);
			}
			finally
			{
				removeActiveInterceptor();
			}
		}

		public override bool Accept(TService message)
		{
			var messageNode = Grid.GetMessageNode(message.CorrelationId);
			if(messageNode == null)
			{
				Grid.ProposeMessageNodeToQuorum(ServiceId, message.CorrelationId);
				return false;
			}

			if (messageNode.CurrentState == GridMessageNode.Completed)
				return true;

			return Grid.IsAssignedToMessage(messageNode);
		}
	}

	public static class ExtensionsForGridThings
	{
		private static Random _random;

		static ExtensionsForGridThings()
		{
			_random = new Random();
		}

		public static IList<GridServiceNode> SelectQuorum(this IEnumerable<GridServiceNode> nodes)
		{
			var all = nodes.ToList();

			if (all.Count <= 2)
				return all;

			int required = all.Count/2 + 1;

			return all.OrderBy(x => _random.Next()).Take(required).ToList();
		}
	}
}