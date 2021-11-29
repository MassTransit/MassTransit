namespace MassTransit.AzureTable
{
    using Microsoft.Azure.Cosmos.Table;


    public interface ICloudTableProvider<in TSaga>
        where TSaga : class, ISaga
    {
        CloudTable GetCloudTable();
    }
}
