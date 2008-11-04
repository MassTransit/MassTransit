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
namespace ServerGUI
{
	using System;
	using System.ComponentModel;
	using System.Windows.Forms;
	using Castle.Windsor;
	using log4net;
	using MassTransit.ServiceBus;
	using MassTransit.WindsorIntegration;

	public partial class ServerForm : Form
	{
		private static readonly ILog _log = LogManager.GetLogger(typeof (ServerForm));

		private IServiceBus _bus;

		private IWindsorContainer _container;
		private IServiceBus _controlBus;

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
				_controlBus = _container.Resolve<IServiceBus>("control");

			    _container.AddComponent<UserAgentSession>();
			    _container.AddComponent<TheAnswerMan>();

				_bus.Subscribe<UserAgentSession>();
				_bus.Subscribe<TheAnswerMan>();

				messageTimer.Start();
			}
			catch (Exception ex)
			{
				_log.Error(ex);
				MessageBox.Show("The server failed to start:\n\n" + ex.Message);
			}
		}


		private void StopService()
		{
			messageTimer.Stop();

			if (_bus != null)
				LogException(() =>
				             	{
				             		_bus.Dispose();
				             		_bus = null;
				             	});

			if (_controlBus != null)
				LogException(() =>
				             	{
									_controlBus.Dispose();
									_controlBus = null;
				             	});

			if (_container != null)
				LogException(() =>
				             	{
									_container.Dispose();
									_container = null;
				             	});
		}

		private void LogException(Action action)
		{
			try
			{
				action();
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

		private void answerQuestions_CheckedChanged(object sender, EventArgs e)
		{
			TheAnswerMan.Enabled = answerQuestions.Checked;
		}

		private void serverTime_ValueChanged(object sender, EventArgs e)
		{
			TheAnswerMan.ServerTime = Convert.ToInt32(serverTime.Value);
		}

		private void messageTimer_Tick(object sender, EventArgs e)
		{
			string text = string.Format("{0}", TheAnswerMan.MessageCount);
			questionsReceived.Text = text;

			text = string.Format("{0}", TheAnswerMan.MessagesSent);
			answersSent.Text = text;
		}
	}
}