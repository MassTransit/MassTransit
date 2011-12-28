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
namespace MassTransit.Diagnostics.Introspection
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Magnum.Extensions;

    public class InMemoryDiagnosticsProbe :
        DiagnosticsProbe
    {
        readonly List<DiagnosticEntry> _entries;

        public InMemoryDiagnosticsProbe()
        {
            _entries = new List<DiagnosticEntry>();
        }

        public void Add(string key, object value)
        {
            var entry = new InMemoryDiagnosticEntry();
            entry.Key = key;
            entry.Value = value.ToString();
            _entries.Add(entry);
        }

        public IEnumerable<DiagnosticEntry> Entries
        {
            get { return _entries; }
        }
        public override string ToString()
        {
            var sb = new StringBuilder();
            _entries
                .OrderBy(entry => entry.Key)
                .Each(entry =>
                    {
                        var key = entry.Key;
                        var value = entry.Value;
                         sb.AppendLine("{0}: {1}".FormatWith(key, value));
                    });
            return sb.ToString();
        }
    }
}