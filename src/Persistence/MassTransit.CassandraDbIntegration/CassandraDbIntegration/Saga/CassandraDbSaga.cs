namespace MassTransit.CassandraDbIntegration.Saga
{
    public class CassandraDbSaga
    {
        public CassandraDbSaga()
        {
            EntityType = "SAGA";
        }

        public string CorrelationId { get; set; }

        public string EntityType { get; set; } = "SAGA";

        public int VersionNumber { get; set; }

        public string Properties { get; set; }
    }
}
