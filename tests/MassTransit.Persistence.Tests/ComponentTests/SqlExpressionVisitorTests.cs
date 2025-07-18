namespace MassTransit.Persistence.Tests.ComponentTests
{
    using System.Linq.Expressions;
    using Integration.Saga;
    using NUnit.Framework;


    public class SqlExpressionVisitorTests
    {
        [Test]
        public void CreateFromExpression_CanHandleEqualNodes_WithConstantValues()
        {
            // Arrange
            Expression<Func<SimpleSaga, bool>> filter = x => x.CorrelateBySomething == "Fiskbullar";

            // Act
            var result = SqlExpressionVisitor.CreateFromExpression(filter).Single();

            Assert.Multiple(() =>
            {
                // Assert
                Assert.That(result.Name, Is.EqualTo(nameof(SimpleSaga.CorrelateBySomething)));
                Assert.That(result.Value, Is.EqualTo("Fiskbullar"));
            });
        }

        [Test]
        public void CreateFromExpression_CanHandleEqualNodes_WithBool()
        {
            // Arrange
            Expression<Func<SimpleSaga, bool>> filter = x => x.Completed;

            // Act
            var result = SqlExpressionVisitor.CreateFromExpression(filter).Single();

            Assert.Multiple(() =>
            {
                // Assert
                Assert.That(result.Name, Is.EqualTo(nameof(SimpleSaga.Completed)));
                Assert.That(result.Value, Is.True);
            });
        }

        [Test]
        public void CreateFromExpression_CanHandleEqualNodes_WithNonConstantValues()
        {
            // Arrange
            var sagaId = NewId.NextGuid();
            Expression<Func<SimpleSaga, bool>> filter = x => x.CorrelationId == sagaId;

            // Act
            var result = SqlExpressionVisitor.CreateFromExpression(filter).Single();

            Assert.Multiple(() =>
            {
                // Assert
                Assert.That(result.Name, Is.EqualTo(nameof(SimpleSaga.CorrelationId)));
                Assert.That(result.Value, Is.EqualTo(sagaId));
            });
        }

        [Test]
        public void CreateFromExpression_CanHandleAndAlsoNodes_WithNonConstantValues_AndBools()
        {
            // Arrange
            var sagaId = NewId.NextGuid();
            Expression<Func<SimpleSaga, bool>> filter = x => x.CorrelationId == sagaId && x.Completed;

            // Act
            List<SqlPredicate>? result = SqlExpressionVisitor.CreateFromExpression(filter);

            // Assert
            Assert.That(result, Has.Count.EqualTo(2));

            var first = result.First();
            Assert.Multiple(() =>
            {
                Assert.That(first.Name, Is.EqualTo(nameof(SimpleSaga.CorrelationId)));
                Assert.That(first.Value, Is.EqualTo(sagaId));
            });

            var last = result.Last();
            Assert.Multiple(() =>
            {
                Assert.That(last.Name, Is.EqualTo(nameof(SimpleSaga.Completed)));
                Assert.That(last.Value, Is.True);
            });
        }

        [Test]
        public void CreateFromExpression_CanHandleNotTrue()
        {
            // Arrange
            var sagaId = NewId.NextGuid();
            Expression<Func<SimpleSaga, bool>> filter = x => x.Completed != true;

            // Act
            List<SqlPredicate>? result = SqlExpressionVisitor.CreateFromExpression(filter);

            // Assert
            Assert.That(result, Has.Count.EqualTo(1));

            var first = result.First();

            Assert.Multiple(() =>
            {
                Assert.That(first.Name, Is.EqualTo(nameof(SimpleSaga.Completed)));
                Assert.That(first.Operator, Is.EqualTo("<>"));
                Assert.That(first.Value, Is.True);
            });
        }

        [Test]
        public void CreateFromExpression_CanHandleInvertedBool()
        {
            // Arrange
            var sagaId = NewId.NextGuid();
            Expression<Func<SimpleSaga, bool>> filter = x => !x.Completed;

            // Act
            List<SqlPredicate>? result = SqlExpressionVisitor.CreateFromExpression(filter);

            // Assert
            Assert.That(result, Has.Count.EqualTo(1));

            var first = result.First();

            Assert.Multiple(() =>
            {
                Assert.That(first.Name, Is.EqualTo(nameof(SimpleSaga.Completed)));
                Assert.That(first.Operator, Is.EqualTo("="));
                Assert.That(first.Value, Is.False);
            });
        }

        [Test]
        public void CreateFromExpression_CanHandleAndAlsoNodes_WithNestedAndAlso()
        {
            // Arrange
            var sagaId = NewId.NextGuid();
            Expression<Func<SimpleSaga, bool>> filter = x => x.CorrelationId == sagaId && x.Completed && x.CorrelateBySomething == "Kebabsvarv";

            // Act
            List<SqlPredicate>? result = SqlExpressionVisitor.CreateFromExpression(filter);

            // Assert
            Assert.That(result, Has.Count.EqualTo(3));

            var first = result[0];
            Assert.Multiple(() =>
            {
                Assert.That(first.Name, Is.EqualTo(nameof(SimpleSaga.CorrelationId)));
                Assert.That(first.Value, Is.EqualTo(sagaId));
            });

            var second = result[1];
            Assert.Multiple(() =>
            {
                Assert.That(second.Name, Is.EqualTo(nameof(SimpleSaga.Completed)));
                Assert.That(second.Value, Is.True);
            });

            var third = result[2];
            Assert.Multiple(() =>
            {
                Assert.That(third.Name, Is.EqualTo(nameof(SimpleSaga.CorrelateBySomething)));
                Assert.That(third.Value, Is.EqualTo("Kebabsvarv"));
            });
        }
    }
}
