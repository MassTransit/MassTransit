namespace MassTransit
{
    using System.Collections.Generic;
    using System.ComponentModel;


    /// <summary>
    /// A specification, that can be validated as part of a configurator, is used
    /// to allow nesting and chaining of specifications while ensuring that all aspects
    /// of the configuration are verified correct.
    /// </summary>
    public interface ISpecification
    {
        /// <summary>
        /// Validate the specification, ensuring that a successful build will occur.
        /// </summary>
        /// <returns></returns>
        [EditorBrowsable(EditorBrowsableState.Never)]
        IEnumerable<ValidationResult> Validate();
    }
}
