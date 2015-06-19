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
    using System.IO;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using Monitoring.Introspection;
    using Monitoring.Introspection.Contracts;
    using Newtonsoft.Json;
    using Serialization;


    public static class IntrospectionExtensions
    {
        public static async Task<ProbeResult> GetProbeResult(this IProbeSite probeSite, CancellationToken cancellationToken = default(CancellationToken))
        {
            var builder = new ProbeResultBuilder(NewId.NextGuid(), cancellationToken);

            await probeSite.Probe(builder);

            return ((IProbeResultBuilder)builder).Build();
        }

        public static string ToJsonString(this ProbeResult result)
        {
            var encoding = new UTF8Encoding(false, true);

            using (var stream = new MemoryStream())
            using (var writer = new StreamWriter(stream, encoding, 1024, true))
            using (var jsonWriter = new JsonTextWriter(writer))
            {
                jsonWriter.Formatting = Formatting.Indented;

                JsonMessageSerializer.Serializer.Serialize(jsonWriter, result, typeof(ProbeResult));

                jsonWriter.Flush();
                writer.Flush();

                return encoding.GetString(stream.ToArray());
            }
        }
    }
}