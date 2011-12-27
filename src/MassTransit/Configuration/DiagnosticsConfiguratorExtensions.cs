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
    using System;
    using System.Collections.Generic;
    using System.Text;
    using Builders;
    using BusConfigurators;
    using Configurators;
    using Magnum.Extensions;
    using Magnum.FileSystem;

    public static class DiagnosticsConfiguratorExtensions
    {
        public static void WriteDiagnosticsTo(this ServiceBusConfigurator cfg, Action<string> action)
        {
            
             cfg.AddBusConfigurator(new DiagnosticsBusBuilder(action));
        }

        public static void WriteDiagnosticsToConsole(this ServiceBusConfigurator cfg)
        {
            cfg.WriteDiagnosticsTo(Console.Write);
        }

        public static void WriteDiagnosticsToFile(this ServiceBusConfigurator cfg, string fileName)
         {
            cfg.WriteDiagnosticsTo(contents =>
                {
                    var fs = new DotNetFileSystem();
                    fs.DeleteFile(fileName);
                    fs.Write(fileName, contents);
                });
         }
    }

    public class DiagnosticsBusBuilder : BusBuilderConfigurator
    {
        readonly Action<string> _writeAction;

        public DiagnosticsBusBuilder(Action<string> writeAction)
        {
            _writeAction = writeAction;
        }

        public IEnumerable<ValidationResult> Validate()
        {
            yield return this.Success("Diagnostics have been turned on.");
        }

        public BusBuilder Configure(BusBuilder builder)
        {
            builder.AddPostCreateAction(bus=>
                {
                    var sb = new StringBuilder();
                    
                    sb.AppendLine("Machine Name: {0}".FormatWith(System.Environment.MachineName));

                    OperatingSystem(sb);
                    Process(sb);

                    sb.AppendLine("Receive From: {0}".FormatWith(bus.Endpoint.Address));
                    sb.AppendLine("Control Bus: {0}".FormatWith(bus.ControlBus.Endpoint.Address));
                    sb.AppendLine("Network Key: {0}".FormatWith(builder.Settings.Network));
                    
                    //serializer(s)
                    //service(s) --
                    //transport(s)

                    sb.AppendLine("Max Consumer Threads: {0}".FormatWith( bus.MaximumConsumerThreads));
                    sb.AppendLine("Receive Timeout: {0}".FormatWith(bus.ReceiveTimeout));
                    sb.AppendLine("Concurrent Receive Threads: {0}".FormatWith(bus.ConcurrentReceiveThreads));
                    
                    sb.Append("Outbound ");
                    bus.OutboundPipeline.View(pipe => sb.AppendLine(pipe));

                    sb.AppendLine("Inbound ");
                    bus.InboundPipeline.View(pipe => sb.AppendLine(pipe));

                    _writeAction(sb.ToString());
                });

            return builder;
        }

        void Process(StringBuilder sb)
        {
            sb.Append("Process: PID?");
            if (System.Environment.Is64BitProcess)
                sb.Append(" (x64)");
            sb.AppendLine();
        }

        void OperatingSystem(StringBuilder sb)
        {
            sb.Append("OS: {0}".FormatWith(System.Environment.OSVersion));
            if (System.Environment.Is64BitOperatingSystem)
                sb.AppendFormat(" (x64)");
            sb.AppendLine();
            
        }
    }
}