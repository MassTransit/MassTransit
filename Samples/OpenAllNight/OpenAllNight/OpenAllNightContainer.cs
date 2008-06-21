namespace OpenAllNight
{
    using Castle.Windsor;
    using MassTransit.WindsorIntegration;

    public class OpenAllNightContainer :
        WindsorContainer
    {
        public OpenAllNightContainer(string xmlFile) : base(xmlFile)
        {
            Initialize();
        }

        private void Initialize()
        {
            this.AddFacility("masstransit", new MassTransitFacility());
        }
    }
}