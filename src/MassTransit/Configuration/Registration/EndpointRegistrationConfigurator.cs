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
namespace MassTransit.Registration
{
    using Definition;


    public abstract class EndpointRegistrationConfigurator<T>
        where T : class
    {
        readonly EndpointSettings<IEndpointDefinition<T>> _settings;

        protected EndpointRegistrationConfigurator()
        {
            _settings = new EndpointSettings<IEndpointDefinition<T>>();
        }

        public string Name
        {
            set => _settings.Name = value;
        }

        public bool Temporary
        {
            set => _settings.IsTemporary = value;
        }

        public int? PrefetchCount
        {
            set => _settings.PrefetchCount = value;
        }

        public int? ConcurrentMessageLimit
        {
            set => _settings.ConcurrentMessageLimit = value;
        }

        public IEndpointSettings<IEndpointDefinition<T>> Settings => _settings;
    }
}
