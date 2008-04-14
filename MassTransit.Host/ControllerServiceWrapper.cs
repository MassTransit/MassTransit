namespace MassTransit.Host
{
    using System.ServiceProcess;
    using log4net;

    public class ControllerServiceWrapper : ServiceBase
    {
        private static readonly ILog _log = LogManager.GetLogger(typeof (ControllerServiceWrapper));
        private readonly Controller _controller;

        public ControllerServiceWrapper(Controller controller)
        {
            _controller = controller;
        }

        protected override void OnStart(string[] args)
        {
            _log.Info("Received service start notification");

            _log.DebugFormat("Arguments: {0}", string.Join(",", args));
            _controller.Start();
        }

        protected override void OnStop()
        {
            _log.Info("Received service stop notification");
            _controller.Stop();
        }
    }
}