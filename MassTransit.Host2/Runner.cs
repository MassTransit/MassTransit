namespace MassTransit.Host2
{
    using System.Collections.Generic;
    using Actions;
    using log4net;

    public static class Runner
    {
        private static ILog _log = LogManager.GetLogger(typeof (Runner));
        private static IDictionary<string, IAction> _actions = new Dictionary<string, IAction>();

        static Runner()
        {
            _actions.Add("install", new InstallServiceAction());
            _actions.Add("uninstall", new UninstallServiceAction());
            _actions.Add("console", new RunAsConsoleAction());
            _actions.Add("service", new RunAsServiceAction());
        }

        public static void Run(HostedEnvironment environment, params string[] args)
        {
            //args parsing
            string action = args[0];

            _log.DebugFormat("Running action: {0}", action);

            _actions[action].Do(environment, args);
        } 

        
    }
}