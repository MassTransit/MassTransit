// Copyright 2007-2011 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
    using System.Collections.Generic;
    using System.Text;
    using Builders;
    using BusConfigurators;
    using Configurators;
    using Magnum.Extensions;
    using Magnum.FileSystem;

    public static class DiagnosticsConfiguratorExtensions
    {
         public static void WriteDiagnosticsToFile(this ServiceBusConfigurator cfg, string fileName)
         {
             cfg.AddBusConfigurator(new DiagnosticsBusBuilder(fileName));
         }
    }

    public class DiagnosticsBusBuilder : BusBuilderConfigurator
    {
        string _fileName;

        public DiagnosticsBusBuilder(string fileName)
        {
            _fileName = fileName;
        }

        public IEnumerable<ValidationResult> Validate()
        {
            yield return this.Success("Diagnostics have been turned on.");
        }

        public BusBuilder Configure(BusBuilder builder)
        {
            FileSystem fs = new DotNetFileSystem();
            
            builder.AddPostCreateAction(bus=>
                {
                    var sb = new StringBuilder();
                    fs.DeleteFile(_fileName);
                    sb.AppendLine("Receive From: {0}".FormatWith(bus.Endpoint.Address));
                    sb.AppendLine("Control Bus: {0}".FormatWith(bus.ControlBus.Endpoint.Address));
                    
                    //serializer(s)
                    //services
                    //

                    sb.AppendLine("Max Consumer Threads: {0}".FormatWith( bus.MaximumConsumerThreads));
                    sb.AppendLine("Receive Timeout: {0}".FormatWith(bus.ReceiveTimeout));
                    sb.AppendLine("Concurrent Receive Threads: {0}".FormatWith(bus.ConcurrentReceiveThreads));

                    sb.AppendLine("Outbound Pipe:");
                    bus.OutboundPipeline.View(pipe => sb.AppendLine(pipe));

                    sb.AppendLine("Inbound Pipe:");
                    bus.InboundPipeline.View(pipe => sb.AppendLine(pipe));
                    
                    fs.Write(_fileName, sb.ToString());
                });

            return builder;
        }
    }
}