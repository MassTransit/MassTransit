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
namespace MassTransit.Pipeline.Filters
{
    using System.Threading.Tasks;
    using Subscriptions;


    /// <summary>
    /// Connects multiple output pipes to a single input pipe
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class TeeConsumeFilter<T> :
        IFilter<ConsumeContext<T>>,
        IConsumeFilterConnector<T>
        where T : class
    {
        readonly Connectable<IPipe<ConsumeContext<T>>> _connections;

        public TeeConsumeFilter()
        {
            _connections = new Connectable<IPipe<ConsumeContext<T>>>();
        }

        public int Count
        {
            get { return _connections.Count; }
        }

        public async Task Send(ConsumeContext<T> context, IPipe<ConsumeContext<T>> next)
        {
            await _connections.ForEach(async pipe => await pipe.Send(context));

            await next.Send(context);
        }

        public bool Inspect(IPipeInspector inspector)
        {
            return inspector.Inspect(this, x => _connections.All(pipe => pipe.Inspect(x)));
        }

        public ConnectHandle Connect(IPipe<ConsumeContext<T>> pipe)
        {
            return _connections.Connect(pipe);
        }
    }
}