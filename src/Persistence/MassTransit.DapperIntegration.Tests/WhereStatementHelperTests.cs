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
namespace MassTransit.DapperIntegration.Tests
{
    using System;
    using System.Linq.Expressions;
    using NUnit.Framework;
    using Sql;


    public class WhereStatementHelperTests
    {
        [Test]
        public void GetWhereStatementAndParametersFromExpression_HandlesSingleValues()
        {
            // Arrange
            var sagaId = NewId.NextGuid();
            Expression<Func<SimpleSaga, bool>> filter = x => x.CorrelationId == sagaId;

            // Act
            var(whereStatement, dynamicParameters) = WhereStatementHelper.GetWhereStatementAndParametersFromExpression(filter);

            // Assert
            Assert.That(whereStatement, Is.EqualTo($"WHERE CorrelationId = @value0"));
            var sagaIdParameter = dynamicParameters.Get<Guid>("value0");
            Assert.That(sagaIdParameter, Is.EqualTo(sagaId));
        }

        [Test]
        public void GetWhereStatementAndParametersFromExpression_HandlesMultipleValues()
        {
            // Arrange
            var sagaId = NewId.NextGuid();
            Expression<Func<SimpleSaga, bool>> filter = x => x.CorrelationId == sagaId && x.Completed && x.CorrelateBySomething == "Kebabsvarv";

            // Act
            var (whereStatement, dynamicParameters) = WhereStatementHelper.GetWhereStatementAndParametersFromExpression(filter);

            // Assert
            Assert.That(whereStatement, Is.EqualTo($"WHERE CorrelationId = @value0 AND Completed = @value1 AND CorrelateBySomething = @value2"));

            var sagaIdParameter = dynamicParameters.Get<Guid>("value0");
            Assert.That(sagaIdParameter, Is.EqualTo(sagaId));

            var completedParameter = dynamicParameters.Get<object>("value1");
            Assert.That(completedParameter, Is.True);

            var correlateBySomethingParameter = dynamicParameters.Get<string>("value2");
            Assert.That(correlateBySomethingParameter, Is.EqualTo("Kebabsvarv"));
        }
    }
}