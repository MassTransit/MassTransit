namespace MassTransit.DapperIntegration.Saga
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Text;
    using Dapper;


    public static class WhereStatementHelper
    {
        public static (string whereStatement, DynamicParameters parameters) GetWhereStatementAndParametersFromExpression<TSaga>(
            Expression<Func<TSaga, bool>> expression)
            where TSaga : class, ISaga
        {
            List<(string, object)> columnsAndValues = SqlExpressionVisitor.CreateFromExpression(expression);
            var parameters = new DynamicParameters();

            if (!columnsAndValues.Any())
                return (string.Empty, parameters);

            var sb = new StringBuilder();
            sb.Append("WHERE");

            var i = 0;
            foreach (var (name, value) in columnsAndValues)
            {
                if (i > 0)
                    sb.Append(" AND");

                var valueName = $"@value{i}";
                sb.Append($" {name} = {valueName}");
                parameters.Add(valueName, value);
                i++;
            }

            return (sb.ToString(), parameters);
        }
    }
}
