// Copyright 2007-2017 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.AutofacIntegration
{
    /// <summary>
    /// Uses a string-based header to identify the lifetime scope
    /// </summary>
    public class StringHeaderLifetimeScopeIdProvider :
        ILifetimeScopeIdProvider<string>
    {
        readonly ConsumeContext _consumeContext;
        readonly string _headerKey;

        public StringHeaderLifetimeScopeIdProvider(ConsumeContext consumeContext, string headerKey)
        {
            _consumeContext = consumeContext;
            _headerKey = headerKey;
        }

        public bool TryGetScopeId(out string id)
        {
            id = _consumeContext.Headers.Get<string>(_headerKey);

            return !string.IsNullOrWhiteSpace(id);
        }
    }
}