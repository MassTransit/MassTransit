namespace OpenAllNight
{
    using System.IO;
    using Castle.Windsor;
    using Castle.Windsor.Configuration.Interpreters;
    using log4net.Config;

    class Program
    {
        static void Main(string[] args)
        {
            XmlConfigurator.ConfigureAndWatch(new FileInfo("log4net.xml"));
            WindsorContainer c = new WindsorContainer(new XmlInterpreter("castle.xml"));

            //start?
            //how to loop
            //and know when to exit
        }
    }
}
