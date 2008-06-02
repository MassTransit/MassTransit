namespace WebRequestReply.UI.MonoRail.Controllers
{
    using Castle.MonoRail.Framework;

    public class DemoController :
        Controller
    {
        public void Default()
        {
            
        }

        public void Post(string requestText, string responseText)
        {
            this.DirectRender("DDS: " + requestText);
        }
    }
}