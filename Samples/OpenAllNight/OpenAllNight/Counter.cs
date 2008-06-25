namespace OpenAllNight
{
    public class Counter
    {
        private long _messagesSent = 0;
        private long _messagesReceived = 0;

        public long MessagesSent
        {
            get { return _messagesSent; }
        }

        public long MessagesReceived
        {
            get { return _messagesReceived; }
        }

        public void IncrementMessagesSent()
        {
            _messagesSent++;    
        }

        public void IncrementMessagesReceived()
        {
            _messagesReceived++;
        }
    }
}