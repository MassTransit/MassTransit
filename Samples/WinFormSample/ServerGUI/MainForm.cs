namespace ServerGUI
{
    using System;
    using System.ComponentModel;
    using System.Windows.Forms;
    using Castle.Windsor;
    using log4net;
    using MassTransit.ServiceBus;
    using MassTransit.WindsorIntegration;
    using Messages;

    public partial class MainForm : Form
    {
        private static readonly ILog _log = LogManager.GetLogger(typeof (MainForm));

        private IServiceBus _bus;
        private IWindsorContainer _container;

        public MainForm()
        {
            InitializeComponent();

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
                    _bus.Dispose();
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