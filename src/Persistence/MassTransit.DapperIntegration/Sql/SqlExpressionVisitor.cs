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
    using System.Linq.Expressions;
    using Internals.Reflection;


    public static class SqlExpressionVisitor
    {
        public static List<(string, object)> CreateFromExpression(Expression node)
        {
            switch (node.NodeType)
            {
                case ExpressionType.Lambda:
                    return LambdaVisit((LambdaExpression)node);
                case ExpressionType.AndAlso:
                    return AndAlsoVisit((BinaryExpression)node);
                case ExpressionType.Equal:
                    return EqualVisit((BinaryExpression)node);
                case ExpressionType.MemberAccess:
                    return MemberAccessVisit((MemberExpression)node);
                default:
                    throw new Exception("Node type not supported.");
            }
        }

        static List<(string, object)> LambdaVisit(LambdaExpression node)
        {
            return CreateFromExpression(node.Body);
        }

        static List<(string, object)> AndAlsoVisit(BinaryExpression node)
        {
            var result = new List<(string, object)>();
            List<(string, object)> leftResult = CreateFromExpression(node.Left);
            result.AddRange(leftResult);

            List<(string, object)> rightResult = CreateFromExpression(node.Right);
            result.AddRange(rightResult);

            return result;
        }

        static List<(string, object)> EqualVisit(BinaryExpression node)
        {
            var left = (MemberExpression)node.Left;

            var name = left.Member.Name;

            object value;
            if (node.Right is ConstantExpression right)
            {
                value = right.Value;
            }
            else
            {
                value = Expression.Lambda<Func<object>>(
                    Expression.Convert(node.Right, typeof(object))).CompileFast().Invoke();
            }

            return new List<(string, object)> {(name, value)};
        }

        static List<(string, object)> MemberAccessVisit(MemberExpression node)
        {
            var name = node.Member.Name;
            object value;

            if (node.Type == typeof(bool))
            {
                value = true; // No support for Not yet.
            }
            else if (node.Type.IsValueType) 
            {
                value = Activator.CreateInstance(node.Type);
            }
            else
            {
                value = null;
            }

            return new List<(string, object)> { (name, value) };
        }
    }
}