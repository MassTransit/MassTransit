namespace MassTransit.DapperIntegration.Tests.ComponentTests
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using Legacy;
    using NUnit.Framework;
    using Saga;
    using SqlBuilders;


    public class WhereStatementHelperTests
    {
        [Test]
        public void GetWhereStatementAndParametersFromExpression_HandlesSingleValues()
        {
            // Arrange
            var sagaId = NewId.NextGuid();
            var parameters = new Dictionary<string, object>();
            Expression<Func<SimpleSaga, bool>> filter = x => x.CorrelationId == sagaId;

            // Act
            var predicates = SqlExpressionVisitor.CreateFromExpression(filter);
            var whereStatement = SqlServerSagaFormatter<SimpleSaga>.BuildQueryPredicate(predicates, (k, v) => parameters.Add(k, v));

            // Assert
            Assert.That(whereStatement, Is.EqualTo("[CorrelationId] = @value0"));
            Assert.That(parameters, Has.Exactly(1).Items);
            Assert.That(parameters, Contains.Key("value0").WithValue(sagaId));
        }

        [Test]
        public void GetWhereStatementAndParametersFromExpression_HandlesMultipleValues()
        {
            // Arrange
            var sagaId = NewId.NextGuid();
            var parameters = new Dictionary<string, object>();
            Expression<Func<SimpleSaga, bool>> filter = x => x.CorrelationId == sagaId && x.Completed && x.CorrelateBySomething == "Kebabsvarv";

            // Act
            var predicates = SqlExpressionVisitor.CreateFromExpression(filter);
            var whereStatement = SqlServerSagaFormatter<SimpleSaga>.BuildQueryPredicate(predicates, (k, v) => parameters.Add(k, v));

            // Assert
            Assert.That(whereStatement, Is.EqualTo("[CorrelationId] = @value0 AND [Completed] = @value1 AND [CorrelateBySomething] = @value2"));
            Assert.That(parameters, Has.Exactly(3).Items);
            Assert.That(parameters, Contains.Key("value0").WithValue(sagaId));
            Assert.That(parameters, Contains.Key("value1").WithValue(true));
            Assert.That(parameters, Contains.Key("value2").WithValue("Kebabsvarv"));
        }
    }
}
