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
    using System.Linq;
    using System.Text;
    using Builders;
    using BusConfigurators;
    using Configurators;
    using Magnum.Collections;
    using Magnum.FileSystem;
    using Magnum.Extensions;

    public static class DiagnosticsConfiguratorExtensions
    {
        public static void WriteDiagnosticsTo(this ServiceBusConfigurator cfg, Action<DiagnosticsProbe> action)
        {
            
             cfg.AddBusConfigurator(new DiagnosticsBusBuilder(action));
        }

        public static void WriteDiagnosticsToConsole(this ServiceBusConfigurator cfg)
        {
            cfg.WriteDiagnosticsTo(Console.Write);
        }

        public static void WriteDiagnosticsToFile(this ServiceBusConfigurator cfg, string fileName)
         {
            cfg.WriteDiagnosticsTo(probe =>
                {
                    var fs = new DotNetFileSystem();
                    fs.DeleteFile(fileName);
                    fs.Write(fileName, probe.ToString());
                });
         }
    }

    public class DiagnosticsBusBuilder : BusBuilderConfigurator
    {
        readonly Action<DiagnosticsProbe> _writeAction;

        public DiagnosticsBusBuilder(Action<DiagnosticsProbe> writeAction)
        {
            _writeAction = writeAction;
        }

        public IEnumerable<ValidationResult> Validate()
        {
            yield return this.Success("Diagnostics have been turned on.");
        }

        public BusBuilder Configure(BusBuilder builder)
        {
            var probe = new InMemoryDiagnosticsProbe();
            builder.AddPostCreateAction(bus=>
                {   
                    probe.Add("mt.version", GetType().Assembly.GetName().Version);
                    probe.Add("host.machine_name", Environment.MachineName);
                    OperatingSystem(probe);
                    Process(probe);
                    probe.Add("mt.network_key", builder.Settings.Network);

                    bus.Diagnose(probe);
                    
                   


                    _writeAction(probe);
                });

            return builder;
        }

        void Process(DiagnosticsProbe probe)
        {
            var msg = "PID?";
            if (Environment.Is64BitProcess)
                msg = msg + " (x64)";
         
            probe.Add("os.process",msg);
        }

        void OperatingSystem(DiagnosticsProbe probe)
        {
            var msg = Environment.OSVersion.ToString();
            if (Environment.Is64BitOperatingSystem)
                msg = msg + " (x64)";
            
            probe.Add("os", msg);
        }
    }

    public interface DiagnosticsProbe
    {
        //methods as needed
        void Add(string key, object  value);
    }

    public class InMemoryDiagnosticsProbe :
        DiagnosticsProbe
    {
        MultiDictionary<string, object> _values;

        public InMemoryDiagnosticsProbe()
        {
            _values = new MultiDictionary<string, object>(true);
        }

        public void Add(string key, object value)
        {
            _values.Add(key, value);
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            _values
                .OrderBy(kvp => kvp.Key)
                .Each(kvp =>
                {
                    var key = kvp.Key;
                    kvp.Value.Each(value =>
                        {
                            sb.AppendLine("{0}: {1}".FormatWith(key, value));
                        });
                });
            return sb.ToString();
        }
    }

    public interface DiagnosticsSource
    {
        void Diagnose(DiagnosticsProbe probe);
    }
}