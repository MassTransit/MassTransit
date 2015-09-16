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
namespace MassTransit.Hosting
{
    /// <summary>
    /// These endpoint settings are loaded from the app.config of the assembly to override
    /// the default values. By default, the queue name is derived from the endpoint specification
    /// class name.
    /// </summary>
    public interface EndpointSettings :
        ISettings
    {
        /// <summary>
        /// The queue name for the endpoint
        /// </summary>
        string QueueName { get; }

        /// <summary>
        /// The consumer limit for the endpoint
        /// </summary>
        int? ConsumerLimit { get; }
    }
}