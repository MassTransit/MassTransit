// Copyright 2007-2008 The Apache Software Foundation.
//  
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use 
// this file except in compliance with the License. You may obtain a copy of the 
// License at 
// 
//     http://www.apache.org/licenses/LICENSE-2.0 
// 
// Unless required by applicable law or agreed to in writing, software distributed 
// under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR 
// CONDITIONS OF ANY KIND, either express or implied. See the License for the 
// specific language governing permissions and limitations under the License.
namespace ClientGUI
{
    using System;
    using System.ComponentModel;
    using System.Windows.Forms;
    using Castle.Windsor;
    using log4net;
    using MassTransit.ServiceBus;
    using MassTransit.WindsorIntegration;

    public partial class ClientForm : Form
    {
        private static readonly ILog _log = LogManager.GetLogger(typeof (ClientForm));

        private IServiceBus _bus;

        private IWindsorContainer _container;

        public ClientForm()
        {
            InitializeComponent();
        }

        private void ClientForm_Load(object sender, EventArgs e)
        {
            StartClient();
        }

        private void StartClient()
        {
            StopClient();

            try
            {
                _container = new DefaultMassTransitContainer("Client.Castle.xml");

                _bus = _container.Resolve<IServiceBus>("client");
            }
            catch (Exception ex)
            {
                _log.Error(ex);
                MessageBox.Show("The client failed to start:\n\n" + ex.Message);
            }
        }

        private void StopClient()
        {
            try
            {
                if (_bus != null)
                {
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

            StopClient();

            e.Cancel = false;
        }
    }
}