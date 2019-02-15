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
    public class ConsumerEndpointDefinition<TConsumer> :
        IEndpointDefinition<TConsumer>
        where TConsumer : class, IConsumer
    {
        readonly IEndpointSettings<IEndpointDefinition<TConsumer>> _settings;
        string _name;

        public ConsumerEndpointDefinition(IEndpointSettings<IEndpointDefinition<TConsumer>> settings)
        {
            _settings = settings;
        }

        public string GetEndpointName(IEndpointNameFormatter formatter)
        {
            return _name ?? (_name = string.IsNullOrWhiteSpace(_settings.Name)
                ? formatter.Consumer<TConsumer>()
                : _settings.Name);
        }

        public bool IsTemporary => _settings.IsTemporary;
        public int? PrefetchCount => _settings.PrefetchCount;
        public int? ConcurrentMessageLimit => _settings.ConcurrentMessageLimit;

        public void Configure<T>(T configurator)
            where T : IReceiveEndpointConfigurator
        {
        }
    }
}