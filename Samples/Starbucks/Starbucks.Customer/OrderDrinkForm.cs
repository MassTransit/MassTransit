namespace Starbucks.Customer
{
	using System;
	using System.ComponentModel;
	using System.Windows.Forms;
	using Magnum;
	using MassTransit;
	using Messages;
	using Microsoft.Practices.ServiceLocation;

	public partial class OrderDrinkForm : Form,
	                                      Consumes<PaymentDueMessage>.For<string>,
	                                      Consumes<DrinkReadyMessage>.For<string>
	{
		private UnsubscribeAction _unsubscribeToken;
		private IServiceBus _bus;
		private IServiceBus Bus
		{
			get
			{
				if (_bus == null)
					_bus = ServiceLocator.Current.GetInstance<IServiceBus>();

				return _bus;
			}
		}


		public OrderDrinkForm()
		{
			InitializeComponent();
		}

		#region For<string> Members

		public void Consume(DrinkReadyMessage message)
		{
			MessageBox.Show(string.Format("Hey, {0}, your drink is ready.", message.Name));

			_unsubscribeToken();
			_unsubscribeToken = null;
		}

		#endregion

		#region For<string> Members

		public string CorrelationId
		{
			get { return txtName.Text; }
		}

		public void Consume(PaymentDueMessage message)
		{
			string prompt = string.Format("Payment due: ${0}", message.Amount);
			if (MessageBox.Show(prompt, "You need to pay", MessageBoxButtons.OKCancel) == DialogResult.OK)
			{
				Bus.Publish(new SubmitPaymentMessage(message.StarbucksTransactionId, PaymentType.CreditCard, message.Amount, message.Name));
			}
		}

		#endregion

		private void button1_Click(object sender, EventArgs e)
		{
			string drink = comboBox1.Text;
			string size = comboBox2.Text;

			string name = txtName.Text;

			if (_unsubscribeToken != null)
				_unsubscribeToken();

			_unsubscribeToken = Bus.Subscribe(this);

			Bus.Publish(new NewOrderMessage(CombGuid.Generate(), name, drink, size));
		}

		protected override void OnClosing(CancelEventArgs e)
		{
			if (_unsubscribeToken != null)
			{
				_unsubscribeToken();
				_unsubscribeToken = null;
			}
			if(_bus != null)
			{
				_bus.Dispose();
				_bus = null;
			}

			base.OnClosing(e);
		}
	}
}