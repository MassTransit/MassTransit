namespace OpenAllNight.Testers
{
    using System.Threading;
    using MassTransit;

    public class MessageCounter :
        Consumes<SimpleMessage>.All
    {
        private static long _messageCount;

        public static long MessageCount
        {
            get { return _messageCount; }
        }

        public void Consume(SimpleMessage message)
        {
            Interlocked.Increment(ref _messageCount);
        }
    }
}