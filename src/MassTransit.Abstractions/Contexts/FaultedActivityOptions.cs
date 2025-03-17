namespace MassTransit;

using System;
using System.Collections.Generic;


public interface FaultedActivityOptions
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
}
