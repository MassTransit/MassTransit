// Copyright 2007-2011 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.SystemView
{
	using System;
	using System.Windows;
	using Core.Consumer;
	using Core.ViewModel;

	/// <summary>
	/// Interaction logic for DistributorWorkerUpdateWindow.xaml
	/// </summary>
	public partial class DistributorWorkerUpdateWindow : Window
	{
		public DistributorWorkerUpdateWindow()
		{
			InitializeComponent();
		}

		public DistributorWorkerUpdateWindow(Worker worker)
			:
				this()
		{
			tbMessage.Text = worker.MessageType;
			tbControlUri.Text = worker.ControlUri.ToString();
			tbDataUri.Text = worker.DataUri.ToString();
			tbPendingLimit.Text = worker.PendingLimit.ToString();
			tbInProcessLimit.Text = worker.InProgressLimit.ToString();
		}

		void btnCancel_Click(object sender, RoutedEventArgs e)
		{
			Close();
		}

		void btnUpdate_Click(object sender, RoutedEventArgs e)
		{
			// is there a better way to do this?
			SubscriptionDataConsumer sdc = App.SubscriptionDataConsumer;

			sdc.UpdateWorker(new Uri(tbControlUri.Text), tbMessage.Text,
				int.Parse(tbPendingLimit.Text), int.Parse(tbInProcessLimit.Text));

			Close();
		}
	}
}