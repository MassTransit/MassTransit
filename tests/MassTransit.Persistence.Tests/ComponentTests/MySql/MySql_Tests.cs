namespace MassTransit.Persistence.Tests.ComponentTests.MySql
{
    public class MySql_Tests
    {
        public class VersionedSaga : ISaga
        {
            public DateTime RowVersion { get; set; }
            public string Name { get; set; }
            public int Age { get; set; }
            public string PhoneNumber { get; set; }
            public string Zip_Code { get; set; }
            public Guid CorrelationId { get; set; }
        }
    }
}
