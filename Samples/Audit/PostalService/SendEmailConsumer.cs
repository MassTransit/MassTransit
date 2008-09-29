namespace PostalService
{
    using System.Net;
    using System.Net.Mail;
    using log4net;
    using MassTransit.ServiceBus;
    using Messages;

    public class SendEmailConsumer :
        Consumes<SendEmail>.All
    {
        private readonly SmtpClient _client;
        private ILog _log = LogManager.GetLogger(typeof (SendEmailConsumer));

        public SendEmailConsumer(string host, int port, string username, string password)
        {
            _client = new SmtpClient(host, port);
            _client.Credentials = new NetworkCredential(username, password);
        }

        public void Consume(SendEmail message)
        {
            _log.InfoFormat("Sending email to '{0}' with subject '{2}'", message.To, message.Subject);
            _client.Send(message.From, message.To, message.Subject, message.Body);
        }
    }
}