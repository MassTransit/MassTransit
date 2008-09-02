namespace MassTransit.Host.Configurations
{
    using System.ServiceProcess;
    using LifeCycles;

    public class InteractiveConfiguration :
        BaseWinServiceConfiguration
    {

        public override IApplicationLifeCycle LifeCycle
        {
            get { throw new System.NotImplementedException(); }
        }

        public override Credentials Credentials
        {
            get { throw new System.NotImplementedException(); }
        }

        public override string ServiceName
        {
            get { throw new System.NotImplementedException(); }
        }

        public override string DispalyName
        {
            get { throw new System.NotImplementedException(); }
        }

        public override string Description
        {
            get { throw new System.NotImplementedException(); }
        }

        public override string[] Dependencies
        {
            get { throw new System.NotImplementedException(); }
        }


        public override void ConfigureServiceProcessInstaller(ServiceProcessInstaller installer)
        {
            installer.Username = null;
            installer.Password = null;
            installer.Account = ServiceAccount.User;
        }
    }
}