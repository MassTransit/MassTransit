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
namespace MassTransit.WindsorIntegration
{
    using System.Threading.Tasks;
    using Castle.MicroKernel.Lifestyle.Scoped;
    using GreenPipes;


    /// <summary>
    /// Calls by the inbound message pipeline to begin and end a message scope
    /// in the container.
    /// </summary>
    public class WindsorMessageScopeFilter :
        IFilter<ConsumeContext>
    {
        void IProbeSite.Probe(ProbeContext context)
        {
            context.CreateFilterScope("windsorMessageScope");
        }

        async Task IFilter<ConsumeContext>.Send(ConsumeContext context, IPipe<ConsumeContext> next)
        {
            if (CallContextLifetimeScope.ObtainCurrentScope() is MessageLifetimeScope)
                await next.Send(context).ConfigureAwait(false);
            else
                using (new MessageLifetimeScope())
                {
                    await next.Send(context).ConfigureAwait(false);
                }
        }
    }
}