namespace MassTransit.Internals.Extensions
{
    using System;
    using System.Data;


    public static class DbCommandExtensions
    {
        public static IDbDataParameter CreateParameter<T>(this IDbCommand command, T valueholder,
            Func<T, object> fieldSelector,
            string parameterName)
        {
            IDbDataParameter parameter = command.CreateParameter();

            parameter.ParameterName = parameterName;
            parameter.Value = fieldSelector(valueholder);

            command.Parameters.Add(parameter);

            return parameter;
        }
    }
}