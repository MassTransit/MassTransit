namespace MassTransit.Azure.Table.Saga
{
    using MassTransit.Saga;
    using Microsoft.Azure.Cosmos.Table;


    public interface ICloudTableProvider<in TSaga>
        where TSaga : class, ISaga
    {
        CloudTable GetCloudTable();
    }
}
