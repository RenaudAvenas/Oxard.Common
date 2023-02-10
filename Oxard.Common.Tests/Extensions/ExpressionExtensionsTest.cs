using System;
using System.Linq.Expressions;
using Oxard.Common.Extensions;
using NFluent;
using NUnit.Framework;

namespace Oxard.Common.Tests.Extensions
{
    [TestFixture]
    public class ExpressionExtensionsTest
    {
        [Test]
        public void WhenPropertyExpressionGetPropertyNameThenPropertyNameIsReturned()
        {
            var fakeForExpression = new FakeForExpression();
            Expression<Func<int>> expression = () => fakeForExpression.Value;
            Check.That(expression.GetPropertyName()).Equals(nameof(fakeForExpression.Value));
        }

        [Test]
        public void WhenPropertyExpressionIsNullAndGetPropertyNameThenException()
        {
            Expression<Func<int>> expression = null;
            Check.ThatCode(() => expression.GetPropertyName()).Throws<ArgumentNullException>();
        }

        [Test]
        public void WhenExpressionIsNotPropertyExpressionAndGetPropertyNameThenException()
        {
            var fakeForExpression = new FakeForExpression();
            Expression<Func<int>> expression = () => fakeForExpression.Method();
            Check.ThatCode(() => expression.GetPropertyName()).Throws<ArgumentException>();
        }

        [Test]
        public void WhenExpressionIsFieldPropertyExpressionAndGetPropertyNameThenException()
        {
            var fakeForExpression = new FakeForExpression();
            Expression<Func<int>> expression = () => fakeForExpression.FieldValue;
            Check.ThatCode(() => expression.GetPropertyName()).Throws<ArgumentException>();
        }

        [Test]
        public void WhenPropertyExpressionGetPropertyValueThenValueIsReturned()
        {
            var fakeForExpression = new FakeForExpression { Value = 3 };
            Expression<Func<int>> expression = () => fakeForExpression.Value;
            Check.That(expression.GetPropertyValue()).IsEqualTo(3);
        }

        private class FakeForExpression
        {
            public int FieldValue;

            public int Value { get; set; }

            public int Method()
            {
                return this.Value;
            }
        }
    }
}