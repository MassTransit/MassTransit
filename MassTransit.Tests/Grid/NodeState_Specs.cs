namespace MassTransit.Tests.Grid
{
	using System;
	using Magnum.DateTimeExtensions;
	using NUnit.Framework;
	using TextFixtures;

	[TestFixture]
	public class NodeState_Specs
	{
		private NotifyNodeAvailable _notifyNodeAvailable;
		private NotifyNodeDown _notifyNodeDown;

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
		}

		[Test]
		public void The_node_should_be_available()
		{
			NodeState state = new NodeState();

			state.RaiseEvent(NodeState.NodeAvailable, _notifyNodeAvailable);

			Assert.AreEqual(NodeState.Available, state.CurrentState);
		}

		[Test]
		public void The_node_should_be_down()
		{
			NodeState state = new NodeState();

			state.RaiseEvent(NodeState.NodeDown, _notifyNodeDown);

			Assert.AreEqual(NodeState.Down, state.CurrentState);
		}

		[Test]
		public void The_node_should_transition_to_down()
		{
			NodeState state = new NodeState();

			state.RaiseEvent(NodeState.NodeAvailable, _notifyNodeAvailable);
			state.RaiseEvent(NodeState.NodeDown, _notifyNodeDown);

			Assert.AreEqual(NodeState.Down, state.CurrentState);
		}

		[Test]
		public void The_node_should_transition_to_up()
		{
			NodeState state = new NodeState();

			state.RaiseEvent(NodeState.NodeDown, _notifyNodeDown);
			state.RaiseEvent(NodeState.NodeAvailable, _notifyNodeAvailable);

			Assert.AreEqual(NodeState.Available, state.CurrentState);
		}

		[Test]
		public void The_node_should_recover_after_going_down_and_up()
		{
			NodeState state = new NodeState();

			state.RaiseEvent(NodeState.NodeAvailable, _notifyNodeAvailable);
			Assert.AreEqual(NodeState.Available, state.CurrentState);

			state.RaiseEvent(NodeState.NodeDown, _notifyNodeDown);
			Assert.AreEqual(NodeState.Down, state.CurrentState);

			state.RaiseEvent(NodeState.NodeAvailable, _notifyNodeAvailable);
			Assert.AreEqual(NodeState.Available, state.CurrentState);

		}
	}

	[TestFixture]
	public class The_NodeState_saga :
		LoopbackTestFixture
	{
		[Test]
		public void Should_be_subscribable()
		{
			SetupSagaRepository<NodeState>();

			LocalBus.Subscribe<NodeState>();
		}

	}
}