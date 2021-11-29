namespace MassTransit
{
    using System;
    using System.Collections.Generic;
    using Courier;


    public static class ActivityObserverConfigurationExtensions
    {
        /// <summary>
        /// Connect an activity observer that will be connected to all activity execute/compensate endpoints
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="observer"></param>
        /// <returns></returns>
        public static ConnectHandle ConnectActivityObserver(this IBusFactoryConfigurator configurator, IActivityObserver observer)
        {
            return new ActivityConfigurationObserver(configurator, observer);
        }


        class ActivityConfigurationObserver :
            IActivityConfigurationObserver,
            ConnectHandle
        {
            readonly List<ConnectHandle> _handles;
            readonly IActivityObserver _observer;
            bool _disposed;

            public ActivityConfigurationObserver(IActivityConfigurationObserverConnector configurator, IActivityObserver observer)
            {
                _observer = observer;
                _handles = new List<ConnectHandle>();

                var handle = configurator.ConnectActivityConfigurationObserver(this);
                _handles.Add(handle);
            }

            public void Dispose()
            {
                _disposed = true;

                for (var i = 0; i < _handles.Count; i++)
                    _handles[i].Dispose();

                _handles.Clear();
            }

            public void Disconnect()
            {
                _disposed = true;

                for (var i = 0; i < _handles.Count; i++)
                    _handles[i].Disconnect();

                _handles.Clear();
            }

            public void ActivityConfigured<TActivity, TArguments>(IExecuteActivityConfigurator<TActivity, TArguments> configurator, Uri compensateAddress)
                where TActivity : class, IExecuteActivity<TArguments>
                where TArguments : class
            {
                if (_disposed)
                    return;

                _handles.Add(configurator.ConnectActivityObserver(_observer));
            }

            public void ExecuteActivityConfigured<TActivity, TArguments>(IExecuteActivityConfigurator<TActivity, TArguments> configurator)
                where TActivity : class, IExecuteActivity<TArguments>
                where TArguments : class
            {
                if (_disposed)
                    return;

                _handles.Add(configurator.ConnectActivityObserver(_observer));
            }

            public void CompensateActivityConfigured<TActivity, TLog>(ICompensateActivityConfigurator<TActivity, TLog> configurator)
                where TActivity : class, ICompensateActivity<TLog>
                where TLog : class
            {
                if (_disposed)
                    return;

                _handles.Add(configurator.ConnectActivityObserver(_observer));
            }
        }
    }
}
