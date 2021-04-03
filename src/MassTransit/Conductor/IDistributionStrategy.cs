namespace MassTransit.Conductor
{
    using System.Collections.Generic;
    using System.Threading.Tasks;


    /// <summary>
    /// A distribution strategy is used to distribute messages across a set of nodes
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="TSelector"></typeparam>
    public interface IDistributionStrategy<T, in TSelector>
    {
        /// <summary>
        /// Adds a range of nodes to the distribution pool
        /// </summary>
        /// <param name="nodes">The nodes to add</param>
        void Init(IEnumerable<T> nodes);

        /// <summary>
        /// Adds a node to the distribution pool
        /// </summary>
        /// <param name="node">The node to add</param>
        void Add(T node);

        /// <summary>
        /// Remove a node (and all of its replicas) from the distribution pool
        /// </summary>
        /// <param name="node">The node to remove</param>
        void Remove(T node);

        /// <summary>
        /// Returns the node for the given selector
        /// </summary>
        /// <param name="data">The data used to select the node</param>
        /// <returns>The element for the specified selector</returns>
        Task<T> GetNode(TSelector data);

        /// <summary>
        /// Returns all nodes that are available and ready to recieve jobs
        /// </summary>
        Task<IEnumerable<T>> GetAvailableNodes();
    }
}
