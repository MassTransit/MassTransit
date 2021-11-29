namespace MassTransit.SagaStateMachine
{
    using System;
    using System.Linq.Expressions;
    using Internals;


    public class EventCorrelationExpressionConverter<TInstance, TMessage> :
        ExpressionVisitor
        where TInstance : class, SagaStateMachineInstance
        where TMessage : class
    {
        readonly ConsumeContext<TMessage> _context;

        public EventCorrelationExpressionConverter(ConsumeContext<TMessage> context)
        {
            _context = context;
        }

        public Expression<Func<TInstance, bool>> Convert(Expression<Func<TInstance, ConsumeContext<TMessage>, bool>> expression)
        {
            var result = Visit(expression);

            return RemoveMessageParameter(result as LambdaExpression);
        }

        static Expression<Func<TInstance, bool>> RemoveMessageParameter(LambdaExpression lambda)
        {
            ParameterExpression[] parameters = { lambda.Parameters[0] };

            return Expression.Lambda<Func<TInstance, bool>>(lambda.Body, parameters);
        }

        protected override Expression VisitMember(MemberExpression m)
        {
            if (m.Expression == null)
                return base.VisitMember(m);

            if (m.Expression.NodeType == ExpressionType.Parameter && m.Expression.Type == typeof(ConsumeContext<TMessage>))
                return EvaluateConsumeContextAccess(m);

            return base.VisitMember(m);
        }

        Expression EvaluateConsumeContextAccess(MemberExpression exp)
        {
            var parameter = exp.Expression as ParameterExpression;

            var fn = Expression.Lambda(typeof(Func<,>).MakeGenericType(typeof(ConsumeContext<TMessage>), exp.Type), exp, parameter).CompileFast();

            return Expression.Constant(fn.DynamicInvoke(_context), exp.Type);
        }
    }
}
