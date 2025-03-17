namespace MassTransit;

using System;
using System.Collections.Generic;


public interface CompletedActivityOptions
{
    /// <summary>
    /// When specified, uses the scheduler to delay execution of the next activity by the specified duration
    /// </summary>
    TimeSpan? Delay { set; }

    /// <summary>
    /// Add or update the variables on the routing slip with the specified object (properties are mapped to variables)
    /// </summary>
    /// <param name="variables"></param>
    void SetVariables(object variables);

    /// <summary>
    /// Add or update the variables on the routing slip with the specified values
    /// </summary>
    /// <param name="variables"></param>
    void SetVariables(IEnumerable<KeyValuePair<string, object>> variables);

    /// <summary>
    /// Add or update the variable on the routing slip with the specified object
    /// </summary>
    /// <param name="key"></param>
    /// <param name="value"></param>
    void SetVariable(string key, object value);

    /// <summary>
    /// Set the log data for compensation
    /// </summary>
    /// <param name="log">The log output to serialize and store in the routing slip for compensation</param>
    void SetLog<TLog>(TLog log)
        where TLog : class;

    /// <summary>
    /// Set the log data for compensation
    /// </summary>
    /// <param name="values">An object to convert to a dictionary for the log data</param>
    void SetLog(object values);

    /// <summary>
    /// Set the log data for compensation using a collection of string/object pairs
    /// </summary>
    /// <param name="values"></param>
    void SetLog(IEnumerable<KeyValuePair<string, object>> values);
}
