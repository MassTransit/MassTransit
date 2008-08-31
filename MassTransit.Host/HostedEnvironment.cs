namespace MassTransit.Host
{
    using Castle.Facilities.FactorySupport;
    using Castle.Facilities.Startable;
    using Castle.Windsor;
    using WindsorIntegration;

    public abstract class HostedEnvironment
    {
        private readonly string _xmlFile;


        protected HostedEnvironment(string xmlFile)
        {
            _xmlFile = xmlFile;
        }

        public abstract string ServiceName { get; }
        public abstract string DispalyName { get; }
        public abstract string Description { get; }

        //provides a way to supply username and password at config time
        public virtual string Username
        {
            get { return ""; }
        }

        public virtual string Password
        {
            get { return ""; }
        }

        public virtual string[] Dependecies
        {
            get { return new[] {"MSMQ"}; }
        }

        public virtual bool AskForCredentialsDuringInstall
        {
            get { return InstallAsLocalSystem == false && string.IsNullOrEmpty(Password); }
        }

        //how to configure?
        public virtual bool InstallAsLocalSystem
        {
            get { return false; }
        }

        public string XmlFile
        {
            get { return _xmlFile; }
        }

        public abstract HostedLifeCycle LifeCycle { get; }

        #region Nested type: Handler

        internal delegate void Handler();

        #endregion
    }
}