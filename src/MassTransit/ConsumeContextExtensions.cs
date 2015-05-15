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
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Context;
    using Pipeline;


    public static class ConsumeContextExtensions
    {
        public static ConsumerConsumeContext<TConsumer, T> PushConsumer<TConsumer, T>(this ConsumeContext<T> context, TConsumer consumer)
            where T : class
            where TConsumer : class
        {
            return new ConsumerConsumeContextProxy<TConsumer, T>(context, consumer);
        }

        public static async Task Forward<T>(this ConsumeContext<T> context, Uri address)
            where T : class
        {
            ISendEndpoint endpoint = await context.GetSendEndpoint(address).ConfigureAwait(false);

            await Forward(context, endpoint, context.Message).ConfigureAwait(false);
        }

        public static async Task Forward<T>(this ConsumeContext context, Uri address, T message)
            where T : class
        {
            ISendEndpoint endpoint = await context.GetSendEndpoint(address).ConfigureAwait(false);

            await Forward(context, endpoint, message).ConfigureAwait(false);
        }

        /// <summary>
        /// Forward the message to another consumer
        /// </summary>
        /// <param name="context"></param>
        /// <param name="endpoint">The endpoint to forward the message tosaq</param>
        /// <param name="message"></param>
        public static Task Forward<T>(this ConsumeContext context, ISendEndpoint endpoint, T message)
            where T : class
        {
            return endpoint.Send(message, CreateCopyContextPipe(context, GetForwardHeaders));
        }

        static IEnumerable<Tuple<string, object>> GetForwardHeaders(ConsumeContext context)
        {
            Uri inputAddress = context.ReceiveContext.InputAddress ?? context.DestinationAddress;
            if (inputAddress != null)
                yield return Tuple.Create<string, object>("MT-Forwarder-Address", inputAddress.ToString());
        }

        /// <summary>
        /// Create a send pipe that copies the source message headers to the message being sent
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="context"></param>
        /// <param name="additionalHeaders">Returns additional headers for the pipe that should be added to the message</param>
        /// <returns></returns>
        public static IPipe<SendContext> CreateCopyContextPipe(this ConsumeContext context,
            Func<ConsumeContext, IEnumerable<Tuple<string, object>>> additionalHeaders)
        {
            return Pipe.New<SendContext>(x => x.Execute(target =>
            {
                target.RequestId = context.RequestId;
                target.CorrelationId = context.CorrelationId;
                target.SourceAddress = context.SourceAddress;
                target.ResponseAddress = context.ResponseAddress;
                target.FaultAddress = context.FaultAddress;

                if (context.ExpirationTime.HasValue)
                    target.TimeToLive = context.ExpirationTime.Value.ToUniversalTime() - DateTime.UtcNow;

                foreach (var header in context.Headers.GetAll())
                    target.Headers.Set(header.Item1, header.Item2);

                foreach (var additionalHeader in additionalHeaders(context))
                    target.Headers.Set(additionalHeader.Item1, additionalHeader.Item2);
            }));
        }
    }
}