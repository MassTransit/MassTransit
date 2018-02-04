namespace MassTransit.Initializers
{
    using System;
    using System.Threading.Tasks;


    public static class TaskInitializerExtensions
    {
        /// <summary>
        /// Awaits the task and calls the selector to return a string property of the result
        /// </summary>
        /// <param name="task"></param>
        /// <param name="selector"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static async Task<string> Select<T>(this Task<T> task, Func<T, string> selector)
        {
            if (task == null)
                throw new ArgumentNullException(nameof(task));

            if (selector == null)
                throw new ArgumentNullException(nameof(selector));

            var taskResult = await task.ConfigureAwait(false);
            if (taskResult != null)
                return selector(taskResult);

            return default;
        }

        /// <summary>
        /// Awaits the task and calls the selector to return a string property of the result
        /// </summary>
        /// <param name="task"></param>
        /// <param name="selector"></param>
        /// <param name="defaultValue"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static async Task<string> Select<T>(this Task<T> task, Func<T, string> selector, string defaultValue)
        {
            if (task == null)
                throw new ArgumentNullException(nameof(task));

            if (selector == null)
                throw new ArgumentNullException(nameof(selector));

            var taskResult = await task.ConfigureAwait(false);
            if (taskResult != null)
                return selector(taskResult) ?? defaultValue;

            return defaultValue;
        }

        /// <summary>
        /// Awaits the task and calls the selector to return a string property of the result
        /// </summary>
        /// <param name="task"></param>
        /// <param name="selector"></param>
        /// <param name="getDefaultValue"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static async Task<string> Select<T>(this Task<T> task, Func<T, string> selector, Func<string> getDefaultValue)
        {
            if (task == null)
                throw new ArgumentNullException(nameof(task));

            if (selector == null)
                throw new ArgumentNullException(nameof(selector));

            if (getDefaultValue == null)
                throw new ArgumentNullException(nameof(getDefaultValue));

            string result = default;
            var taskResult = await task.ConfigureAwait(false);
            if (taskResult != null)
                result = selector(taskResult);

            return result ?? getDefaultValue();
        }

        /// <summary>
        /// Awaits the task and calls the selector to return a string property of the result
        /// </summary>
        /// <param name="task"></param>
        /// <param name="selector"></param>
        /// <param name="getDefaultValue"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static async Task<string> Select<T>(this Task<T> task, Func<T, string> selector, Func<Task<string>> getDefaultValue)
        {
            if (task == null)
                throw new ArgumentNullException(nameof(task));

            if (selector == null)
                throw new ArgumentNullException(nameof(selector));

            if (getDefaultValue == null)
                throw new ArgumentNullException(nameof(getDefaultValue));

            string result = default;
            var taskResult = await task.ConfigureAwait(false);
            if (taskResult != null)
                result = selector(taskResult);

            return result ?? await getDefaultValue().ConfigureAwait(false);
        }

        /// <summary>
        /// Awaits the task and calls the selector to return a string property of the result
        /// </summary>
        /// <param name="task"></param>
        /// <param name="selector"></param>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <returns></returns>
        public static async Task<TResult> Select<T, TResult>(this Task<T> task, Func<T, TResult> selector)
        {
            if (task == null)
                throw new ArgumentNullException(nameof(task));

            if (selector == null)
                throw new ArgumentNullException(nameof(selector));

            var taskResult = await task.ConfigureAwait(false);
            if (taskResult != null)
                return selector(taskResult);

            return default;
        }

        /// <summary>
        /// Awaits the task and calls the selector to return a TResult property of the result
        /// </summary>
        /// <param name="task"></param>
        /// <param name="selector"></param>
        /// <param name="defaultValue"></param>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <returns></returns>
        public static async Task<TResult> Select<T, TResult>(this Task<T> task, Func<T, TResult> selector, TResult defaultValue)
            where TResult : class
        {
            if (task == null)
                throw new ArgumentNullException(nameof(task));

            if (selector == null)
                throw new ArgumentNullException(nameof(selector));

            var taskResult = await task.ConfigureAwait(false);
            if (taskResult != null)
                return selector(taskResult) ?? defaultValue;

            return defaultValue;
        }

        /// <summary>
        /// Awaits the task and calls the selector to return a TResult property of the result
        /// </summary>
        /// <param name="task"></param>
        /// <param name="selector"></param>
        /// <param name="defaultValue"></param>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <returns></returns>
        public static async Task<TResult> Select<T, TResult>(this Task<T> task, Func<T, TResult?> selector, TResult defaultValue)
            where TResult : struct
        {
            if (task == null)
                throw new ArgumentNullException(nameof(task));

            if (selector == null)
                throw new ArgumentNullException(nameof(selector));

            var taskResult = await task.ConfigureAwait(false);
            if (taskResult != null)
                return selector(taskResult) ?? defaultValue;

            return defaultValue;
        }

        /// <summary>
        /// Awaits the task and calls the selector to return a TResult property of the result
        /// </summary>
        /// <param name="task"></param>
        /// <param name="selector"></param>
        /// <param name="getDefaultValue"></param>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <returns></returns>
        public static async Task<TResult> Select<T, TResult>(this Task<T> task, Func<T, TResult> selector, Func<TResult> getDefaultValue)
            where TResult : class
        {
            if (task == null)
                throw new ArgumentNullException(nameof(task));

            if (selector == null)
                throw new ArgumentNullException(nameof(selector));

            if (getDefaultValue == null)
                throw new ArgumentNullException(nameof(getDefaultValue));

            TResult result = default;
            var taskResult = await task.ConfigureAwait(false);
            if (taskResult != null)
                result = selector(taskResult);

            return result ?? getDefaultValue();
        }

        /// <summary>
        /// Awaits the task and calls the selector to return a TResult property of the result
        /// </summary>
        /// <param name="task"></param>
        /// <param name="selector"></param>
        /// <param name="getDefaultValue"></param>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <returns></returns>
        public static async Task<TResult> Select<T, TResult>(this Task<T> task, Func<T, TResult?> selector, Func<TResult> getDefaultValue)
            where TResult : struct
        {
            if (task == null)
                throw new ArgumentNullException(nameof(task));

            if (selector == null)
                throw new ArgumentNullException(nameof(selector));

            if (getDefaultValue == null)
                throw new ArgumentNullException(nameof(getDefaultValue));

            TResult? result = default;
            var taskResult = await task.ConfigureAwait(false);
            if (taskResult != null)
                result = selector(taskResult);

            return result ?? getDefaultValue();
        }

        /// <summary>
        /// Awaits the task and calls the selector to return a TResult property of the result
        /// </summary>
        /// <param name="task"></param>
        /// <param name="selector"></param>
        /// <param name="getDefaultValue"></param>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <returns></returns>
        public static async Task<TResult> Select<T, TResult>(this Task<T> task, Func<T, TResult> selector, Func<Task<TResult>> getDefaultValue)
            where TResult : class
        {
            if (task == null)
                throw new ArgumentNullException(nameof(task));

            if (selector == null)
                throw new ArgumentNullException(nameof(selector));

            if (getDefaultValue == null)
                throw new ArgumentNullException(nameof(getDefaultValue));

            TResult result = default;
            var taskResult = await task.ConfigureAwait(false);
            if (taskResult != null)
                result = selector(taskResult);

            return result ?? await getDefaultValue().ConfigureAwait(false);
        }

        /// <summary>
        /// Awaits the task and calls the selector to return a TResult property of the result
        /// </summary>
        /// <param name="task"></param>
        /// <param name="selector"></param>
        /// <param name="getDefaultValue"></param>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <returns></returns>
        public static async Task<TResult> Select<T, TResult>(this Task<T> task, Func<T, TResult?> selector, Func<Task<TResult>> getDefaultValue)
            where TResult : struct
        {
            if (task == null)
                throw new ArgumentNullException(nameof(task));

            if (selector == null)
                throw new ArgumentNullException(nameof(selector));

            if (getDefaultValue == null)
                throw new ArgumentNullException(nameof(getDefaultValue));

            TResult? result = default;
            var taskResult = await task.ConfigureAwait(false);
            if (taskResult != null)
                result = selector(taskResult);

            return result ?? await getDefaultValue().ConfigureAwait(false);
        }
    }
}
