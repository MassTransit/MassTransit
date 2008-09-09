namespace ClientGUI
{
    using System.Windows.Forms;
    using MassTransit.ServiceBus;
    using MassTransit.ServiceBus.Timeout.Messages;

    public class TimeoutWatcher :
        Consumes<TimeoutExpired>.All
    {
        public void Consume(TimeoutExpired message)
        {
           MessageBox.Show("Timeout Acheived", "bob", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}