using System;
using System.Data.Entity;

namespace MassTransit.EntityFrameworkIntegration
{
    using MassTransit.Saga;

    /// <summary>
    /// Factory method for creating new database context connections.
    /// </summary>
    public delegate DbContext SagaDbContextFactory();
}
