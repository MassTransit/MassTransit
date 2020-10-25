namespace MassTransit.Topology
{
    using System;


    /// <summary>
    /// Specify the EntityName used for this message contract, overriding the configured <see cref="IEntityNameFormatter" />
    /// if configured.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface)]
    public class EntityNameAttribute :
        Attribute
    {
        /// <summary>
        ///
        /// </summary>
        /// <param name="entityName">The entity name to use for the message type</param>
        public EntityNameAttribute(string entityName)
        {
            EntityName = entityName;
        }

        public string EntityName { get; }
    }
}
