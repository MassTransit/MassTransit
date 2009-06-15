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
	using System.Linq;
	using log4net;
	using Saga;
	using Sagas;

	public class GridServiceComponentInterceptor<TService, TComponent> :
		GridServiceInterceptor<TService>
		where TService : class, CorrelatedBy<Guid>
		where TComponent : Consumes<TService>.All
	{
		private readonly Func<TComponent> _getComponent;

		public GridServiceComponentInterceptor(IGridControl grid, Func<TComponent> getComponent)
			: base(grid)
		{
			_getComponent = getComponent;
		}

		public override void Consume(TService message)
		{
			RemoveActiveInterceptor removeActiveInterceptor = NotifyGridOfActiveInterceptor(message.CorrelationId);

			try
			{
				var component = _getComponent();
				component.Consume(message);
			}
			finally
			{
				removeActiveInterceptor();
			}
		}

		public override bool Accept(TService message)
		{
			return true;
		}
	}

	public class GridXServiceComponentInterceptor<TService, TComponent> :
		GridServiceInterceptor<TService>
		where TService : class, CorrelatedBy<Guid>
		where TComponent : Consumes<TService>.Selected
	{
		private static readonly ILog _log = LogManager.GetLogger(typeof (GridXServiceComponentInterceptor<TService, TComponent>).ToFriendlyName());

		private readonly ISagaRepository<GridMessageNode> _messageNodeRepository;
		private readonly Func<TComponent> _getComponent;

		public GridXServiceComponentInterceptor(IGridControl grid, ISagaRepository<GridMessageNode> messageNodeRepository, Func<TComponent> getComponent)
			: base(grid)
		{
			_messageNodeRepository = messageNodeRepository;
			_getComponent = getComponent;
		}

		public override void Consume(TService message)
		{
			var messageNode = GetMessageNode(message.CorrelationId);
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

		private GridMessageNode GetMessageNode(Guid correlationId)
		{
			 return _messageNodeRepository.Where(x => x.CorrelationId == correlationId).FirstOrDefault();
		}

		public override bool Accept(TService message)
		{
			var messageNode = GetMessageNode(message.CorrelationId);
			if(messageNode == null)
			{
				Grid.NotifyNewMessage(message.CorrelationId);
				return false;
			}

			if (messageNode.CurrentState == GridMessageNode.Completed)
				return true;

			if(messageNode.CurrentState == GridMessageNode.WaitingForCompletion && 
				messageNode.DataUri == CurrentMessage.Headers.Bus.Endpoint.Uri && 
				messageNode.ControlUri == CurrentMessage.Headers.Bus.ControlBus.Endpoint.Uri)
			{
				return true;
			}

			return false;
		}
	}
}