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
	using Sagas;

	public class GridServiceComponentInterceptor<TService, TComponent> :
		GridServiceInterceptor<TService>
		where TService : class, CorrelatedBy<Guid>
		where TComponent : Consumes<TService>.All
	{
		private static readonly ILog _log = LogManager.GetLogger(typeof (GridServiceComponentInterceptor<TService, TComponent>).ToFriendlyName());

		private readonly Func<TComponent> _getComponent;

		public GridServiceComponentInterceptor(IGridControl grid, Func<TComponent> getComponent)
			: base(grid)
		{
			_getComponent = getComponent;
		}

		public override void Consume(TService message)
		{
			if (!Grid.ConsumeMessage(ServiceId, message.CorrelationId))
				return;

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
			return Grid.AcceptMessage(ServiceId, message.CorrelationId);
		}
	}

	public static class ExtensionsForGridThings
	{
		private static Random _random;

		static ExtensionsForGridThings()
		{
			_random = new Random();
		}

		public static ServiceNode SelectNodeToUse(this IList<ServiceNode> nodes)
		{
			return nodes.OrderBy(x => _random.Next()).First();
		}

		public static IList<ServiceNode> SelectQuorum(this IEnumerable<ServiceNode> nodes, Uri controlUri, Uri dataUri)
		{
			var all = nodes.ToList();
			if (all.Count <= 3)
				return all;

			var me = all.Where(x => x.ControlUri == controlUri);
			var everyoneElse = nodes.Except(me).ToList();

			return everyoneElse.OrderBy(x => _random.Next()).Take(all.Count/2).Union(me).ToList();
		}
	}
}