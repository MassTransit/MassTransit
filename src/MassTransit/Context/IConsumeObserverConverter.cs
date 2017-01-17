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
namespace MassTransit.Context
{
    using System;
    using System.Threading.Tasks;


    /// <summary>
    /// Calls the generic version of the IPublishEndpoint.Send method with the object's type
    /// </summary>
    public interface IConsumeObserverConverter
    {
        Task PreConsume(IConsumeObserver observer, object context);

        Task PostConsume(IConsumeObserver observer, object context);

        Task ConsumeFault(IConsumeObserver observer, object context, Exception exception);
    }
}