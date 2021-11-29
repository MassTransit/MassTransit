namespace MassTransit
{
    using System;
    using System.Threading.Tasks;


    public static class RespondAsyncExecuteExtensions
    {
        /// <summary>
        /// Responds to the current message immediately, returning the Task for the
        /// sending message. The caller may choose to await the response to ensure it was sent, or
        /// allow the framework to wait for it (which will happen automatically before the message is acked)
        /// </summary>
        /// <typeparam name="T">The message type</typeparam>
        /// <param name="context">The context to send the message</param>
        /// <param name="message">The message</param>
        /// <param name="callback">The callback for the send context</param>
        /// <returns>The task which is completed once the Send is acknowledged by the broker</returns>
        public static Task RespondAsync<T>(this ConsumeContext context, T message, Action<SendContext<T>> callback)
            where T : class
        {
            return context.RespondAsync(message, callback.ToPipe());
        }

        /// <summary>
        /// Responds to the current message immediately, returning the Task for the
        /// sending message. The caller may choose to await the response to ensure it was sent, or
        /// allow the framework to wait for it (which will happen automatically before the message is acked)
        /// </summary>
        /// <typeparam name="T">The message type</typeparam>
        /// <param name="context">The context to send the message</param>
        /// <param name="message">The message</param>
        /// <param name="callback">The callback for the send context</param>
        /// <returns>The task which is completed once the Send is acknowledged by the broker</returns>
        public static Task RespondAsync<T>(this ConsumeContext context, T message, Func<SendContext<T>, Task> callback)
            where T : class
        {
            return context.RespondAsync(message, callback.ToPipe());
        }

        /// <summary>
        /// Responds to the current message immediately, returning the Task for the
        /// sending message. The caller may choose to await the response to ensure it was sent, or
        /// allow the framework to wait for it (which will happen automatically before the message is acked)
        /// </summary>
        /// <param name="context">The context to send the message</param>
        /// <param name="message">The message</param>
        /// <param name="callback">The callback for the send context</param>
        // <param name="cancellationToken">To cancel the send from happening</param>
        /// <returns>The task which is completed once the Send is acknowledged by the broker</returns>
        public static Task RespondAsync(this ConsumeContext context, object message, Action<SendContext> callback)
        {
            return context.RespondAsync(message, callback.ToPipe());
        }

        /// <summary>
        /// Responds to the current message immediately, returning the Task for the
        /// sending message. The caller may choose to await the response to ensure it was sent, or
        /// allow the framework to wait for it (which will happen automatically before the message is acked)
        /// </summary>
        /// <param name="context">The context to send the message</param>
        /// <param name="message">The message</param>
        /// <param name="callback">The callback for the send context</param>
        /// <returns>The task which is completed once the Send is acknowledged by the broker</returns>
        public static Task RespondAsync(this ConsumeContext context, object message, Func<SendContext, Task> callback)
        {
            return context.RespondAsync(message, callback.ToPipe());
        }

        /// <summary>
        /// Responds to the current message immediately, returning the Task for the
        /// sending message. The caller may choose to await the response to ensure it was sent, or
        /// allow the framework to wait for it (which will happen automatically before the message is acked)
        /// </summary>
        /// <param name="context">The context to send the message</param>
        /// <param name="message">The message</param>
        /// <param name="messageType">The message type to send the object as</param>
        /// <param name="callback">The callback for the send context</param>
        /// <returns>The task which is completed once the Send is acknowledged by the broker</returns>
        public static Task RespondAsync(this ConsumeContext context, object message, Type messageType, Action<SendContext> callback)
        {
            return context.RespondAsync(message, messageType, callback.ToPipe());
        }

        /// <summary>
        /// Responds to the current message immediately, returning the Task for the
        /// sending message. The caller may choose to await the response to ensure it was sent, or
        /// allow the framework to wait for it (which will happen automatically before the message is acked)
        /// </summary>
        /// <param name="context">The context to send the message</param>
        /// <param name="message">The message</param>
        /// <param name="messageType">The message type to send the object as</param>
        /// <param name="callback">The callback for the send context</param>
        /// <returns>The task which is completed once the Send is acknowledged by the broker</returns>
        public static Task RespondAsync(this ConsumeContext context, object message, Type messageType, Func<SendContext, Task> callback)
        {
            return context.RespondAsync(message, messageType, callback.ToPipe());
        }

        /// <summary>
        /// Responds to the current message immediately, returning the Task for the
        /// sending message. The caller may choose to await the response to ensure it was sent, or
        /// allow the framework to wait for it (which will happen automatically before the message is acked)
        /// </summary>
        /// <typeparam name="T">The message type</typeparam>
        /// <param name="context">The context to send the message</param>
        /// <param name="values">The values that map to the object</param>
        /// <param name="callback">The callback for the send context</param>
        /// <returns>The task which is completed once the Send is acknowledged by the broker</returns>
        public static Task RespondAsync<T>(this ConsumeContext context, object values, Action<SendContext> callback)
            where T : class
        {
            return context.RespondAsync(values, callback.ToPipe());
        }

        /// <summary>
        /// Responds to the current message immediately, returning the Task for the
        /// sending message. The caller may choose to await the response to ensure it was sent, or
        /// allow the framework to wait for it (which will happen automatically before the message is acked)
        /// </summary>
        /// <typeparam name="T">The message type</typeparam>
        /// <param name="context">The context to send the message</param>
        /// <param name="values">The values that map to the object</param>
        /// <param name="callback">The callback for the send context</param>
        /// <returns>The task which is completed once the Send is acknowledged by the broker</returns>
        public static Task RespondAsync<T>(this ConsumeContext context, object values, Func<SendContext<T>, Task> callback)
            where T : class
        {
            return context.RespondAsync(values, callback.ToPipe());
        }
    }
}
