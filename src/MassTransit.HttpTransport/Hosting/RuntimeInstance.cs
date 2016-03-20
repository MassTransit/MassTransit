namespace MassTransit.HttpTransport.Hosting
{
    using System;
    using MassTransit.Pipeline;
    using Microsoft.Owin.Hosting;


    public class RuntimeInstance : OwinHostInstance
    {
        readonly HttpHostSettings _settings;
        IDisposable _appInstance;

        public RuntimeInstance(HttpHostSettings settings)
        {
            _settings = settings;
        }

        public string Host { get; }
        public int Port { get; set; }
        public EventHandler<ShutdownEventArgs> HostShutdown { get; set; }

        public void Start(IPipe<ReceiveContext> receivePipe)
        {
            var options = new StartOptions();
            options.Urls.Add(_settings.Host);
            options.Port = _settings.Port;

            _appInstance = WebApp.Start(options, app =>
            {
                new HttpMassTransitStartUp(receivePipe).Configuration(app);
            });
        }

        public void Dispose()
        {
            if(_appInstance != null)
                _appInstance.Dispose();
        }
    }
}