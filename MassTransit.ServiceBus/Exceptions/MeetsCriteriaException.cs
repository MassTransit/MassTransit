namespace MassTransit.ServiceBus.Exceptions
{
    using System;

    public class MeetsCriteriaException<T> 
        : Exception where T : IMessage
    {
        private readonly MessageConsumerCallbackItem<T> _item;

        public MeetsCriteriaException(MessageConsumerCallbackItem<T> item, string s, Exception ex) : base(s, ex)
        {
            _item = item;
        }


        public MessageConsumerCallbackItem<T> Item
        {
            get { return _item; }
        }
    }
}