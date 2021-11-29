namespace MassTransit
{
    using System;
    using Configuration;


    public interface ITransformSpecificationConfigurator<TMessage>
        where TMessage : class
    {
        /// <summary>
        /// Get a transform specification using the default constructor
        /// </summary>
        /// <typeparam name="T">The transform specification type</typeparam>
        /// <returns></returns>
        IConsumeTransformSpecification<TMessage> Get<T>()
            where T : IConsumeTransformSpecification<TMessage>, new();

        /// <summary>
        /// Get a transform specification using the factory method
        /// </summary>
        /// <typeparam name="T">The transform specification type</typeparam>
        /// <param name="transformFactory">The transform specification factory method</param>
        /// <returns></returns>
        IConsumeTransformSpecification<TMessage> Get<T>(Func<T> transformFactory)
            where T : IConsumeTransformSpecification<TMessage>;
    }
}
