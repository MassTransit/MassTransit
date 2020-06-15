namespace MassTransit
{
    using System;
    using Contracts.JobService;
    using Util;


    public static class JobServiceEventExtensions
    {
        /// <summary>
        /// Deserializes the job from the message
        /// </summary>
        /// <typeparam name="T">The job type</typeparam>
        /// <param name="message">The message event</param>
        /// <returns></returns>
        public static T GetJob<T>(this JobAttemptFaulted message)
        {
            if (message == null)
                throw new ArgumentNullException(nameof(message));

            return ObjectTypeDeserializer.Deserialize<T>(message.Job);
        }

        /// <summary>
        /// Deserializes the job from the message
        /// </summary>
        /// <typeparam name="T">The job type</typeparam>
        /// <param name="message">The message event</param>
        /// <returns></returns>
        public static T GetJob<T>(this JobAttemptCanceled message)
        {
            if (message == null)
                throw new ArgumentNullException(nameof(message));

            return ObjectTypeDeserializer.Deserialize<T>(message.Job);
        }

        /// <summary>
        /// Deserializes the job from the message
        /// </summary>
        /// <typeparam name="T">The job type</typeparam>
        /// <param name="message">The message event</param>
        /// <returns></returns>
        public static T GetJob<T>(this JobAttemptCompleted message)
        {
            if (message == null)
                throw new ArgumentNullException(nameof(message));

            return ObjectTypeDeserializer.Deserialize<T>(message.Job);
        }

        /// <summary>
        /// Returns the job from the message
        /// </summary>
        /// <typeparam name="TJob"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static TJob GetJob<TJob>(this StartJob source)
            where TJob : class
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            return ObjectTypeDeserializer.Deserialize<TJob>(source.Job);
        }

        /// <summary>
        /// Returns the arguments from the JobCompleted event
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static T GetJob<T>(this JobCompleted source)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            return ObjectTypeDeserializer.Deserialize<T>(source.Job);
        }

        /// <summary>
        /// Returns the result from the JobCompleted event
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static T GetResult<T>(this JobCompleted source)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            return ObjectTypeDeserializer.Deserialize<T>(source.Result);
        }
    }
}
