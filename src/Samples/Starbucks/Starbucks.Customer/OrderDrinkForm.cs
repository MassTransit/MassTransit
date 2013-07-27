// Copyright 2007-2013 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace Starbucks.Customer
{
	using System;
	using System.ComponentModel;
	using System.Windows.Forms;
	using Magnum;
	using MassTransit;
	using Messages;

	public partial class OrderDrinkForm :
		Form,
		Consumes<PaymentDueMessage>.For<Guid>,
		Consumes<DrinkReadyMessage>.For<Guid>
	{
		private UnsubscribeAction _unsubscribeToken;

		public OrderDrinkForm(IServiceBus bus)
		{
			InitializeComponent();

		    Bus = bus;
		}

		private IServiceBus Bus { get; set; }
		public Guid CorrelationId { get; private set; }

		public void Consume(DrinkReadyMessage message)
		{
			MessageBox.Show(string.Format("Hey, {0}, your {1} is ready.", message.Name, message.Drink));

			_unsubscribeToken();
			_unsubscribeToken = null;
		}

		public void Consume(PaymentDueMessage message)
		{
			
			string prompt = string.Format("Payment due: ${0} Would you like to add a tip?", message.Amount);

			DialogResult result = MessageBox.Show(prompt, "Payment Due", MessageBoxButtons.YesNoCancel);

			decimal payment = message.Amount;
			if (result == DialogResult.Yes)
				payment += payment*0.2m;

			if (result != DialogResult.Cancel)
			{
				var submitPaymentMessage = new SubmitPaymentMessage
					{
						CorrelationId = message.CorrelationId,
						PaymentType = PaymentType.CreditCard,
						Amount = payment,
					};

				Bus.Publish(submitPaymentMessage);
			}
		}

		private void button1_Click(object sender, EventArgs e)
		{
			string drink = comboBox1.Text;
			string size = comboBox2.Text;
			string name = txtName.Text;

			CorrelationId = CombGuid.Generate();

			if (_unsubscribeToken != null)
				_unsubscribeToken();
			_unsubscribeToken = Bus.SubscribeInstance(this);

			var message = new NewOrderMessage
				{
					CorrelationId = CorrelationId,
					Item = drink,
					Name = name,
					Size = size,
				};

			Bus.Publish(message, x=> x.SetResponseAddress(Bus.Endpoint.Address.Uri));
		}

		protected override void OnClosing(CancelEventArgs e)
		{
			if (_unsubscribeToken != null)
			{
				_unsubscribeToken();
				_unsubscribeToken = null;
			}
			if (Bus != null)
			{
				Bus.Dispose();
				Bus = null;
			}

			base.OnClosing(e);
		}
	}
}