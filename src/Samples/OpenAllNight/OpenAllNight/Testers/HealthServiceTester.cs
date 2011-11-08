namespace OpenAllNight.Testers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using MassTransit;
    using MassTransit.Services.HealthMonitoring.Messages;

    public class HealthServiceTester :
        Consumes<HealthUpdate>.All
    {
        private Counter _counter;
        private IServiceBus _bus;
        private IList<HealthInformation> _healthInfos = new List<HealthInformation>();

        private Uri _testUri = new Uri("msmq://localhost/some_queue");
        private Guid _testGuid = Guid.NewGuid();

        public HealthServiceTester(Counter counter, IServiceBus bus)
        {
            _counter = counter;
            _bus = bus;
        }


        

        public void Test()
        {
            bool result = _healthInfos.Any(i => i.DataUri.Equals(_bus.Endpoint.Address.Uri));
            if (result)
                Console.WriteLine("Found myself in the health info");
            else
            {
                _bus.Publish(new HealthUpdateRequest());
                Console.WriteLine("Can't find myself. :(");
            }

            //should be true
        }

        public void Consume(HealthUpdate message)
        {
            _healthInfos = message.Information;
        }
    }
}