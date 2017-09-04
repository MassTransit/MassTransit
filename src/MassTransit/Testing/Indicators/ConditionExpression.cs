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
namespace MassTransit.Testing.Indicators
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Util;


    /// <summary>
    /// A collection of blocks of conditions that must occur to signal a resource.
    /// Each condition block in the list is logically OR'd with the other condition blocks.
    /// Each condition within a condition block is logically AND'd with the other conditions in the same block.
    /// </summary>
    public class ConditionExpression : IConditionObserver
    {
        readonly List<IObservableCondition[]> _conditionBlocks = new List<IObservableCondition[]>();
        readonly ISignalResource _resource;

        public ConditionExpression(ISignalResource resource)
        {
            _resource = resource;
        }

        public Task ConditionUpdated()
        {
            if (CheckCondition())
                _resource.Signal();
            return TaskUtil.Completed;
        }

        /// <summary>
        /// Adds a condition block where all conditions in the array must be logically ANDed together to succeed.
        /// </summary>
        public void AddConditionBlock(params IObservableCondition[] conditions)
        {
            if (conditions.Length == 0)
                throw new ArgumentException("Must add at least 1 condition to a condition block.");

            foreach (var condition in conditions)
            {
                condition.ConnectConditionObserver(this);
            }
            _conditionBlocks.Add(conditions);
        }

        public void ClearAllConditions()
        {
            _conditionBlocks.Clear();
        }

        public bool CheckCondition()
        {
            if (_conditionBlocks.Count == 0)
                throw new InvalidOperationException("Cannot check an empty condition.");

            return _conditionBlocks.Any(conditionBlock => conditionBlock.All(condition => condition.IsMet));
        }
    }
}