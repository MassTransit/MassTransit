namespace MassTransit.EntityFrameworkCoreIntegration
{
    using System.Text;


    public class MySqlLockStatementFormatter :
        ILockStatementFormatter
    {
        public void Create(StringBuilder sb, string schema, string table)
        {
            sb.AppendFormat("SELECT * FROM `{0}` WHERE ", table);
        }

        public void AppendColumn(StringBuilder sb, int index, string columnName)
        {
            if (index == 0)
                sb.AppendFormat("`{0}` = @p0", columnName);
            else
                sb.AppendFormat(" AND `{0}` = @p{1}", columnName, index);
        }

        public void Complete(StringBuilder sb)
        {
            sb.Append(" FOR UPDATE");
        }

        public void CreateOutboxStatement(StringBuilder sb, string schema, string table, string columnName)
        {
            sb.AppendFormat(@"SELECT * FROM `{0}` ORDER BY `{1}` LIMIT 1 FOR UPDATE SKIP LOCKED", table, columnName);
        }
    }
}
