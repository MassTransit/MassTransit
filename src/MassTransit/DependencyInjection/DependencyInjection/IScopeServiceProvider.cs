namespace MassTransit.DependencyInjection
{
    using System;


    /// <summary>
    /// Supports retrieval of services used by the ConfigurationRegistry
    /// </summary>
    public interface IScopeServiceProvider :
        IServiceProvider
    {
    }
}
