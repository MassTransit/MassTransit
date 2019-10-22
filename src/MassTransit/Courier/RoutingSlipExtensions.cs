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
namespace MassTransit.Courier
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Context;
    using Contracts;
    using Newtonsoft.Json;


    public static class RoutingSlipExtensions
    {
        static RoutingSlipExtensions()
        {
            MessageCorrelation.UseCorrelationId<RoutingSlip>(x => x.TrackingNumber);
        }

        /// <summary>
        /// Returns true if there are no remaining activities to be executed
        /// </summary>
        /// <param name="routingSlip"></param>
        /// <returns></returns>
        public static bool RanToCompletion(this RoutingSlip routingSlip)
        {
            return routingSlip.Itinerary == null || routingSlip.Itinerary.Count == 0;
        }

        public static Uri GetNextExecuteAddress(this RoutingSlip routingSlip)
        {
            return routingSlip.Itinerary.Select(x => x.Address).First();
        }

        public static Uri GetNextCompensateAddress(this RoutingSlip routingSlip)
        {
            return routingSlip.CompensateLogs.Select(x => x.Address).Last();
        }

        public static async Task Execute<T>(this T source, RoutingSlip routingSlip)
            where T : IPublishEndpoint, ISendEndpointProvider
        {
            if (routingSlip.RanToCompletion())
            {
                var timestamp = DateTime.UtcNow;
                var duration = timestamp - routingSlip.CreateTimestamp;

                IRoutingSlipEventPublisher publisher = new RoutingSlipEventPublisher(source, source, routingSlip);

                await publisher.PublishRoutingSlipCompleted(timestamp, duration, routingSlip.Variables).ConfigureAwait(false);
            }
            else
            {
                var endpoint = await source.GetSendEndpoint(routingSlip.GetNextExecuteAddress()).ConfigureAwait(false);

                await endpoint.Send(routingSlip).ConfigureAwait(false);
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
