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
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Threading;
    using System.Windows.Forms;
    using Castle.Windsor;
    using log4net;
    using MassTransit.ServiceBus;
    using MassTransit.ServiceBus.Util;
    using MassTransit.WindsorIntegration;
    using Messages;

    public partial class ClientForm : Form
    {
        private static readonly ILog _log = LogManager.GetLogger(typeof (ClientForm));

        private IServiceBus _bus;

        private IWindsorContainer _container;

        private BackgroundWorker[] _workers = new BackgroundWorker[2];

        private delegate void ThreadSafeUpdateMessageCount(int client, int sent, int received);


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

            StartClient();
        }

        private class TrapperKeeper :
            Consumes<QuestionAnswered>.Selected
        {
            private readonly int _target;
            private Dictionary<Guid, SubmitQuestion> _questions = new Dictionary<Guid, SubmitQuestion>();
            private int _answered;
            private readonly ManualResetEvent _done = new ManualResetEvent(false);

            public ManualResetEvent Done
            {
                get { return _done; }
            }

            public TrapperKeeper(int target)
            {
                _target = target;
            }

            public int Answered
            {
                get { return _answered; }
            }

            public void Consume(QuestionAnswered message)
            {
                if (Interlocked.Increment(ref _answered) == _target)
                    _done.Set();

            }

            public bool Accept(QuestionAnswered message)
            {
                lock (_questions)
                    return _questions.ContainsKey(message.CorrelationId);
            }

            public void Add(SubmitQuestion question)
            {
                lock(_questions)
                    _questions.Add(question.CorrelationId, question);
            }
        }

        private void ClientForm_DoWork(object sender, DoWorkEventArgs e)
        {
            ClientFormWorkerArgs args = e.Argument as ClientFormWorkerArgs;
            if (args == null)
                return;

            const int target = 1000;

            TrapperKeeper tk = new TrapperKeeper(target);

            _bus.Subscribe(tk);

            for (int i = 0; i < target; i++)
            {
                SubmitQuestion question = new SubmitQuestion(CombGuid.NewCombGuid());
                tk.Add(question);

                UpdateMessageCount(args.Client, i + 1, tk.Answered);

                _bus.Publish(question);

                if(args.WaitTime > 0)
                    Thread.Sleep(args.WaitTime);
            }

            if ( tk.Done.WaitOne(TimeSpan.FromSeconds(30), true) == false)
            {
                // bad news bears, walter mathow
            }

            UpdateMessageCount(args.Client, target, tk.Answered);
        }

        private void UpdateMessageCount(int client, int sent, int received)
        {
            if (client1Sent.InvokeRequired)
            {
                ThreadSafeUpdateMessageCount tsu = UpdateMessageCount;
                BeginInvoke(tsu, new object[] {client, sent, received});
            }
            else
            {
                switch (client)
                {
                    case 1:
                        client1Sent.Text = sent.ToString();
                        client1Received.Text = received.ToString();
                        break;

                    case 2:
                        client2Sent.Text = sent.ToString();
                        client2Received.Text = received.ToString();
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

        public class ClientFormWorkerArgs
        {
            private readonly int _client;
            private readonly int _waitTime;

            public int Client
            {
                get { return _client; }
            }

            public int WaitTime
            {
                get { return _waitTime; }
            }

            public ClientFormWorkerArgs(int client, int waitTime)
            {
                _client = client;
                _waitTime = waitTime;
            }
        }

        private void Client2Active_CheckedChanged(object sender, EventArgs e)
        {
            if (_workers[1].IsBusy)
                return;

            if (client2Active.Checked == false)
                return;

            _workers[1].RunWorkerAsync(new ClientFormWorkerArgs(2, int.Parse(client1WaitTime.Text)));

        }

        private void Client1Active_CheckedChanged(object sender, EventArgs e)
        {
            if (_workers[0].IsBusy)
                return;

            if (client1Active.Checked == false)
                return;

            _workers[0].RunWorkerAsync(new ClientFormWorkerArgs(1, int.Parse(client1WaitTime.Text)));

        }
    }
}