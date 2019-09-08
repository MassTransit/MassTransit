// Copyright 2007-2016 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.ExtensionsDependencyInjectionIntegration
{
    using System;
    using Automatonymous;
    using GreenPipes;
    using Microsoft.Extensions.DependencyInjection;
    using Util;


    public class DependencyInjectionStateMachineActivityFactory :
        IStateMachineActivityFactory
    {
        Activity<TInstance, TData> IStateMachineActivityFactory.GetActivity<TActivity, TInstance, TData>(BehaviorContext<TInstance, TData> context)
        {
            return GetActivity<TActivity>(context);
        }

        Activity<TInstance> IStateMachineActivityFactory.GetActivity<TActivity, TInstance>(BehaviorContext<TInstance> context)
        {
            return GetActivity<TActivity>(context);
        }

        static TActivity GetActivity<TActivity>(PipeContext context)
        {
            if (context.TryGetPayload(out IServiceScope serviceScope))
                return serviceScope.ServiceProvider.GetRequiredService<TActivity>();

            if (context.TryGetPayload(out IServiceProvider serviceProvider))
                return serviceProvider.GetRequiredService<TActivity>();

            throw new PayloadNotFoundException($"IServiceProvider or IServiceScope was not found to create activity: {TypeMetadataCache<TActivity>.ShortName}");
        }
    }
}
