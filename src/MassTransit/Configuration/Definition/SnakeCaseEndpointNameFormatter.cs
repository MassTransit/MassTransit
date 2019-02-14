// Copyright 2007-2019 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.Definition
{
    using System.Text.RegularExpressions;


    /// <summary>
    /// Formats the endpoint name using snake case. For example,
    ///
    /// SubmitOrderConsumer -> submit_order
    /// OrderState -> order_state
    /// UpdateCustomerActivity -> update_customer_execute, update_customer_compensate
    ///
    /// </summary>
    public class SnakeCaseEndpointNameFormatter :
        DefaultEndpointNameFormatter
    {
        static readonly Regex _pattern = new Regex("(?<=[a-z0-9])[A-Z]", RegexOptions.Compiled);
        readonly string _separator;

        public new static IEndpointNameFormatter Instance { get; } = new SnakeCaseEndpointNameFormatter();

        public SnakeCaseEndpointNameFormatter()
        {
            _separator = "_";
        }

        public SnakeCaseEndpointNameFormatter(string separator)
        {
            _separator = separator ?? "_";
        }

        protected override string SanitizeName(string name)
        {
            return _pattern.Replace(name, m => _separator + m.Value).ToLowerInvariant();
        }
    }
}
