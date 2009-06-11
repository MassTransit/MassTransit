namespace MassTransit.Tests.Grid
{
	using System;
	using Magnum.DateTimeExtensions;
	using NUnit.Framework;
	using Rhino.Mocks;

	[TestFixture]
	public class NodeState_Specs
	{
		private NotifyNodeAvailable _notifyNodeAvailable;
		private NotifyNodeDown _notifyNodeDown;
		private NodeState _state;
		private IEndpoint _endpoint;

		[SetUp]
		public void Setup()
		{
			_notifyNodeAvailable = new NotifyNodeAvailable
				{
					ControlEndpointUri = new Uri("loopback://localhost/data"),
					DataEndpointUri = new Uri("loopback://localhost/control"),
					LastUpdated = 5.Minutes().FromUtcNow(),
				};

			_notifyNodeDown = new NotifyNodeDown
				{
					ControlEndpointUri = new Uri("loopback://localhost/data"),
					DataEndpointUri = new Uri("loopback://localhost/control"),
					LastUpdated = 5.Minutes().FromUtcNow(),
				};

			_endpoint = MockRepository.GenerateMock<IEndpoint>();

			_state = new NodeState();
			_state.Bus = MockRepository.GenerateMock<IServiceBus>();
			_state.Bus.Stub(x => x.Endpoint).Return(_endpoint);
		}

		[Test]
		public void The_node_should_be_available()
		{

			_state.RaiseEvent(NodeState.NodeAvailable, _notifyNodeAvailable);

			Assert.AreEqual(NodeState.Available, _state.CurrentState);
		}

		[Test]
		public void The_node_should_be_down()
		{
			_state.RaiseEvent(NodeState.NodeDown, _notifyNodeDown);

			Assert.AreEqual(NodeState.Down, _state.CurrentState);
		}

		[Test]
		public void The_node_should_transition_to_down()
		{
			_state.RaiseEvent(NodeState.NodeAvailable, _notifyNodeAvailable);
			_state.RaiseEvent(NodeState.NodeDown, _notifyNodeDown);

			Assert.AreEqual(NodeState.Down, _state.CurrentState);
		}

		[Test]
		public void The_node_should_transition_to_up()
		{
			_state.RaiseEvent(NodeState.NodeDown, _notifyNodeDown);
			_state.RaiseEvent(NodeState.NodeAvailable, _notifyNodeAvailable);

			Assert.AreEqual(NodeState.Available, _state.CurrentState);
		}

		[Test]
		public void The_node_should_recover_after_going_down_and_up()
		{
			_state.RaiseEvent(NodeState.NodeAvailable, _notifyNodeAvailable);
			Assert.AreEqual(NodeState.Available, _state.CurrentState);

			_state.RaiseEvent(NodeState.NodeDown, _notifyNodeDown);
			Assert.AreEqual(NodeState.Down, _state.CurrentState);

			_state.RaiseEvent(NodeState.NodeAvailable, _notifyNodeAvailable);
			Assert.AreEqual(NodeState.Available, _state.CurrentState);
		}
	}
}