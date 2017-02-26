namespace MassTransit.Util.Caching
{
    using System.Threading.Tasks;


    public interface ICacheIndex<TValue>
        where TValue : class
    {
        /// <summary>
        /// Clear the index, removing all nodes, but leaving them unmodified
        /// </summary>
        void Clear();

        /// <summary>
        /// Adds a node to the index
        /// </summary>
        /// <param name="node">The node</param>
        /// <returns>True if the value was added, false if the value already existed in the index</returns>
        Task<bool> Add(INode<TValue> node);

        /// <summary>
        /// Check if the value is in the index, and if found, return the node
        /// </summary>
        /// <param name="value">The value</param>
        /// <param name="node">The matching node</param>
        /// <returns>True if the value was found, otherwise false</returns>
        bool TryGetExistingNode(TValue value, out INode<TValue> node);
    }
}