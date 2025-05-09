namespace MassTransit.DapperIntegration.Tests.IntegrationTests.StateMachineSagas
{
    using System;


    public class VersionedSaga : ISagaVersion
    {
        public Guid CorrelationId { get; set; }
        public int Version { get; set; }
        public string Name { get; set; }
        public int Age { get; set; }
        public string PhoneNumber { get; set; }
        public string Zip_Code { get; set; }
    }
}
