// Copyright 2007-2018 Chris Patterson, Dru Sellers, Travis Smith, et. al.
//  
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use
// this file except in compliance with the License. You may obtain a copy of the 
// License at 
// 
//     http://www.apache.org/licenses/LICENSE-2.0 
// 
// Unless required by applicable law or agreed to in writing, software distributed
// under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR 
// CONDITIONS OF ANY KIND, either express or implied. See the License for the 
// specific language governing permissions and limitations under the License.
namespace MassTransit.DapperIntegration.Sql
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Text;
    using Dapper;
    using Saga;


    public static class WhereStatementHelper
    {
        public static (string whereStatement, DynamicParameters parameters) GetWhereStatementAndParametersFromExpression<TSaga>(
            Expression<Func<TSaga, bool>> expression)
            where TSaga : class, ISaga
        {
            List<(string, object)> columnsAndValues = SqlExpressionVisitor.CreateFromExpression(expression);
            var parameters = new DynamicParameters();

            if (!columnsAndValues.Any())
            {
                return (string.Empty, parameters);
            }

            var sb = new StringBuilder();
            sb.Append("WHERE");

            var i = 0;
            foreach (var (name, value) in columnsAndValues)
            {
                if (i > 0)
                {
                    sb.Append(" AND");
                }

                var valueName = $"@value{i}";
                sb.Append($" {name} = {valueName}");
                parameters.Add(valueName, value);
                i++;
            }

            return (sb.ToString(), parameters);
        }
    }
}