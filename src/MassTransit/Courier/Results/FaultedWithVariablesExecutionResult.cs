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
namespace MassTransit.Courier.Results
{
    using System;
    using System.Collections.Generic;
    using Contracts;


    class FaultedWithVariablesExecutionResult<TArguments> :
        FaultedExecutionResult<TArguments>
        where TArguments : class
    {
        readonly Exception _exception;
        readonly IDictionary<string, object> _variables;

        public FaultedWithVariablesExecutionResult(ExecuteContext<TArguments> executeContext, IRoutingSlipEventPublisher publisher, Activity activity,
            RoutingSlip routingSlip, Exception exception, IDictionary<string, object> variables)
            : base(executeContext, publisher, activity, routingSlip, exception)
        {
            _exception = exception;
            _variables = variables;
        }

        public override bool IsFaulted(out Exception exception)
        {
            exception = new ExceptionWithVariables(_variables, _exception.Message, _exception);
            return true;
        }

        protected override void Build(RoutingSlipBuilder builder)
        {
            base.Build(builder);
            builder.SetVariables(_variables);
        }
    }
}
