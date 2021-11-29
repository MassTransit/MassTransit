namespace MassTransit.Caching.Internals
{
    using System.Threading.Tasks;


    /// <summary>
    /// A factory node is a temporary node used by an index until the node has
    /// been resolved.
    /// </summary>
    /// <typeparam name="TValue">The value type</typeparam>
    public class FactoryNode<TValue> :
        INode<TValue>
        where TValue : class
    {
        readonly INodeValueFactory<TValue> _nodeValueFactory;

        public FactoryNode(INodeValueFactory<TValue> nodeValueFactory)
        {
            _nodeValueFactory = nodeValueFactory;
        }

        public Task<TValue> Value => _nodeValueFactory.Value;

        public bool HasValue => _nodeValueFactory.Value.IsCompleted;

        public bool IsValid
        {
            get
            {
                Task<TValue> value = _nodeValueFactory.Value;
                return !value.IsFaulted && !value.IsCanceled;
            }
        }

        public Task<TValue> GetValue(IPendingValue<TValue> pendingValue)
        {
            _nodeValueFactory.Add(pendingValue);

            return _nodeValueFactory.Value;
        }
    }
}
