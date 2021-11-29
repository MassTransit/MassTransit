namespace MassTransit.Util
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using Internals;


    /// <summary>
    /// Generates a monospaced text table, useful in trace output formats. Shamelessly inspired by ConsoleTables
    /// https://github.com/khalidabuhakmeh/ConsoleTables
    /// </summary>
    public class TextTable
    {
        static readonly HashSet<Type> NumericTypes = new HashSet<Type>
        {
            typeof(int),
            typeof(double),
            typeof(decimal),
            typeof(long),
            typeof(short),
            typeof(sbyte),
            typeof(byte),
            typeof(ulong),
            typeof(ushort),
            typeof(uint),
            typeof(float)
        };

        readonly IList<object> _columns;
        readonly IList<object[]> _rows;
        Type[] _columnTypes;

        public TextTable(params string[] columns)
            : this(new TextTableOptions { Columns = new List<string>(columns) })
        {
        }

        public TextTable(TextTableOptions options)
        {
            Options = options ?? throw new ArgumentNullException(nameof(options));

            options.Out ??= TextWriter.Null;

            _rows = new List<object[]>();
            _columns = new List<object>(options.Columns);
        }

        public TextTableOptions Options { get; }

        public TextTable AddColumns(params string[] names)
        {
            return AddColumns((IEnumerable<string>)names);
        }

        public TextTable AddColumns(IEnumerable<string> names)
        {
            foreach (var name in names)
                _columns.Add(name);

            return this;
        }

        public TextTable AddRow(params object[] values)
        {
            if (values == null)
                throw new ArgumentNullException(nameof(values));
            if (_columns.Count == 0)
                throw new InvalidOperationException("Columns must be specified before adding rows");
            if (_columns.Count != values.Length)
                throw new ArgumentException(nameof(values), $"Rows must have {_columns.Count} columns, only {values.Length} provided");

            _rows.Add(values);

            return this;
        }

        public TextTable Configure(Action<TextTableOptions> action)
        {
            action(Options);

            return this;
        }

        /// <summary>
        /// Create a table from an existing enumerable collection
        /// </summary>
        /// <param name="rows">The collection</param>
        /// <typeparam name="T">The collection element type</typeparam>
        /// <returns></returns>
        public static TextTable Create<T>(IEnumerable<T> rows)
        {
            IReadOnlyPropertyCache<T> properties = TypeCache<T>.ReadOnlyPropertyCache;

            Type[] columnTypes = properties.Select(x => x.Property.PropertyType).ToArray();
            var columnNames = properties.Select(x => x.Property.Name).ToArray();

            var table = new TextTable(columnNames) { _columnTypes = columnTypes };

            foreach (IEnumerable<object> propertyValues in rows.Select(value => properties.Select(column => column.GetProperty(value))))
                table.AddRow(propertyValues.ToArray());

            return table;
        }

        public override string ToString()
        {
            var builder = new StringBuilder();

            List<int> columnWidths = CalculateColumnWidths();

            List<string> columnAlignment = Enumerable.Range(0, _columns.Count)
                .Select(GetNumberAlignment)
                .ToList();

            var dataRowFormat = "\x2503 " + string.Join(" \x2502 ", Enumerable.Range(0, _columns.Count)
                .Select(x => "{" + x + "," + columnAlignment[x] + columnWidths[x] + "}")) + " \x2503";

            var columnHeaders = string.Format(dataRowFormat, _columns.ToArray());

            List<string> formattedRows = _rows.Select(row => string.Format(dataRowFormat, row)).ToList();

            var rowSeparator = "\x2520\x2500" + string.Join("\x2500\x253C\x2500", Enumerable.Range(0, _columns.Count)
                .Select(x => new string('\x2500', columnWidths[x]))) + "\x2500\x2528";
            var top = "\x250F\x2501" + string.Join("\x2501\x252F\x2501", Enumerable.Range(0, _columns.Count)
                .Select(x => new string('\x2501', columnWidths[x]))) + "\x2501\x2513";
            var bottom = "\x2517\x2501" + string.Join("\x2501\x2537\x2501", Enumerable.Range(0, _columns.Count)
                .Select(x => new string('\x2501', columnWidths[x]))) + "\x2501\x251B";

            builder.AppendLine(top);
            builder.AppendLine(columnHeaders);

            foreach (var row in formattedRows)
            {
                builder.AppendLine(rowSeparator);
                builder.AppendLine(row);
            }

            builder.AppendLine(bottom);

            if (Options.EnableCount)
            {
                builder.AppendLine("");
                builder.AppendFormat("Count: {0}", _rows.Count);
            }

            return builder.ToString();
        }

        string GetNumberAlignment(int column)
        {
            return Options.NumberAlignment == NumberAlignment.Right
                && _columnTypes != null
                && NumericTypes.Contains(_columnTypes[column])
                    ? ""
                    : "-";
        }

        List<int> CalculateColumnWidths()
        {
            List<int> columnLengths = _columns
                .Select((t, i) => _rows.Select(x => x[i])
                    .Union(new[] { _columns[i] })
                    .Where(x => x != null)
                    .Select(x => x.ToString().Length).Max())
                .ToList();
            return columnLengths;
        }

        public void Write()
        {
            Options.Out.WriteLine(ToString());
        }

        public TextTable SetColumn(int column, string name, Type columnType = default)
        {
            if (column < 0 || column >= _columns.Count)
                throw new ArgumentOutOfRangeException(nameof(column));
            if (name == null)
                throw new ArgumentNullException(nameof(name));

            _columns[column] = name;

            if (columnType != default && _columnTypes != null)
                _columnTypes[column] = columnType;

            return this;
        }

        public TextTable EnableCount(bool enabled)
        {
            Options.EnableCount = enabled;
            return this;
        }

        public TextTable SetRightNumberAlignment()
        {
            Options.NumberAlignment = NumberAlignment.Right;
            return this;
        }

        public TextTable OutputTo(TextWriter textWriter)
        {
            Options.Out = textWriter ?? TextWriter.Null;
            return this;
        }
    }


    public class TextTableOptions
    {
        /// <summary>
        /// The column names
        /// </summary>
        public IEnumerable<string> Columns { get; set; } = new List<string>();

        /// <summary>
        /// Include the row count at the end of the table
        /// </summary>
        public bool EnableCount { get; set; }

        /// <summary>
        /// Specify the number alignment (defaults to left)
        /// </summary>
        public NumberAlignment NumberAlignment { get; set; } = NumberAlignment.Left;

        /// <summary>
        /// The <see cref="System.IO.TextWriter" /> to write to. Defaults to <see cref="System.Console.Out" />.
        /// </summary>
        public TextWriter Out { get; set; } = Console.Out;
    }


    public enum NumberAlignment
    {
        Left,
        Right
    }
}
