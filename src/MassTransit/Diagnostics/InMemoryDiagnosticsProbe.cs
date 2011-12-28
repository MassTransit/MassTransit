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
namespace MassTransit.Diagnostics
{
    using System.Linq;
    using System.Text;
    using Magnum.Collections;
    using Magnum.Extensions;

    public class InMemoryDiagnosticsProbe :
        DiagnosticsProbe
    {
        readonly MultiDictionary<string, object> _values;

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
                        kvp.Value.Each(value => { sb.AppendLine("{0}: {1}".FormatWith(key, value)); });
                    });
            return sb.ToString();
        }
    }
}