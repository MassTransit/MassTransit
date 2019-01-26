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
namespace MassTransit.WindsorIntegration.ScopeProviders
{
    using System;
    using Castle.MicroKernel;
    using Castle.MicroKernel.Lifestyle;
    using Castle.MicroKernel.Lifestyle.Scoped;
    using GreenPipes;
    using Scoping;


    static class InternalScopeExtensions
    {
        public static IDisposable CreateNewOrUseExistingMessageScope(this IKernel kernel)
        {
            var currentScope = CallContextLifetimeScope.ObtainCurrentScope();
            if (currentScope is MessageLifetimeScope)
                return null;

            return kernel.BeginScope();
        }

        public static void UpdateScope(this IKernel kernel, ConsumeContext context)
        {
            kernel.Resolve<ScopedConsumeContextProvider>().SetContext(context);
        }

        public static void UpdatePayload(this PipeContext context, IKernel kernel)
        {
            context.AddOrUpdatePayload(() => kernel, existing => kernel);
        }
    }
}
