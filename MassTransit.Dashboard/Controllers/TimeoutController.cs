namespace MassTransit.Dashboard.Controllers
{
    using Castle.MonoRail.Framework;
    using Services.Timeout;

    [Layout("default")]
    public class TimeoutController :
        SmartDispatcherController
    {
        private readonly RemoteTimeoutViewer _viewer;

        public TimeoutController(RemoteTimeoutViewer viewer)
        {
            _viewer = viewer;
        }

        public void View()
        {
            PropertyBag.Add("timeouts", _viewer.List);
        }
    }
}