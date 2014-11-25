// Copyright 2007-2014 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.Configurators
{
    using System;
    using System.Collections.Generic;
    using System.Linq;


    public class PipeConfigurationResult :
        IPipeConfigurationResult
    {
        readonly List<ValidationResult> _results;

        public PipeConfigurationResult(IEnumerable<ValidationResult> results)
        {
            _results = results.ToList();
        }

        public bool ContainsFailure
        {
            get { return _results.Any(x => x.Disposition == ValidationResultDisposition.Failure); }
        }

        public string GetMessage(string header)
        {
            if (header == null)
                throw new ArgumentNullException("header");

            string message = header +
                             Environment.NewLine +
                             string.Join(Environment.NewLine, _results.Select(x => x.ToString()).ToArray());

            return message;
        }

        public bool Any()
        {
            return _results.Count > 0;
        }
    }
}