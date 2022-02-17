namespace MassTransit
{
    using System;
    using System.Collections.Generic;


    public interface CompensateContext :
        CourierContext
    {
        /// <summary>
        /// Set the compensation result, which completes the activity
        /// </summary>
        CompensationResult Result { get; set; }

        /// <summary>
        /// The compensation was successful
        /// </summary>
        /// <returns></returns>
        CompensationResult Compensated();

        /// <summary>
        /// The compensation was successful
        /// </summary>
        /// <param name="values">The variables to be updated on the routing slip</param>
        /// <returns></returns>
        CompensationResult Compensated(object values);

        /// <summary>
        /// The compensation was successful
        /// </summary>
        /// <param name="variables">The variables to be updated on the routing slip</param>
        /// <returns></returns>
        CompensationResult Compensated(IDictionary<string, object> variables);

        /// <summary>
        /// The compensation failed
        /// </summary>
        /// <returns></returns>
        CompensationResult Failed();

        /// <summary>
        /// The compensation failed with the specified exception
        /// </summary>
        /// <param name="exception"></param>
        /// <returns></returns>
        CompensationResult Failed(Exception exception);
    }


    public interface CompensateContext<out TLog> :
        CompensateContext
        where TLog : class
    {
        /// <summary>
        /// The execution log from the activity execution
        /// </summary>
        TLog Log { get; }

        CompensateActivityContext<TActivity, TLog> CreateActivityContext<TActivity>(TActivity activity)
            where TActivity : class, ICompensateActivity<TLog>;
    }
}
