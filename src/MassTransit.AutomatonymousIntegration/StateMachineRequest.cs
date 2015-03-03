// Copyright 2007-2015 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace Automatonymous
{
    using System.Threading.Tasks;
    using MassTransit;


    public class StateMachineRequest<TRequest, TResponse> :
        Request<TRequest, TResponse>
        where TRequest : class
        where TResponse : class
    {
        readonly string _name;
        readonly RequestSettings _settings;

        public StateMachineRequest(string requestName, RequestSettings settings)
        {
            _name = requestName;
            _settings = settings;
        }

        public string Name
        {
            get { return _name; }
        }

        public RequestSettings Settings
        {
            get { return _settings; }
        }

        public Event<TResponse> Completed { get; set; }
        public Event<Fault<TRequest>> Faulted { get; set; }
        public Event<TRequest> TimeoutExpired { get; set; }

        public State Pending { get; set; }

        public async Task SendRequest<T>(ConsumeContext<T> context, TRequest requestMessage)
            where T : class
        {
            // capture requestId
            // send request to endpoint
            // schedule timeout for requestId
        }
    }
}