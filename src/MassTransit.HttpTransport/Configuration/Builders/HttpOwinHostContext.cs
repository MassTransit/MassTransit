namespace MassTransit.HttpTransport.Configuration.Builders
{
    using System;
    using System.Threading.Tasks;
    using Context;
    using Hosting;
    using Pipeline;
    using Util;


    public class HttpOwinHostContext :
        BasePipeContext,
        OwinHostContext,
        IDisposable
    {
        public HttpOwinHostContext(OwinHostInstance host, HttpHostSettings settings, ITaskSupervisor supervisor)
            : this(host, settings, supervisor.CreateParticipant($"{TypeMetadataCache<HttpOwinHostContext>.ShortName} - {settings.ToDebugString()}"))
        {
        }

        HttpOwinHostContext(OwinHostInstance host, HttpHostSettings settings, ITaskParticipant participant)
            : base(new PayloadCache(), participant.StoppedToken)
        {
            HostSettings = settings;
            Instance = host;
        }

        public HttpHostSettings HostSettings { get; set; }
        public OwinHostInstance Instance { get; }
        public void Dispose()
        {
            Instance.Dispose();
        }
    }
}