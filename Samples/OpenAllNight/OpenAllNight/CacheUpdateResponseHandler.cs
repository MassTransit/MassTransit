namespace OpenAllNight
{
    using log4net;
    using MassTransit;
    using MassTransit.Services.Subscriptions.Messages;

    public class CacheUpdateResponseHandler : 
        Consumes<CacheUpdateResponse>.All
    {
        private static readonly ILog _log = LogManager.GetLogger(typeof (CacheUpdateResponseHandler));
        private readonly Counter _counter;

        public CacheUpdateResponseHandler(Counter counter)
        {
            _counter = counter;
        }

        public void Consume(CacheUpdateResponse message)
        {
            _counter.IncrementMessagesReceived();
            //Console.WriteLine("Received update message number {0}", _counter.MessagesReceived);
        }
    }
}