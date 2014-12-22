// Copyright 2007-2014 Chris Patterson
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
namespace MassTransit.Courier
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Contracts;
    using Events;
    using InternalMessages;
    using Newtonsoft.Json;


    public static class RoutingSlipExtensions
    {
        /// <summary>
        /// Returns true if there are no remaining activities to be executed
        /// </summary>
        /// <param name="routingSlip"></param>
        /// <returns></returns>
        public static bool RanToCompletion(this RoutingSlip routingSlip)
        {
            return routingSlip.Itinerary == null || routingSlip.Itinerary.Count == 0;
        }

        /// <summary>
        /// Returns true if at least one activity log is present, signifying that activities have
        /// been executed with compensation logs
        /// </summary>
        /// <param name="routingSlip"></param>
        /// <returns></returns>
        public static bool IsRunning(this RoutingSlip routingSlip)
        {
            return routingSlip.ActivityLogs != null && routingSlip.ActivityLogs.Count > 0;
        }

        public static Uri GetNextExecuteAddress(this RoutingSlip routingSlip)
        {
            Activity activity = routingSlip.Itinerary.First();

            return activity.Address;
        }

        public static Uri GetNextCompensateAddress(this RoutingSlip routingSlip)
        {
            CompensateLog activity = routingSlip.CompensateLogs.Last();

            return activity.Address;
        }

        public static async Task Execute(this IBus bus, RoutingSlip routingSlip)
        {
            if (routingSlip.RanToCompletion())
            {
                DateTime timestamp = DateTime.UtcNow;
                TimeSpan duration = timestamp - routingSlip.CreateTimestamp;

                IRoutingSlipEventPublisher publisher = new RoutingSlipEventPublisher(bus, bus, routingSlip);

                await
                    publisher.Publish(new RoutingSlipCompletedMessage(routingSlip.TrackingNumber, timestamp, duration, routingSlip.Variables));
            }
            else
            {
                ISendEndpoint endpoint = await bus.GetSendEndpoint(routingSlip.GetNextExecuteAddress());

                await endpoint.Send(routingSlip, Pipe.New<SendContext>(x => x.Execute(context => context.SourceAddress = bus.Address)));
            }
        }

        public static string ToJsonString(this RoutingSlip routingSlip)
        {
            using (var stream = new MemoryStream())
            using (var writer = new StreamWriter(stream))
            {
                SerializerCache.Serializer.Serialize(writer, routingSlip);

                writer.Flush();

                return Encoding.UTF8.GetString(stream.ToArray());
            }
        }

        public static RoutingSlip GetRoutingSlip(string json)
        {
            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(json)))
            using (var writer = new StreamReader(stream))
            using (var reader = new JsonTextReader(writer))
            {
                return SerializerCache.Deserializer.Deserialize<RoutingSlip>(reader);
            }
        }
    }
}