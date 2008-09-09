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
    using System.Threading;
    using System.Windows.Forms;
    using Castle.Windsor;
    using log4net;
    using MassTransit.ServiceBus;
    using MassTransit.ServiceBus.Timeout.Messages;
    using MassTransit.ServiceBus.Util;
    using MassTransit.WindsorIntegration;
    using Messages;

    public partial class ClientForm : Form
    {
        private static readonly ILog _log = LogManager.GetLogger(typeof (ClientForm));

        private IServiceBus _bus;

        private IWindsorContainer _container;

        private BackgroundWorker[] _workers = new BackgroundWorker[3];
        private int _target;


        public ClientForm()
        {
            InitializeComponent();
        }

        private void ClientForm_Load(object sender, EventArgs e)
        {
            _workers[0] = new BackgroundWorker();
            _workers[0].DoWork += ClientForm_DoWork;
            _workers[1] = new BackgroundWorker();
            _workers[1].DoWork += ClientForm_DoWork;
            _workers[2] = new BackgroundWorker();
            _workers[2].DoWork += ClientForm_DoWork;

            StartClient();
        }

        private void ClientForm_DoWork(object sender, DoWorkEventArgs e)
        {
            ClientFormWorkerArgs args = e.Argument as ClientFormWorkerArgs;
            if (args == null)
                return;

            _target = 5000;

            TrapperKeeper tk = new TrapperKeeper(_target);

            _bus.Subscribe(tk);

            for (int i = 0; i < _target; i++)
            {
                SubmitQuestion question = new SubmitQuestion(CombGuid.NewCombGuid());
                tk.Add(question);

                if (i%10 == 0) //update every ten things
                    UpdateMessageCount(args.Client, i + 1, tk.Answered, tk.SendingElapsedMilliseconds, tk.ElapsedMilliseconds);

                _bus.Publish(question);

                if (args.WaitTime > 0)
                    Thread.Sleep(args.WaitTime);
            }
            tk.SendComplete();

            for (int i = 0; i < 3000; i++)
            {
                if (tk.Done.WaitOne(TimeSpan.FromSeconds(0.1), true))
                    break;

                UpdateMessageCount(args.Client, _target, tk.Answered, tk.SendingElapsedMilliseconds, tk.ElapsedMilliseconds);
            }

            UpdateMessageCount(args.Client, _target, tk.Answered, tk.SendingElapsedMilliseconds, tk.ElapsedMilliseconds);
        }

        private void UpdateMessageCount(int clientId, int numberOfMessagesSent, int numberOfMessagesReceived, long sentElapsedTime, long receiveElapsedTime)
        {
            if (client1Sent.InvokeRequired)
            {
                ThreadSafeUpdateMessageCount tsu = UpdateMessageCount;
                BeginInvoke(tsu, new object[] {clientId, numberOfMessagesSent, numberOfMessagesReceived, sentElapsedTime, receiveElapsedTime});
            }
            else
            {
                string sentText = string.Format("{0}/{1} ({2}/s)", numberOfMessagesSent, _target, receiveElapsedTime == 0 ? 0 : numberOfMessagesSent * 1000 / sentElapsedTime);
                string recvText = string.Format("{0}/{1} ({2}/s)", numberOfMessagesReceived, _target, receiveElapsedTime == 0 ? 0 : numberOfMessagesReceived*1000/receiveElapsedTime);
                switch (clientId)
                {
                    case 1:
                        client1Sent.Text = sentText;
                        client1Received.Text = recvText;
                        if (numberOfMessagesReceived == _target)
                            client1Active.Checked = false;
                        break;

                    case 2:
                        client2Sent.Text = sentText;
                        client2Received.Text = recvText;
                        if (numberOfMessagesReceived == _target)
                            client2Active.Checked = false;
                        break;

                    case 3:
                        client3Sent.Text = sentText;
                        client3Received.Text = recvText;
                        if (numberOfMessagesReceived == _target)
                            client3Active.Checked = false;
                        break;
                }
            }
        }

        private void StartClient()
        {
            StopClient();

            try
            {
                _container = new DefaultMassTransitContainer("Client.Castle.xml");

                _bus = _container.Resolve<IServiceBus>("client");
                _bus.AddComponent<TimeoutWatcher>();
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
                    _bus.Dispose();
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

        private void Client2Active_CheckedChanged(object sender, EventArgs e)
        {
            if (_workers[1].IsBusy)
                return;

            if (client2Active.Checked == false)
                return;

            _workers[1].RunWorkerAsync(new ClientFormWorkerArgs(2, int.Parse(client2WaitTime.Text)));
        }

        private void Client1Active_CheckedChanged(object sender, EventArgs e)
        {
            if (_workers[0].IsBusy)
                return;

            if (client1Active.Checked == false)
                return;

            _workers[0].RunWorkerAsync(new ClientFormWorkerArgs(1, int.Parse(client1WaitTime.Text)));
        }

        private void client3Active_CheckedChanged(object sender, EventArgs e)
        {
            if (_workers[2].IsBusy)
                return;

            if (client3Active.Checked == false)
                return;

            _workers[2].RunWorkerAsync(new ClientFormWorkerArgs(3, int.Parse(client3WaitTime.Text)));
        }

        private class ClientFormWorkerArgs
        {
            private readonly int _client;
            private readonly int _waitTime;

            public ClientFormWorkerArgs(int client, int waitTime)
            {
                _client = client;
                _waitTime = waitTime;
            }

            public int Client
            {
                get { return _client; }
            }

            public int WaitTime
            {
                get { return _waitTime; }
            }
        }

        private delegate void ThreadSafeUpdateMessageCount(int clientId, int numberOfMessagesSent, int numberOfMessagesReceived, long sendElapsed, long receivedElapsed);

		private void scheduleTimeout_Click(object sender, EventArgs e)
		{
			_bus.Publish(new ScheduleTimeout(Guid.NewGuid(), DateTime.Now + TimeSpan.FromSeconds(30)));
		}
    }
}