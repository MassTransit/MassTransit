using System;
using System.Windows.Forms;
using Microsoft.Practices.ServiceLocation;
using Starbucks.Messages;

namespace Starbucks.Customer
{
    using MassTransit;

    public partial class OrderDrinkForm : Form,
                                          Consumes<PaymentDueMessage>.For<string>,
                                          Consumes<DrinkReadyMessage>.For<string>
    {
        public OrderDrinkForm()
        {
            InitializeComponent();
        }

        #region For<string> Members

        public void Consume(DrinkReadyMessage message)
        {
            MessageBox.Show(string.Format("Hey, {0}, your drink is ready.", message.Name));
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
                IServiceBus bus = GetBus();
                bus.Publish(new SubmitPaymentMessage(message.StarbucksTransactionId, PaymentType.CreditCard, message.Amount, message.Name));
            }
        }

        #endregion

        private void button1_Click(object sender, EventArgs e)
        {
            string drink = comboBox1.Text;
            string size = comboBox2.Text;

            string name = txtName.Text;

            IServiceBus bus = GetBus();
            bus.Subscribe(this);
            bus.Publish(new NewOrderMessage(name, drink, size));
        }

        private IServiceBus GetBus()
        {
            return ServiceLocator.Current.GetInstance<IServiceBus>();
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            var bus = GetBus();
            bus.Unsubscribe(this);
            base.OnClosing(e);
        }
    }
}