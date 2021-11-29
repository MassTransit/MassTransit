namespace MassTransit.Configuration
{
    public class MediatorConfiguration :
        ReceivePipeDispatcherConfiguration,
        IMediatorConfigurator
    {
        readonly IHostConfiguration _hostConfiguration;

        public MediatorConfiguration(IHostConfiguration hostConfiguration, IReceiveEndpointConfiguration endpointConfiguration)
            : base(hostConfiguration, endpointConfiguration)
        {
            _hostConfiguration = hostConfiguration;

            if (_hostConfiguration.LogContext == null)
            {
                LogContext.ConfigureCurrentLogContext();

                _hostConfiguration.LogContext = LogContext.Current;
            }
        }

        public ConnectHandle ConnectConsumeObserver(IConsumeObserver observer)
        {
            return _hostConfiguration.ConnectConsumeObserver(observer);
        }

        public ConnectHandle ConnectSendObserver(ISendObserver observer)
        {
            return _hostConfiguration.ConnectSendObserver(observer);
        }

        public ConnectHandle ConnectPublishObserver(IPublishObserver observer)
        {
            return _hostConfiguration.ConnectPublishObserver(observer);
        }
    }
}
