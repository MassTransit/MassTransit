namespace MassTransit.Testing
{
    using System;
    using System.Collections.Generic;


    public interface IPublishedMessageList :
        IEnumerable<IPublishedMessage>
    {
        IEnumerable<IPublishedMessage> Select();
        IEnumerable<IPublishedMessage> Select(Func<IPublishedMessage, bool> filter);

        IEnumerable<IPublishedMessage<T>> Select<T>()
            where T : class;

        IEnumerable<IPublishedMessage<T>> Select<T>(Func<IPublishedMessage<T>, bool> filter)
            where T : class;
    }
}
