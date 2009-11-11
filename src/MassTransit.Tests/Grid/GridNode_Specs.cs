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
		private Node _state;
		private IEndpoint _endpoint;

		[SetUp]
		public void Setup()
		{
			_notifyNodeAvailable = new NotifyNodeAvailable(new Uri("loopback://localhost/control"), new Uri("loopback://localhost/data"),DateTime.UtcNow, 5.Minutes().FromUtcNow());

			_notifyNodeDown = new NotifyNodeDown(new Uri("loopback://localhost/control"), new Uri("loopback://localhost/data"), DateTime.UtcNow, 5.Minutes().FromUtcNow());

			_endpoint = MockRepository.GenerateMock<IEndpoint>();

			_state = new Node(CombGuid.Generate());
			_state.Bus = MockRepository.GenerateMock<IServiceBus>();
			_state.Bus.Stub(x => x.Endpoint).Return(_endpoint);
		}

		[Test]
		public void The_node_should_be_available()
		{

			_state.RaiseEvent(Node.NodeAvailable, _notifyNodeAvailable);

			Assert.AreEqual(Node.Available, _state.CurrentState);
		}

		[Test]
		public void The_node_should_be_down()
		{
			_state.RaiseEvent(Node.NodeDown, _notifyNodeDown);

			Assert.AreEqual(Node.Down, _state.CurrentState);
		}

		[Test]
		public void The_node_should_transition_to_down()
		{
			_state.RaiseEvent(Node.NodeAvailable, _notifyNodeAvailable);
			_state.RaiseEvent(Node.NodeDown, _notifyNodeDown);

			Assert.AreEqual(Node.Down, _state.CurrentState);
		}

		[Test]
		public void The_node_should_transition_to_up()
		{
			_state.RaiseEvent(Node.NodeDown, _notifyNodeDown);
			_state.RaiseEvent(Node.NodeAvailable, _notifyNodeAvailable);

			Assert.AreEqual(Node.Available, _state.CurrentState);
		}

		[Test]
		public void The_node_should_recover_after_going_down_and_up()
		{
			_state.RaiseEvent(Node.NodeAvailable, _notifyNodeAvailable);
			Assert.AreEqual(Node.Available, _state.CurrentState);

			_state.RaiseEvent(Node.NodeDown, _notifyNodeDown);
			Assert.AreEqual(Node.Down, _state.CurrentState);

			_state.RaiseEvent(Node.NodeAvailable, _notifyNodeAvailable);
			Assert.AreEqual(Node.Available, _state.CurrentState);
		}
	}
}