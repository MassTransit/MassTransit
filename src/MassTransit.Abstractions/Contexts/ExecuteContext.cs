namespace MassTransit
{
    using System;
    using System.Collections.Generic;
    using Courier;


    public interface ExecuteContext :
        CourierContext
    {
        /// <summary>
        /// Set the execution result, which completes the activity
        /// </summary>
        ExecutionResult Result { get; set; }

        /// <summary>
        /// Completes the execution, without passing a compensating log entry
        /// </summary>
        /// <returns></returns>
        ExecutionResult Completed();

        /// <summary>
        /// Completes the execution, passing updated variables to the routing slip
        /// </summary>
        /// <param name="variables"></param>
        /// <returns></returns>
        ExecutionResult CompletedWithVariables(IEnumerable<KeyValuePair<string, object>> variables);

        /// <summary>
        /// Completes the execution, passing updated variables to the routing slip
        /// </summary>
        /// <param name="variables"></param>
        /// <returns></returns>
        ExecutionResult CompletedWithVariables(object variables);

        /// <summary>
        /// Completes the activity, passing a compensation log entry
        /// </summary>
        /// <typeparam name="TLog"></typeparam>
        /// <param name="log"></param>
        /// <returns></returns>
        ExecutionResult Completed<TLog>(TLog log)
            where TLog : class;

        /// <summary>
        /// Completes the activity, passing a compensation log entry
        /// </summary>
        /// <typeparam name="TLog"></typeparam>
        /// <param name="logValues">An object to initialize the log properties</param>
        /// <returns></returns>
        ExecutionResult Completed<TLog>(object logValues)
            where TLog : class;

        /// <summary>
        /// Completes the activity, passing a compensation log entry and additional variables to set on
        /// the routing slip
        /// </summary>
        /// <typeparam name="TLog"></typeparam>
        /// <param name="log"></param>
        /// <param name="variables">An anonymous object of values to add/set as variables on the routing slip</param>
        /// <returns></returns>
        ExecutionResult CompletedWithVariables<TLog>(TLog log, object variables)
            where TLog : class;

        /// <summary>
        /// Completes the activity, passing a compensation log entry and additional variables to set on
        /// the routing slip
        /// </summary>
        /// <typeparam name="TLog"></typeparam>
        /// <param name="logValues"></param>
        /// <param name="variables">An anonymous object of values to add/set as variables on the routing slip</param>
        /// <returns></returns>
        ExecutionResult CompletedWithVariables<TLog>(object logValues, object variables)
            where TLog : class;

        /// <summary>
        /// Completes the activity, passing a compensation log entry and additional variables to set on
        /// the routing slip
        /// </summary>
        /// <typeparam name="TLog"></typeparam>
        /// <param name="log"></param>
        /// <param name="variables">An dictionary of values to add/set as variables on the routing slip</param>
        /// <returns></returns>
        ExecutionResult CompletedWithVariables<TLog>(TLog log, IEnumerable<KeyValuePair<string, object>> variables)
            where TLog : class;

        ExecutionResult ReviseItinerary(Action<IItineraryBuilder> buildItinerary);

        ExecutionResult ReviseItinerary<TLog>(TLog log, Action<IItineraryBuilder> buildItinerary)
            where TLog : class;

        ExecutionResult ReviseItinerary<TLog>(TLog log, object variables, Action<IItineraryBuilder> buildItinerary)
            where TLog : class;

        ExecutionResult ReviseItinerary<TLog>(TLog log, IEnumerable<KeyValuePair<string, object>> variables, Action<IItineraryBuilder> buildItinerary)
            where TLog : class;

        /// <summary>
        /// Terminate the routing slip (with extreme prejudice), completing it but discarding any remaining itinerary
        /// activities.
        /// </summary>
        ExecutionResult Terminate();

        /// <summary>
        /// Terminate the routing slip (with extreme prejudice), completing it but discarding any remaining itinerary
        /// activities.
        /// <param name="variables">An dictionary of values to add/set as variables on the routing slip</param>
        /// </summary>
        ExecutionResult Terminate(object variables);

        /// <summary>
        /// Terminate the routing slip (with extreme prejudice), completing it but discarding any remaining itinerary
        /// activities.
        /// <param name="variables">An dictionary of values to add/set as variables on the routing slip</param>
        /// </summary>
        ExecutionResult Terminate(IEnumerable<KeyValuePair<string, object>> variables);

        /// <summary>
        /// The activity Faulted for an unknown reason, but compensation should be triggered
        /// </summary>
        /// <returns></returns>
        ExecutionResult Faulted();

        /// <summary>
        /// The activity Faulted, and compensation should be triggered
        /// </summary>
        /// <param name="exception"></param>
        /// <returns></returns>
        ExecutionResult Faulted(Exception exception);

        /// <summary>
        /// The activity Faulted with no exception, but compensation should be triggered and passing additional variables to set on
        /// the routing slip
        /// </summary>
        /// <param name="exception"></param>
        /// <param name="variables">An anonymous object of values to add/set as variables on the routing slip</param>
        /// <returns></returns>
        ExecutionResult FaultedWithVariables(Exception exception, object variables);

        /// <summary>
        /// The activity Faulted with no exception, but compensation should be triggered and passing additional variables to set on
        /// the routing slip
        /// </summary>
        /// <param name="exception"></param>
        /// <param name="variables">An dictionary of values to add/set as variables on the routing slip</param>
        /// <returns></returns>
        ExecutionResult FaultedWithVariables(Exception exception, IEnumerable<KeyValuePair<string, object>> variables);
    }


    public interface ExecuteContext<out TArguments> :
        ExecuteContext
        where TArguments : class
    {
        /// <summary>
        /// The arguments from the routing slip for this activity
        /// </summary>
        TArguments Arguments { get; }

        ExecuteActivityContext<TActivity, TArguments> CreateActivityContext<TActivity>(TActivity activity)
            where TActivity : class, IExecuteActivity<TArguments>;
    }
}
