namespace MassTransit.Observables
{
    using System;
    using System.Threading.Tasks;
    using Util;


    public class PublishObservable :
        Connectable<IPublishObserver>,
        IPublishObserver,
        ISendObserver
    {
        public Task PrePublish<T>(PublishContext<T> context)
            where T : class
        {
            return ForEachAsync(x => x.PrePublish(context));
        }

        public Task PostPublish<T>(PublishContext<T> context)
            where T : class
        {
            return ForEachAsync(x => x.PostPublish(context));
        }

        public Task PublishFault<T>(PublishContext<T> context, Exception exception)
            where T : class
        {
            return ForEachAsync(x => x.PublishFault(context, exception));
        }

        public Task PreSend<T>(SendContext<T> context)
            where T : class
        {
            var publishContext = context.GetPayload<PublishContext<T>>();

            return ForEachAsync(x => x.PrePublish(publishContext));
        }

        public Task PostSend<T>(SendContext<T> context)
            where T : class
        {
            var publishContext = context.GetPayload<PublishContext<T>>();

            return ForEachAsync(x => x.PostPublish(publishContext));
        }

        public Task SendFault<T>(SendContext<T> context, Exception exception)
            where T : class
        {
            var publishContext = context.GetPayload<PublishContext<T>>();

            return ForEachAsync(x => x.PublishFault(publishContext, exception));
        }
    }
}
