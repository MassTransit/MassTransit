namespace MassTransit
{
    using System;


    public class CassandraDbSagaRepositoryOptions<TSaga>
        where TSaga : class, ISaga
    {
        public CassandraDbSagaRepositoryOptions()
        {
        }

        public string FormatSagaKey(Guid correlationId)
        {
            return correlationId.ToString();
        }
    }
}
