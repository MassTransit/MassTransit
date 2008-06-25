namespace OpenAllNight
{
    using Castle.Core;
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
            this.AddComponentLifeStyle("counter", typeof (Counter), LifestyleType.Singleton);
            this.AddComponentLifeStyle("rvaoeuaoe", typeof (CacheUpdateResponseHandler), LifestyleType.Transient);
        }
    }
}