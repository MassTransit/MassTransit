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
namespace MassTransit.Tests.Grid
{
	using System;
	using System.Security.Cryptography;
	using System.Text;
	using System.Threading;
	using log4net;
	using Magnum.StateMachine;
	using MassTransit.Saga;

	public class ServiceGrid :
		Grid,
		Consumes<NotifyNewNodeAvailable>.All
	{
		private static readonly MD5CryptoServiceProvider _cryptoProvider = new MD5CryptoServiceProvider();

		private static readonly ILog _log = LogManager.GetLogger(typeof (ServiceGrid));
		private readonly IEndpointFactory _endpointFactory;
		private readonly ISagaRepository<NodeState> _nodeStateRepository;
		private IServiceBus _bus;
		private IServiceBus _controlBus;
		private DateTime _created;
		private UnsubscribeAction _unsubscribeAction;


		public ServiceGrid(IEndpointFactory endpointFactory, ISagaRepository<NodeState> nodeStateRepository)
		{
			_endpointFactory = endpointFactory;
			_nodeStateRepository = nodeStateRepository;
		}

		public void Consume(NotifyNewNodeAvailable message)
		{
			if (message.ControlEndpointUri != _controlBus.Endpoint.Uri)
			{
				_log.DebugFormat("{0} sending node available response to {1}", _controlBus.Endpoint.Uri, message.ControlEndpointUri);

				_endpointFactory.GetEndpoint(message.ControlEndpointUri).Send(NewNotifyNodeAvailableMessage());
			}
		}


		public bool IsHealthy
		{
			get { throw new NotImplementedException(); }
		}

		public void Start(IServiceBus bus)
		{
			_bus = bus;
			_controlBus = bus.ControlBus;

			_created = DateTime.UtcNow;


			_unsubscribeAction = _controlBus.Subscribe(this);
			_unsubscribeAction += _controlBus.Subscribe<NodeState>();


			NotifyAvailable();
		}

		public void Execute<T>(T command)
		{
			// begin saga for this command

			// publish message

			// get message from worker that is processing this command

			// get completion message for this command
		}

		public void ConfigureService<T>(Action<IServiceGridConfigurator<T>> configurator)
			where T : class
		{
			_unsubscribeAction += _bus.Subscribe<T>();

			configurator(new ServiceGridConfigurator<T>(this));
		}

		public void AddCommand<T>()
		{
			Guid commandId = GenerateCommandIdFromType(typeof (T));

			var message = new AddGridCommandService(commandId, _controlBus.Endpoint.Uri, _bus.Endpoint.Uri);
			_controlBus.Publish(message);
		}

		private void NotifyAvailable()
		{
			_controlBus.Publish(NewNotifyNodeAvailableMessage());
		}

		private NotifyNodeAvailable NewNotifyNodeAvailableMessage()
		{
			return new NotifyNodeAvailable
				{
					ControlEndpointUri = _controlBus.Endpoint.Uri,
					DataEndpointUri = _bus.Endpoint.Uri,
					LastUpdated = DateTime.UtcNow,
					Created = _created,
				};
		}

		public static Guid GenerateCommandIdFromType(Type type)
		{
			string key = type.AssemblyQualifiedName;

			var bytes = Encoding.UTF8.GetBytes(key);

			var hash = _cryptoProvider.ComputeHash(bytes);

			return new Guid(hash);
		}

		public void Stop()
		{
			_unsubscribeAction();
		}
	}

	public class AddGridCommandService
	{
		public AddGridCommandService(Guid commandId, Uri controlUri, Uri dataUri)
		{
			throw new NotImplementedException();
		}
	}

	public class ServiceGridConfigurator<T> :
		IServiceGridConfigurator<T>
	{
		private readonly Grid _grid;

		public ServiceGridConfigurator(Grid grid)
		{
			_grid = grid;
		}

		public void ForCommand<TMessage>()
		{
			_grid.AddCommand<TMessage>();
		}
	}

	public interface IServiceGridConfigurator<T>
	{
		void ForCommand<TMessage>();
	}


	public class GridServiceProposer
	{
		private long _nextBallotId;

		public void ProposeNextServiceNode(Type commandType, IServiceBus bus)
		{
			Guid commandId = ServiceGrid.GenerateCommandIdFromType(commandType);

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




	public class GridServiceSaga :
		SagaStateMachine<GridServiceSaga>,
		ISaga
	{
		static GridServiceSaga()
		{
			Define(() =>
				{
					Initially(
						When(PrepareToChange)
							.Then((saga, message) => { })
							.TransitionTo(Active));

				});
		}


		public static State Initial { get; set; }
		public static State Completed { get; set; }
		public static State Active { get; set; }

		public static Event<Prepare> PrepareToChange { get; set; }
		public static Event<Promise> PromiseToChange { get; set; }
		public static Event<Accept> AcceptChange { get; set; }
		public static Event<Accepted> AcceptedChange { get; set; }


		public Guid CorrelationId { get; set; }
		public IServiceBus Bus { get; set; }
	}


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

	/// <summary>
	/// The prepare is phase 1a of the Paxos algorithm
	/// </summary>
	[Serializable]
	public class Prepare :
		PaxosMessageBase
	{
	}

	[Serializable]
	public class PaxosValueMessageBase : 
		PaxosMessageBase
	{
		/// <summary>
		/// The number that established with the current value
		/// </summary>
		public long ValueBallotId { get; set; }

		/// <summary>
		/// The last assigned value associated with this key
		/// </summary>
		public string Value { get; set; }
	}

	/// <summary>
	/// When a prepare isn't accepted by an acceptor, this is sent to the proposer
	/// </summary>
	[Serializable]
	public class PrepareRejected :
		PaxosValueMessageBase
	{
	}

	/// <summary>
	/// The promise is phase 1b of the Paxos algorithm
	/// </summary>
	[Serializable]
	public class Promise :
		PaxosValueMessageBase
	{
	}

	/// <summary>
	/// The accept is the phase 2a of Paxos
	/// </summary>
	[Serializable]
	public class Accept :
		PaxosMessageBase
	{
		/// <summary>
		/// The value proposed for this key
		/// </summary>
		public string Value { get; set; }
	}

	/// <summary>
	/// Accepted is phase 2b of the Paxos algorithm
	/// </summary>
	[Serializable]
	public class Accepted :
		PaxosValueMessageBase
	{
		/// <summary>
		/// True if this is the final value (TODO is this right?)
		/// </summary>
		public bool IsFinal { get; set; }
	}
}