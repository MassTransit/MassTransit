namespace MassTransit.Host.Configurations
{
    using System.Configuration;
    using System.ServiceProcess;
    using LifeCycles;

    public class DotNetConfiguration :
        BaseWinServiceConfiguration
    {

        public override IApplicationLifeCycle LifeCycle
        {
            get { throw new System.NotImplementedException(); }
        }

        public override Credentials Credentials
        {
            get { return new Credentials(
                ConfigurationManager.AppSettings["username"],
                ConfigurationManager.AppSettings["password"]); }
        }

        public override string ServiceName
        {
            get { return ConfigurationManager.AppSettings["serviceName"]; }
        }

        public override string DispalyName
        {
            get { return ConfigurationManager.AppSettings["displayName"]; }
        }

        public override string Description
        {
            get { return ConfigurationManager.AppSettings["description"]; }
        }

        public override string[] Dependencies
        {
            get { return ConfigurationManager.AppSettings["dependencies"].Split(','); }
        }

        public override void ConfigureServiceProcessInstaller(ServiceProcessInstaller installer)
        {
            installer.Account = ServiceAccount.User;
            installer.Username = Credentials.Username;
            installer.Password = Credentials.Password;
        }
    }
}