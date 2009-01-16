namespace VendorWebClient
{
    using System;
    using System.Diagnostics;
    using System.Windows.Forms;
    using BusinessEntity;

    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

    	private Stopwatch _timer;

        private void checkInventoryButton_Click(object sender, EventArgs e)
        {
            InventoryServiceSoapClient client = new InventoryServiceSoapClient();

            client.CheckInventoryCompleted += CheckInventoryCompleted;

        	_timer = Stopwatch.StartNew();

            client.CheckInventoryAsync(partNumberBox.Text);
        }

        private void CheckInventoryCompleted(object sender, CheckInventoryCompletedEventArgs e)
        {
        	_timer.Stop();

            if(e.Error != null)
            {
                MessageBox.Show(e.Error.Message, "Inventory Service Error");

                onHandBox.Text = "ERROR";
                onOrderBox.Text = "ERROR";
                return;
            }

            if (e.Result != null)
            {
                responsePartNumber.Text = e.Result.PartNumber;
                onHandBox.Text = e.Result.QuantityOnHand.ToString();
                onOrderBox.Text = e.Result.QuantityOnOrder.ToString();
            	responseTime.Text = string.Format("{0}ms", _timer.ElapsedMilliseconds);
            }
        }
    }
}