namespace PostalService.Messages
{
    using System;

    [Serializable]
    public class SendEmail
    {
        private string _to;
        private string _from;
        private string _subject;
        private string _body;

        public SendEmail(string to, string from, string subject, string body)
        {
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
    }
}