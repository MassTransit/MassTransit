namespace PostalService
{
    using System;
    using System.Net;
    using System.Net.Mail;
    using log4net;
    using MassTransit;
    using Messages;

    public class SendEmailConsumer :
        Consumes<SendEmail>.All
    {
        private readonly SmtpClient _client;
        private readonly ILog _log = LogManager.GetLogger(typeof (SendEmailConsumer));

        public SendEmailConsumer(string host, int port, string username, string password)
        {
            _client = new SmtpClient(host, port);
            _client.Credentials = new NetworkCredential(username, password);
            _client.EnableSsl = true;
        }

        public IServiceBus Bus { get; set; }

        public void Consume(SendEmail message)
        {
            _log.InfoFormat("Sending email to '{0}' with subject '{1}'", message.To, message.Subject);
            _client.Send(message.From, message.To, message.Subject, message.Body);

            if (message.CorrelationId != Guid.Empty)
            {
                Bus.Publish(new EmailSent(message.CorrelationId));
            }
        }
    }
}