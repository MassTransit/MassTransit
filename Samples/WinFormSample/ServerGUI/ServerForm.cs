namespace ServerGUI
{
    using System;
    using System.ComponentModel;
    using System.Windows.Forms;
    using Castle.Windsor;
    using log4net;
    using MassTransit.ServiceBus;
    using MassTransit.ServiceBus.Subscriptions;
    using MassTransit.WindsorIntegration;

    public partial class ServerForm : Form
    {
        private static readonly ILog _log = LogManager.GetLogger(typeof (ServerForm));

        private IServiceBus _bus;

        private IWindsorContainer _container;

        public ServerForm()
        {
            InitializeComponent();
        }

        private void ServerForm_Load(object sender, EventArgs e)
        {
            StartService();
        }

        private void StartService()
        {
            StopService();

            try
            {
                _container = new DefaultMassTransitContainer("Server.Castle.xml");

                _bus = _container.Resolve<IServiceBus>("server");

                _bus.AddComponent<UserAgentSession>();
            }
            catch (Exception ex)
            {
                _log.Error(ex);
                MessageBox.Show("The server failed to start:\n\n" + ex.Message);
            }
        }


        private void StopService()
        {
            try
            {
                if (_bus != null)
                {
                    SubscriptionClient client = _container.Resolve<SubscriptionClient>("server.subscriptionClient");
                    if (client != null)
                    {
                        client.Stop();
                        _container.Release(client);
                    }

                    _container.Release(_bus);
                    _bus = null;
                }
            }
            catch (Exception ex)
            {
                _log.Error(ex);
            }

            try
            {
                if (_container != null)
                {
                    _container.Dispose();
                    _container = null;
                }
            }
            catch (Exception ex)
            {
                _log.Error(ex);
            }
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);

            StopService();

            e.Cancel = false;
        }
    }
}