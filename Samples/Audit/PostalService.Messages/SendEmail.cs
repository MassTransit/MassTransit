namespace PostalService.Messages
{
    using System;
    using MassTransit.ServiceBus;

    [Serializable]
    public class SendEmail :
        CorrelatedBy<Guid>
    {
        private readonly string _body;
        private readonly Guid _correlationId;
        private readonly string _from;
        private readonly string _subject;
        private readonly string _to;

        public SendEmail(Guid correlationId, string to, string from, string subject, string body)
        {
            _correlationId = correlationId;
            _to = to;
            _from = from;
            _subject = subject;
            _body = body;
        }

        public string To
        {
            get { return _to; }
        }

        public string From
        {
            get { return _from; }
        }

        public string Subject
        {
            get { return _subject; }
        }

        public string Body
        {
            get { return _body; }
        }

        public Guid CorrelationId
        {
            get { return _correlationId; }
        }
    }
}