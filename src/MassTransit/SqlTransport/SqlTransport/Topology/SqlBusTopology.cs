namespace MassTransit.SqlTransport.Topology
{
    using Configuration;
    using Transports;


    public class SqlBusTopology :
        BusTopology,
        ISqlBusTopology
    {
        readonly ISqlTopologyConfiguration _configuration;

        public SqlBusTopology(ISqlHostConfiguration hostConfiguration, ISqlTopologyConfiguration configuration)
            : base(hostConfiguration, configuration)
        {
            _configuration = configuration;
        }

        ISqlPublishTopology ISqlBusTopology.PublishTopology => _configuration.Publish;
        ISqlSendTopology ISqlBusTopology.SendTopology => _configuration.Send;

        ISqlMessagePublishTopology<T> ISqlBusTopology.Publish<T>()
        {
            return _configuration.Publish.GetMessageTopology<T>();
        }

        ISqlMessageSendTopology<T> ISqlBusTopology.Send<T>()
        {
            return _configuration.Send.GetMessageTopology<T>();
        }
    }
}
