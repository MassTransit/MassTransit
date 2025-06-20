#nullable enable
namespace MassTransit.DapperIntegration.SqlBuilders
{
    using System;
    using System.Linq.Expressions;


    public interface ISagaSqlFormatter<TModel> where TModel : class
    {
        string BuildInsertSql();
        string BuildUpdateSql();
        string BuildDeleteSql();
        string BuildLoadSql();
        string BuildQuerySql(Expression<Func<TModel, bool>> filterExpression, Action<string, object> parameterCallback);

        void MapPrefix<TProperty>(Expression<Func<TModel, TProperty>> mappingExpression, string? prefixName = null);
        void MapProperty<TProperty>(Expression<Func<TModel, TProperty>> mappingExpression, string targetName);
    }
}
#nullable restore
