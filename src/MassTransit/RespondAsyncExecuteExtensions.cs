// Copyright 2007-2015 Chris Patterson, Dru Sellers, Travis Smith, et. al.
//  
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use
// this file except in compliance with the License. You may obtain a copy of the 
// License at 
// 
//     http://www.apache.org/licenses/LICENSE-2.0 
// 
// Unless required by applicable law or agreed to in writing, software distributed
// under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR 
// CONDITIONS OF ANY KIND, either express or implied. See the License for the 
// specific language governing permissions and limitations under the License.
namespace MassTransit
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using GreenPipes;


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
        public static Task RespondAsync<T>(this ConsumeContext<T> context, T message, Action<SendContext<T>> callback)
            where T : class
        {
            return context.RespondAsync(message, Pipe.Execute(callback));
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
        public static Task RespondAsync<T>(this ConsumeContext<T> context, T message, Action<SendContext> callback)
            where T : class
        {
            return context.RespondAsync(message, Pipe.Execute(callback));
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
        public static Task RespondAsync<T>(this ConsumeContext<T> context, T message, Func<SendContext<T>, Task> callback)
            where T : class
        {
            return context.RespondAsync(message, Pipe.ExecuteAsync(callback));
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
        public static Task RespondAsync<T>(this ConsumeContext<T> context, T message, Func<SendContext, Task> callback)
            where T : class
        {
            return context.RespondAsync(message, Pipe.ExecuteAsync(callback));
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
            return context.RespondAsync(message, Pipe.Execute(callback));
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
            return context.RespondAsync(message, Pipe.ExecuteAsync(callback));
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
            return context.RespondAsync(message, messageType, Pipe.Execute(callback));
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
            return context.RespondAsync(message, messageType, Pipe.ExecuteAsync(callback));
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
            return context.RespondAsync(values, Pipe.Execute(callback));
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
            return context.RespondAsync(values, Pipe.ExecuteAsync(callback));
        }
    }
}