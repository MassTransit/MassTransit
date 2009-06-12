namespace MassTransit.Tests.Grid
{
	using System;
	using Magnum;
	using Magnum.DateTimeExtensions;
	using MassTransit.Grid.Messages;
	using MassTransit.Grid.Sagas;
	using NUnit.Framework;
	using Rhino.Mocks;

	[TestFixture]
	public class When_tracking_the_state_of_a_GridNode
	{
		private NotifyNodeAvailable _notifyNodeAvailable;
		private NotifyNodeDown _notifyNodeDown;
		private GridNode _state;
		private IEndpoint _endpoint;

		[SetUp]
		public void Setup()
		{
			_notifyNodeAvailable = new NotifyNodeAvailable
				{
					ControlUri = new Uri("loopback://localhost/data"),
					DataUri = new Uri("loopback://localhost/control"),
					LastUpdated = 5.Minutes().FromUtcNow(),
				};

			_notifyNodeDown = new NotifyNodeDown
				{
					ControlUri = new Uri("loopback://localhost/data"),
					DataUri = new Uri("loopback://localhost/control"),
					LastUpdated = 5.Minutes().FromUtcNow(),
				};

			_endpoint = MockRepository.GenerateMock<IEndpoint>();

			_state = new GridNode(CombGuid.Generate());
			_state.Bus = MockRepository.GenerateMock<IServiceBus>();
			_state.Bus.Stub(x => x.Endpoint).Return(_endpoint);
		}

		[Test]
		public void The_node_should_be_available()
		{

			_state.RaiseEvent(GridNode.NodeAvailable, _notifyNodeAvailable);

			Assert.AreEqual(GridNode.Available, _state.CurrentState);
		}

		[Test]
		public void The_node_should_be_down()
		{
			_state.RaiseEvent(GridNode.NodeDown, _notifyNodeDown);

			Assert.AreEqual(GridNode.Down, _state.CurrentState);
		}

		[Test]
		public void The_node_should_transition_to_down()
		{
			_state.RaiseEvent(GridNode.NodeAvailable, _notifyNodeAvailable);
			_state.RaiseEvent(GridNode.NodeDown, _notifyNodeDown);

			Assert.AreEqual(GridNode.Down, _state.CurrentState);
		}

		[Test]
		public void The_node_should_transition_to_up()
		{
			_state.RaiseEvent(GridNode.NodeDown, _notifyNodeDown);
			_state.RaiseEvent(GridNode.NodeAvailable, _notifyNodeAvailable);

			Assert.AreEqual(GridNode.Available, _state.CurrentState);
		}

		[Test]
		public void The_node_should_recover_after_going_down_and_up()
		{
			_state.RaiseEvent(GridNode.NodeAvailable, _notifyNodeAvailable);
			Assert.AreEqual(GridNode.Available, _state.CurrentState);

			_state.RaiseEvent(GridNode.NodeDown, _notifyNodeDown);
			Assert.AreEqual(GridNode.Down, _state.CurrentState);

			_state.RaiseEvent(GridNode.NodeAvailable, _notifyNodeAvailable);
			Assert.AreEqual(GridNode.Available, _state.CurrentState);
		}
	}
}