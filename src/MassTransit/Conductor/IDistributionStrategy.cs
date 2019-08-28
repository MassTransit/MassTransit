namespace MassTransit.Conductor
{
    using System.Collections.Generic;


    /// <summary>
    /// A distribution strategy is used to distribute messages across a set of nodes
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IDistributionStrategy<T>
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
        /// Returns the node for the given data block
        /// </summary>
        /// <param name="data">The data block used to select the node</param>
        /// <returns>The element for the specified data block</returns>
        T GetNode(byte[] data);
    }
}
