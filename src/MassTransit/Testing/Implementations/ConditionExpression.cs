namespace MassTransit.Testing.Implementations
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;


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
            return Task.CompletedTask;
        }

        /// <summary>
        /// Adds a condition block where all conditions in the array must be logically ANDed together to succeed.
        /// </summary>
        public void AddConditionBlock(params IObservableCondition[] conditions)
        {
            if (conditions.Length == 0)
                throw new ArgumentException("Must add at least 1 condition to a condition block.");

            foreach (var condition in conditions)
                condition.ConnectConditionObserver(this);
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
