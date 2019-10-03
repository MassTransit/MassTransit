namespace MassTransit.Tests.Courier
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using MassTransit.Courier;
    using MassTransit.Courier.Contracts;
    using MassTransit.MessageData;
    using MassTransit.Testing;
    using NUnit.Framework;
    using TestFramework;
    using TestFramework.Courier;


    [TestFixture]
    public class Using_message_data_arguments :
        InMemoryActivityTestFixture
    {
        Task<ConsumeContext<RoutingSlipCompleted>> _completed;
        Task<ConsumeContext<RoutingSlipActivityCompleted>> _activityCompleted;
        Task<ConsumeContext<RoutingSlipFaulted>> _faulted;
        Guid _trackingNumber;
        readonly IMessageDataRepository _repository = new InMemoryMessageDataRepository();

        [OneTimeSetUp]
        public async Task Should_properly_load()
        {

            _completed = SubscribeHandler<RoutingSlipCompleted>();
            _faulted = SubscribeHandler<RoutingSlipFaulted>();
            _activityCompleted = SubscribeHandler<RoutingSlipActivityCompleted>();

            _trackingNumber = NewId.NextGuid();
            var builder = new RoutingSlipBuilder(_trackingNumber);
            builder.AddSubscription(Bus.Address, RoutingSlipEvents.All);

            ActivityTestContext testActivity = GetActivityContext<SetLargeVariableActivity>();
            builder.AddActivity(testActivity.Name, testActivity.ExecuteUri, new
            {
                Key = "Name",
                Value = await _repository.PutString("Frank"),
            });

            await Bus.Execute(builder.Build());

            await Task.WhenAny(_completed, _faulted);

            if(_faulted.IsCompleted)
                Console.WriteLine(string.Join(",", _faulted.Result.Message.ActivityExceptions.Select(x => x.ExceptionInfo.Message)));

            Assert.That(_completed.Status, Is.EqualTo(TaskStatus.RanToCompletion));
        }

        protected override void SetupActivities(BusTestHarness testHarness)
        {
            var context = AddActivityContext<SetLargeVariableActivity, SetLargeVariableArguments>(() => new SetLargeVariableActivity());

            context.OnConfigureExecuteReceiveEndpoint += configurator => configurator.UseMessageData(_repository);
        }

        [Test]
        public async Task Should_override_variables_in_the_routing_slip()
        {
            ConsumeContext<RoutingSlipCompleted> context = await _completed;

            Assert.AreEqual(_trackingNumber, context.Message.TrackingNumber);

            Assert.AreEqual("Frank", context.Message.GetVariable<string>("Name"));
        }

        [Test]
        public async Task Should_receive_the_routing_slip_activity_completed_event()
        {
            ConsumeContext<RoutingSlipActivityCompleted> context = await _activityCompleted;

            Assert.AreEqual(_trackingNumber, context.Message.TrackingNumber);
        }
    }
}
