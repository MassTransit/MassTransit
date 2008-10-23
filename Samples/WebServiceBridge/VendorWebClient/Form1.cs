namespace VendorWebClient
{
    using System;
    using System.Windows.Forms;
    using BusinessEntity;

    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void checkInventoryButton_Click(object sender, EventArgs e)
        {
            InventoryServiceSoapClient client = new InventoryServiceSoapClient();

            client.CheckInventoryCompleted += CheckInventoryCompleted;

            client.CheckInventoryAsync(partNumberBox.Text);
        }

        private void CheckInventoryCompleted(object sender, CheckInventoryCompletedEventArgs e)
        {
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
            }
        }
    }
}