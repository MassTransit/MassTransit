namespace MassTransit.Persistence.Configuration
{
    /// <summary>
    /// Enables JobConsumer support via preconfigured saga repositories.
    /// </summary>
    public interface ICustomJobSagaRepositoryConfigurator
    {
        /// <summary>
        /// Gets/sets a custom context factory for JobSagas.
        /// </summary>
        DatabaseContextFactory<JobSaga>? JobContextFactory { get; set; }

        /// <summary>
        /// Gets/sets a custom context factory for JobTypeSagas.
        /// </summary>
        DatabaseContextFactory<JobTypeSaga>? JobTypeContextFactory { get; set; }

        /// <summary>
        /// Gets/sets a custom context factory for JobAttemptSagas.
        /// </summary>
        DatabaseContextFactory<JobAttemptSaga>? JobAttemptContextFactory { get; set; }

        /// <summary>
        /// Set a custom context factory for JobSagas.
        /// </summary>
        ICustomJobSagaRepositoryConfigurator SetJobContextFactory(DatabaseContextFactory<JobSaga> contextFactory);

        /// <summary>
        /// Set a custom context factory for JobTypeSagas.
        /// </summary>
        ICustomJobSagaRepositoryConfigurator SetJobTypeContextFactory(DatabaseContextFactory<JobTypeSaga> contextFactory);

        /// <summary>
        /// Set a custom context factory for JobAttemptSagas.
        /// </summary>
        ICustomJobSagaRepositoryConfigurator SetJobAttemptContextFactory(DatabaseContextFactory<JobAttemptSaga> contextFactory);
    }
}
