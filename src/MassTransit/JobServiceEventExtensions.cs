namespace MassTransit
{
    using System;
    using Contracts.JobService;
    using Util;


    public static class JobServiceEventExtensions
    {
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

        public static TJob GetJob<TJob>(this FaultJob source)
            where TJob : class
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            return ObjectTypeDeserializer.Deserialize<TJob>(source.Job);
        }

        /// <summary>
        /// Returns the job from the JobCompleted event
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

        /// <summary>
        /// Returns the job from the JobFaulted event
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static T GetJob<T>(this JobFaulted source)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            return ObjectTypeDeserializer.Deserialize<T>(source.Job);
        }
    }
}
