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
    using System.Threading.Tasks;


    public static class RetryLaterExtensions
    {
        /// <summary>
        /// A legacy hold over, RetryLater just increments the delivery count and puts the message at the end
        /// of the input queue. There is no delay, so an empty queue just spins -- quickly. In fact, another consumer
        /// can start before this one completes the RetryLater operation. Consider using Defer instead.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="context"></param>
        /// <returns></returns>
        public static async Task RetryLater<T>(this ConsumeContext<T> context)
            where T : class
        {
            ISendEndpoint endpoint = await context.GetSendEndpoint(context.ReceiveContext.InputAddress);

            await endpoint.Send(context.Message, context.CreateCopyContextPipe(UpdateDeliveryContext));
        }

        static void UpdateDeliveryContext(ConsumeContext context, SendContext sendContext)
        {
            int? previousDeliveryCount = context.Headers.Get("MT-Redelivery-Count", default(int?));
            if (!previousDeliveryCount.HasValue)
                previousDeliveryCount = 0;
            sendContext.Headers.Set("MT-Redelivery-Count", previousDeliveryCount.Value + 1);
        }
    }
}