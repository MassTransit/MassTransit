namespace BusinessWebService
{
    using Castle.Windsor;
    using MassTransit.WindsorIntegration;

    public class IoC
    {
        private static readonly IWindsorContainer _container;

        static IoC()
        {
            _container = new DefaultMassTransitContainer("castle.xml");
        }

        public static IWindsorContainer Container
        {
            get { return _container; }
        }
    }
}