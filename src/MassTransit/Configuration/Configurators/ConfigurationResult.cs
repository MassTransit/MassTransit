namespace MassTransit.Configurators
{
    using System.Collections.Generic;
    using GreenPipes;


    public interface ConfigurationResult
    {
        IEnumerable<ValidationResult> Results { get; }
    }
}
