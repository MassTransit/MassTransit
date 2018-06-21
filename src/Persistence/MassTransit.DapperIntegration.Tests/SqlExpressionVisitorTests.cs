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
    using System.Linq;
    using System.Linq.Expressions;
    using NUnit.Framework;
    using Sql;


    public class SqlExpressionVisitorTests
    {
        [Test]
        public void CreateFromExpression_CanHandleEqualNodes_WithConstantValues()
        {
            // Arrange
            Expression<Func<SimpleSaga, bool>> filter = x => x.CorrelateBySomething == "Fiskbullar";

            // Act
            var result = SqlExpressionVisitor.CreateFromExpression(filter).Single();

            // Assert
            Assert.That(result.Item1, Is.EqualTo(nameof(SimpleSaga.CorrelateBySomething)));
            Assert.That(result.Item2, Is.EqualTo("Fiskbullar"));
        }

        [Test]
        public void CreateFromExpression_CanHandleEqualNodes_WithBool()
        {
            // Arrange
            Expression<Func<SimpleSaga, bool>> filter = x => x.Completed;

            // Act
            var result = SqlExpressionVisitor.CreateFromExpression(filter).Single();

            // Assert
            Assert.That(result.Item1, Is.EqualTo(nameof(SimpleSaga.Completed)));
            Assert.That(result.Item2, Is.True);
        }

        [Test]
        public void CreateFromExpression_CanHandleEqualNodes_WithNonConstantValues()
        {
            // Arrange
            var sagaId = NewId.NextGuid();
            Expression<Func<SimpleSaga, bool>> filter = x => x.CorrelationId == sagaId;

            // Act
            var result = SqlExpressionVisitor.CreateFromExpression(filter).Single();

            // Assert
            Assert.That(result.Item1, Is.EqualTo(nameof(SimpleSaga.CorrelationId)));
            Assert.That(result.Item2, Is.EqualTo(sagaId));
        }

        [Test]
        public void CreateFromExpression_CanHandleAndAlsoNodes_WithNonConstantValues_AndBools()
        {
            // Arrange
            var sagaId = NewId.NextGuid();
            Expression<Func<SimpleSaga, bool>> filter = x => x.CorrelationId == sagaId && x.Completed;

            // Act
            var result = SqlExpressionVisitor.CreateFromExpression(filter);

            // Assert
            Assert.That(result.Count, Is.EqualTo(2));

            var first = result.First();
            Assert.That(first.Item1, Is.EqualTo(nameof(SimpleSaga.CorrelationId)));
            Assert.That(first.Item2, Is.EqualTo(sagaId));

            var last = result.Last();
            Assert.That(last.Item1, Is.EqualTo(nameof(SimpleSaga.Completed)));
            Assert.That(last.Item2, Is.True);
        }

        [Test]
        public void CreateFromExpression_CanHandleAndAlsoNodes_WithNestedAndAlso()
        {
            // Arrange
            var sagaId = NewId.NextGuid();
            Expression<Func<SimpleSaga, bool>> filter = x => x.CorrelationId == sagaId && x.Completed && x.CorrelateBySomething == "Kebabsvarv";

            // Act
            var result = SqlExpressionVisitor.CreateFromExpression(filter);

            // Assert
            Assert.That(result.Count, Is.EqualTo(3));

            var first = result[0];
            Assert.That(first.Item1, Is.EqualTo(nameof(SimpleSaga.CorrelationId)));
            Assert.That(first.Item2, Is.EqualTo(sagaId));

            var second = result[1];
            Assert.That(second.Item1, Is.EqualTo(nameof(SimpleSaga.Completed)));
            Assert.That(second.Item2, Is.True);

            var third = result[2];
            Assert.That(third.Item1, Is.EqualTo(nameof(SimpleSaga.CorrelateBySomething)));
            Assert.That(third.Item2, Is.EqualTo("Kebabsvarv"));
        }
    }
}