namespace MassTransit.ServiceBus.Exceptions
{
    using System;

    public class MeetsCriteriaException<T> 
        : Exception where T : IMessage
    {
        private readonly MessageConsumer<T>.CallbackItem<T> _item;

        public MeetsCriteriaException(MessageConsumer<T>.CallbackItem<T> item, string s, Exception ex) : base(s, ex)
        {
            _item = item;
        }


        public MessageConsumer<T>.CallbackItem<T> Item
        {
            get { return _item; }
        }
    }
}