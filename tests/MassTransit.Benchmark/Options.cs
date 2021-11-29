#if LINQ
using System.Linq;
#endif

#if TEST
using NDesk.Options;
#endif

namespace NDesk.Options
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.IO;
    using System.Runtime.Serialization;
    using System.Security.Permissions;
    using System.Text;
    using System.Text.RegularExpressions;


    public class OptionValueCollection : IList,
        IList<string>
    {
        OptionContext c;

        List<string> values = new List<string>();

        internal OptionValueCollection(OptionContext c)
        {
            this.c = c;
        }

        #region IEnumerable

        IEnumerator IEnumerable.GetEnumerator()
        {
            return values.GetEnumerator();
        }

        #endregion

        #region IEnumerable<T>

        public IEnumerator<string> GetEnumerator()
        {
            return values.GetEnumerator();
        }

        #endregion

        public List<string> ToList()
        {
            return new List<string>(values);
        }

        public string[] ToArray()
        {
            return values.ToArray();
        }

        public override string ToString()
        {
            return string.Join(", ", values.ToArray());
        }

        #region ICollection

        void ICollection.CopyTo(Array array, int index)
        {
            (values as ICollection).CopyTo(array, index);
        }

        bool ICollection.IsSynchronized
        {
            get { return (values as ICollection).IsSynchronized; }
        }

        object ICollection.SyncRoot
        {
            get { return (values as ICollection).SyncRoot; }
        }

        #endregion

        #region ICollection<T>

        public void Add(string item)
        {
            values.Add(item);
        }

        public void Clear()
        {
            values.Clear();
        }

        public bool Contains(string item)
        {
            return values.Contains(item);
        }

        public void CopyTo(string[] array, int arrayIndex)
        {
            values.CopyTo(array, arrayIndex);
        }

        public bool Remove(string item)
        {
            return values.Remove(item);
        }

        public int Count
        {
            get { return values.Count; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        #endregion

        #region IList

        int IList.Add(object value)
        {
            return (values as IList).Add(value);
        }

        bool IList.Contains(object value)
        {
            return (values as IList).Contains(value);
        }

        int IList.IndexOf(object value)
        {
            return (values as IList).IndexOf(value);
        }

        void IList.Insert(int index, object value)
        {
            (values as IList).Insert(index, value);
        }

        void IList.Remove(object value)
        {
            (values as IList).Remove(value);
        }

        void IList.RemoveAt(int index)
        {
            (values as IList).RemoveAt(index);
        }

        bool IList.IsFixedSize
        {
            get { return false; }
        }

        object IList.this[int index]
        {
            get { return this[index]; }
            set { (values as IList)[index] = value; }
        }

        #endregion

        #region IList<T>

        public int IndexOf(string item)
        {
            return values.IndexOf(item);
        }

        public void Insert(int index, string item)
        {
            values.Insert(index, item);
        }

        public void RemoveAt(int index)
        {
            values.RemoveAt(index);
        }

        void AssertValid(int index)
        {
            if (c.Option == null)
                throw new InvalidOperationException("OptionContext.Option is null.");
            if (index >= c.Option.MaxValueCount)
                throw new ArgumentOutOfRangeException("index");
            if (c.Option.OptionValueType == OptionValueType.Required &&
                index >= values.Count)
                throw new OptionException(string.Format(
                        c.OptionSet.MessageLocalizer("Missing required value for option '{0}'."), c.OptionName),
                    c.OptionName);
        }

        public string this[int index]
        {
            get
            {
                AssertValid(index);
                return index >= values.Count ? null : values[index];
            }
            set { values[index] = value; }
        }

        #endregion
    }


    public class OptionContext
    {
        OptionValueCollection c;
        int index;
        string name;
        Option option;
        OptionSet set;

        public OptionContext(OptionSet set)
        {
            this.set = set;
            c = new OptionValueCollection(this);
        }

        public Option Option
        {
            get { return option; }
            set { option = value; }
        }

        public string OptionName
        {
            get { return name; }
            set { name = value; }
        }

        public int OptionIndex
        {
            get { return index; }
            set { index = value; }
        }

        public OptionSet OptionSet
        {
            get { return set; }
        }

        public OptionValueCollection OptionValues
        {
            get { return c; }
        }
    }


    public enum OptionValueType
    {
        None,
        Optional,
        Required
    }


    public abstract class Option
    {
        static readonly char[] NameTerminator = {'=', ':'};
        int count;
        string[] names;

        string prototype,
            description;

        string[] separators;
        OptionValueType type;

        protected Option(string prototype, string description)
            : this(prototype, description, 1)
        {
        }

        protected Option(string prototype, string description, int maxValueCount)
        {
            if (prototype == null)
                throw new ArgumentNullException("prototype");
            if (prototype.Length == 0)
                throw new ArgumentException("Cannot be the empty string.", "prototype");
            if (maxValueCount < 0)
                throw new ArgumentOutOfRangeException("maxValueCount");

            this.prototype = prototype;
            names = prototype.Split('|');
            this.description = description;
            count = maxValueCount;
            type = ParsePrototype();

            if (count == 0 && type != OptionValueType.None)
                throw new ArgumentException(
                    "Cannot provide maxValueCount of 0 for OptionValueType.Required or " +
                    "OptionValueType.Optional.",
                    "maxValueCount");
            if (type == OptionValueType.None && maxValueCount > 1)
                throw new ArgumentException(
                    string.Format("Cannot provide maxValueCount of {0} for OptionValueType.None.", maxValueCount),
                    "maxValueCount");
            if (Array.IndexOf(names, "<>") >= 0 &&
                ((names.Length == 1 && type != OptionValueType.None) ||
                    (names.Length > 1 && MaxValueCount > 1)))
                throw new ArgumentException(
                    "The default option handler '<>' cannot require values.",
                    "prototype");
        }

        public string Prototype
        {
            get { return prototype; }
        }

        public string Description
        {
            get { return description; }
        }

        public OptionValueType OptionValueType
        {
            get { return type; }
        }

        public int MaxValueCount
        {
            get { return count; }
        }

        internal string[] Names
        {
            get { return names; }
        }

        internal string[] ValueSeparators
        {
            get { return separators; }
        }

        public string[] GetNames()
        {
            return (string[])names.Clone();
        }

        public string[] GetValueSeparators()
        {
            if (separators == null)
                return new string[0];
            return (string[])separators.Clone();
        }

        protected static T Parse<T>(string value, OptionContext c)
        {
            TypeConverter conv = TypeDescriptor.GetConverter(typeof(T));
            T t = default;
            try
            {
                if (value != null)
                    t = (T)conv.ConvertFromString(value);
            }
            catch (Exception e)
            {
                throw new OptionException(
                    string.Format(
                        c.OptionSet.MessageLocalizer("Could not convert string `{0}' to type {1} for option `{2}'."),
                        value, typeof(T).Name, c.OptionName),
                    c.OptionName, e);
            }

            return t;
        }

        OptionValueType ParsePrototype()
        {
            var type = '\0';
            var seps = new List<string>();
            for (var i = 0; i < names.Length; ++i)
            {
                string name = names[i];
                if (name.Length == 0)
                    throw new ArgumentException("Empty option names are not supported.", "prototype");

                int end = name.IndexOfAny(NameTerminator);
                if (end == -1)
                    continue;
                names[i] = name.Substring(0, end);
                if (type == '\0' || type == name[end])
                    type = name[end];
                else
                    throw new ArgumentException(
                        string.Format("Conflicting option types: '{0}' vs. '{1}'.", type, name[end]),
                        "prototype");
                AddSeparators(name, end, seps);
            }

            if (type == '\0')
                return OptionValueType.None;

            if (count <= 1 && seps.Count != 0)
                throw new ArgumentException(
                    string.Format("Cannot provide key/value separators for Options taking {0} value(s).", count),
                    "prototype");
            if (count > 1)
            {
                if (seps.Count == 0)
                    separators = new[] {":", "="};
                else if (seps.Count == 1 && seps[0].Length == 0)
                    separators = null;
                else
                    separators = seps.ToArray();
            }

            return type == '=' ? OptionValueType.Required : OptionValueType.Optional;
        }

        static void AddSeparators(string name, int end, ICollection<string> seps)
        {
            int start = -1;
            for (int i = end + 1; i < name.Length; ++i)
            {
                switch (name[i])
                {
                    case '{':
                        if (start != -1)
                            throw new ArgumentException(
                                string.Format("Ill-formed name/value separator found in \"{0}\".", name),
                                "prototype");
                        start = i + 1;
                        break;
                    case '}':
                        if (start == -1)
                            throw new ArgumentException(
                                string.Format("Ill-formed name/value separator found in \"{0}\".", name),
                                "prototype");
                        seps.Add(name.Substring(start, i - start));
                        start = -1;
                        break;
                    default:
                        if (start == -1)
                            seps.Add(name[i].ToString());
                        break;
                }
            }

            if (start != -1)
                throw new ArgumentException(
                    string.Format("Ill-formed name/value separator found in \"{0}\".", name),
                    "prototype");
        }

        public void Invoke(OptionContext c)
        {
            OnParseComplete(c);
            c.OptionName = null;
            c.Option = null;
            c.OptionValues.Clear();
        }

        protected abstract void OnParseComplete(OptionContext c);

        public override string ToString()
        {
            return Prototype;
        }
    }


    [Serializable]
    public class OptionException : Exception
    {
        string option;

        public OptionException()
        {
        }

        public OptionException(string message, string optionName)
            : base(message)
        {
            option = optionName;
        }

        public OptionException(string message, string optionName, Exception innerException)
            : base(message, innerException)
        {
            option = optionName;
        }

        protected OptionException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            option = info.GetString("OptionName");
        }

        public string OptionName
        {
            get { return option; }
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("OptionName", option);
        }
    }


    public delegate void OptionAction<TKey, TValue>(TKey key, TValue value);


    public class OptionSet : KeyedCollection<string, Option>
    {
        public OptionSet()
            : this(delegate(string f)
            {
                return f;
            })
        {
        }

        public OptionSet(Converter<string, string> localizer)
        {
            this.localizer = localizer;
        }

        Converter<string, string> localizer;

        public Converter<string, string> MessageLocalizer
        {
            get { return localizer; }
        }

        protected override string GetKeyForItem(Option item)
        {
            if (item == null)
                throw new ArgumentNullException("option");
            if (item.Names != null && item.Names.Length > 0)
                return item.Names[0];
            // This should never happen, as it's invalid for Option to be
            // constructed w/o any names.
            throw new InvalidOperationException("Option has no names!");
        }

        [Obsolete("Use KeyedCollection.this[string]")]
        protected Option GetOptionForName(string option)
        {
            if (option == null)
                throw new ArgumentNullException("option");
            try
            {
                return base[option];
            }
            catch (KeyNotFoundException)
            {
                return null;
            }
        }

        protected override void InsertItem(int index, Option item)
        {
            base.InsertItem(index, item);
            AddImpl(item);
        }

        protected override void RemoveItem(int index)
        {
            base.RemoveItem(index);
            Option p = Items[index];
            // KeyedCollection.RemoveItem() handles the 0th item
            for (var i = 1; i < p.Names.Length; ++i)
            {
                Dictionary.Remove(p.Names[i]);
            }
        }

        protected override void SetItem(int index, Option item)
        {
            base.SetItem(index, item);
            RemoveItem(index);
            AddImpl(item);
        }

        void AddImpl(Option option)
        {
            if (option == null)
                throw new ArgumentNullException("option");
            var added = new List<string>(option.Names.Length);
            try
            {
                // KeyedCollection.InsertItem/SetItem handle the 0th name.
                for (var i = 1; i < option.Names.Length; ++i)
                {
                    Dictionary.Add(option.Names[i], option);
                    added.Add(option.Names[i]);
                }
            }
            catch (Exception)
            {
                foreach (string name in added)
                    Dictionary.Remove(name);
                throw;
            }
        }

        public new OptionSet Add(Option option)
        {
            base.Add(option);
            return this;
        }


        sealed class ActionOption : Option
        {
            Action<OptionValueCollection> action;

            public ActionOption(string prototype, string description, int count, Action<OptionValueCollection> action)
                : base(prototype, description, count)
            {
                if (action == null)
                    throw new ArgumentNullException("action");
                this.action = action;
            }

            protected override void OnParseComplete(OptionContext c)
            {
                action(c.OptionValues);
            }
        }


        public OptionSet Add(string prototype, Action<string> action)
        {
            return Add(prototype, null, action);
        }

        public OptionSet Add(string prototype, string description, Action<string> action)
        {
            if (action == null)
                throw new ArgumentNullException("action");
            Option p = new ActionOption(prototype, description, 1,
                delegate(OptionValueCollection v)
                {
                    action(v[0]);
                });
            base.Add(p);
            return this;
        }

        public OptionSet Add(string prototype, OptionAction<string, string> action)
        {
            return Add(prototype, null, action);
        }

        public OptionSet Add(string prototype, string description, OptionAction<string, string> action)
        {
            if (action == null)
                throw new ArgumentNullException("action");
            Option p = new ActionOption(prototype, description, 2,
                delegate(OptionValueCollection v)
                {
                    action(v[0], v[1]);
                });
            base.Add(p);
            return this;
        }


        sealed class ActionOption<T> : Option
        {
            Action<T> action;

            public ActionOption(string prototype, string description, Action<T> action)
                : base(prototype, description, 1)
            {
                if (action == null)
                    throw new ArgumentNullException("action");
                this.action = action;
            }

            protected override void OnParseComplete(OptionContext c)
            {
                action(Parse<T>(c.OptionValues[0], c));
            }
        }


        sealed class ActionOption<TKey, TValue> : Option
        {
            OptionAction<TKey, TValue> action;

            public ActionOption(string prototype, string description, OptionAction<TKey, TValue> action)
                : base(prototype, description, 2)
            {
                if (action == null)
                    throw new ArgumentNullException("action");
                this.action = action;
            }

            protected override void OnParseComplete(OptionContext c)
            {
                action(
                    Parse<TKey>(c.OptionValues[0], c),
                    Parse<TValue>(c.OptionValues[1], c));
            }
        }


        public OptionSet Add<T>(string prototype, Action<T> action)
        {
            return Add(prototype, null, action);
        }

        public OptionSet Add<T>(string prototype, string description, Action<T> action)
        {
            return Add(new ActionOption<T>(prototype, description, action));
        }

        public OptionSet Add<TKey, TValue>(string prototype, OptionAction<TKey, TValue> action)
        {
            return Add(prototype, null, action);
        }

        public OptionSet Add<TKey, TValue>(string prototype, string description, OptionAction<TKey, TValue> action)
        {
            return Add(new ActionOption<TKey, TValue>(prototype, description, action));
        }

        protected virtual OptionContext CreateOptionContext()
        {
            return new OptionContext(this);
        }

#if LINQ
		public List<string> Parse (IEnumerable<string> arguments)
		{
			bool process = true;
			OptionContext c = CreateOptionContext ();
			c.OptionIndex = -1;
			var def = GetOptionForName ("<>");
			var unprocessed =
				from argument in arguments
				where ++c.OptionIndex >= 0 && (process || def != null)
					? process
						? argument == "--" 
							? (process = false)
							: !Parse (argument, c)
								? def != null 
									? Unprocessed (null, def, c, argument) 
									: true
								: false
						: def != null 
							? Unprocessed (null, def, c, argument)
							: true
					: true
				select argument;
			List<string> r = unprocessed.ToList ();
			if (c.Option != null)
				c.Option.Invoke (c);
			return r;
		}
#else
        public List<string> Parse(IEnumerable<string> arguments)
        {
            OptionContext c = CreateOptionContext();
            c.OptionIndex = -1;
            var process = true;
            var unprocessed = new List<string>();
            Option def = Contains("<>") ? this["<>"] : null;
            foreach (string argument in arguments)
            {
                ++c.OptionIndex;
                if (argument == "--")
                {
                    process = false;
                    continue;
                }

                if (!process)
                {
                    Unprocessed(unprocessed, def, c, argument);
                    continue;
                }

                if (!Parse(argument, c))
                    Unprocessed(unprocessed, def, c, argument);
            }

            if (c.Option != null)
                c.Option.Invoke(c);
            return unprocessed;
        }
#endif

        static bool Unprocessed(ICollection<string> extra, Option def, OptionContext c, string argument)
        {
            if (def == null)
            {
                extra.Add(argument);
                return false;
            }

            c.OptionValues.Add(argument);
            c.Option = def;
            c.Option.Invoke(c);
            return false;
        }

        readonly Regex ValueOption = new Regex(
            @"^(?<flag>--|-|/)(?<name>[^:=]+)((?<sep>[:=])(?<value>.*))?$");

        protected bool GetOptionParts(string argument, out string flag, out string name, out string sep,
            out string value)
        {
            if (argument == null)
                throw new ArgumentNullException("argument");

            flag = name = sep = value = null;
            Match m = ValueOption.Match(argument);
            if (!m.Success)
            {
                return false;
            }

            flag = m.Groups["flag"].Value;
            name = m.Groups["name"].Value;
            if (m.Groups["sep"].Success && m.Groups["value"].Success)
            {
                sep = m.Groups["sep"].Value;
                value = m.Groups["value"].Value;
            }

            return true;
        }

        protected virtual bool Parse(string argument, OptionContext c)
        {
            if (c.Option != null)
            {
                ParseValue(argument, c);
                return true;
            }

            string f,
                n,
                s,
                v;
            if (!GetOptionParts(argument, out f, out n, out s, out v))
                return false;

            Option p;
            if (Contains(n))
            {
                p = this[n];
                c.OptionName = f + n;
                c.Option = p;
                switch (p.OptionValueType)
                {
                    case OptionValueType.None:
                        c.OptionValues.Add(n);
                        c.Option.Invoke(c);
                        break;
                    case OptionValueType.Optional:
                    case OptionValueType.Required:
                        ParseValue(v, c);
                        break;
                }

                return true;
            }

            // no match; is it a bool option?
            if (ParseBool(argument, n, c))
                return true;
            // is it a bundled option?
            if (ParseBundledValue(f, string.Concat(n + s + v), c))
                return true;

            return false;
        }

        void ParseValue(string option, OptionContext c)
        {
            if (option != null)
                foreach (string o in c.Option.ValueSeparators != null
                    ? option.Split(c.Option.ValueSeparators, StringSplitOptions.None)
                    : new[] {option})
                {
                    c.OptionValues.Add(o);
                }

            if (c.OptionValues.Count == c.Option.MaxValueCount ||
                c.Option.OptionValueType == OptionValueType.Optional)
                c.Option.Invoke(c);
            else if (c.OptionValues.Count > c.Option.MaxValueCount)
            {
                throw new OptionException(localizer(string.Format(
                        "Error: Found {0} option values when expecting {1}.",
                        c.OptionValues.Count, c.Option.MaxValueCount)),
                    c.OptionName);
            }
        }

        bool ParseBool(string option, string n, OptionContext c)
        {
            Option p;
            string rn;
            if (n.Length >= 1 && (n[n.Length - 1] == '+' || n[n.Length - 1] == '-') &&
                Contains(rn = n.Substring(0, n.Length - 1)))
            {
                p = this[rn];
                string v = n[n.Length - 1] == '+' ? option : null;
                c.OptionName = option;
                c.Option = p;
                c.OptionValues.Add(v);
                p.Invoke(c);
                return true;
            }

            return false;
        }

        bool ParseBundledValue(string f, string n, OptionContext c)
        {
            if (f != "-")
                return false;
            for (var i = 0; i < n.Length; ++i)
            {
                Option p;
                string opt = f + n[i];
                string rn = n[i].ToString();
                if (!Contains(rn))
                {
                    if (i == 0)
                        return false;
                    throw new OptionException(string.Format(localizer(
                        "Cannot bundle unregistered option '{0}'."), opt), opt);
                }

                p = this[rn];
                switch (p.OptionValueType)
                {
                    case OptionValueType.None:
                        Invoke(c, opt, n, p);
                        break;
                    case OptionValueType.Optional:
                    case OptionValueType.Required:
                    {
                        string v = n.Substring(i + 1);
                        c.Option = p;
                        c.OptionName = opt;
                        ParseValue(v.Length != 0 ? v : null, c);
                        return true;
                    }
                    default:
                        throw new InvalidOperationException("Unknown OptionValueType: " + p.OptionValueType);
                }
            }

            return true;
        }

        static void Invoke(OptionContext c, string name, string value, Option option)
        {
            c.OptionName = name;
            c.Option = option;
            c.OptionValues.Add(value);
            option.Invoke(c);
        }

        const int OptionWidth = 29;

        public void WriteOptionDescriptions(TextWriter o)
        {
            foreach (Option p in this)
            {
                var written = 0;
                if (!WriteOptionPrototype(o, p, ref written))
                    continue;

                if (written < OptionWidth)
                    o.Write(new string(' ', OptionWidth - written));
                else
                {
                    o.WriteLine();
                    o.Write(new string(' ', OptionWidth));
                }

                List<string> lines = GetLines(localizer(GetDescription(p.Description)));
                o.WriteLine(lines[0]);
                var prefix = new string(' ', OptionWidth + 2);
                for (var i = 1; i < lines.Count; ++i)
                {
                    o.Write(prefix);
                    o.WriteLine(lines[i]);
                }
            }
        }

        bool WriteOptionPrototype(TextWriter o, Option p, ref int written)
        {
            string[] names = p.Names;

            int i = GetNextOptionIndex(names, 0);
            if (i == names.Length)
                return false;

            if (names[i].Length == 1)
            {
                Write(o, ref written, "  -");
                Write(o, ref written, names[0]);
            }
            else
            {
                Write(o, ref written, "      --");
                Write(o, ref written, names[0]);
            }

            for (i = GetNextOptionIndex(names, i + 1);
                i < names.Length;
                i = GetNextOptionIndex(names, i + 1))
            {
                Write(o, ref written, ", ");
                Write(o, ref written, names[i].Length == 1 ? "-" : "--");
                Write(o, ref written, names[i]);
            }

            if (p.OptionValueType == OptionValueType.Optional ||
                p.OptionValueType == OptionValueType.Required)
            {
                if (p.OptionValueType == OptionValueType.Optional)
                {
                    Write(o, ref written, localizer("["));
                }

                Write(o, ref written, localizer("=" + GetArgumentName(0, p.MaxValueCount, p.Description)));
                string sep = p.ValueSeparators != null && p.ValueSeparators.Length > 0
                    ? p.ValueSeparators[0]
                    : " ";
                for (var c = 1; c < p.MaxValueCount; ++c)
                {
                    Write(o, ref written, localizer(sep + GetArgumentName(c, p.MaxValueCount, p.Description)));
                }

                if (p.OptionValueType == OptionValueType.Optional)
                {
                    Write(o, ref written, localizer("]"));
                }
            }

            return true;
        }

        static int GetNextOptionIndex(string[] names, int i)
        {
            while (i < names.Length && names[i] == "<>")
            {
                ++i;
            }

            return i;
        }

        static void Write(TextWriter o, ref int n, string s)
        {
            n += s.Length;
            o.Write(s);
        }

        static string GetArgumentName(int index, int maxIndex, string description)
        {
            if (description == null)
                return maxIndex == 1 ? "VALUE" : "VALUE" + (index + 1);
            string[] nameStart;
            if (maxIndex == 1)
                nameStart = new[] {"{0:", "{"};
            else
                nameStart = new[] {"{" + index + ":"};
            for (var i = 0; i < nameStart.Length; ++i)
            {
                int start,
                    j = 0;
                do
                {
                    start = description.IndexOf(nameStart[i], j);
                }
                while (start >= 0 && j != 0 ? description[j++ - 1] == '{' : false);

                if (start == -1)
                    continue;
                int end = description.IndexOf("}", start);
                if (end == -1)
                    continue;
                return description.Substring(start + nameStart[i].Length, end - start - nameStart[i].Length);
            }

            return maxIndex == 1 ? "VALUE" : "VALUE" + (index + 1);
        }

        static string GetDescription(string description)
        {
            if (description == null)
                return string.Empty;
            var sb = new StringBuilder(description.Length);
            int start = -1;
            for (var i = 0; i < description.Length; ++i)
            {
                switch (description[i])
                {
                    case '{':
                        if (i == start)
                        {
                            sb.Append('{');
                            start = -1;
                        }
                        else if (start < 0)
                            start = i + 1;

                        break;
                    case '}':
                        if (start < 0)
                        {
                            if (i + 1 == description.Length || description[i + 1] != '}')
                                throw new InvalidOperationException("Invalid option description: " + description);
                            ++i;
                            sb.Append("}");
                        }
                        else
                        {
                            sb.Append(description.Substring(start, i - start));
                            start = -1;
                        }

                        break;
                    case ':':
                        if (start < 0)
                            goto default;
                        start = i + 1;
                        break;
                    default:
                        if (start < 0)
                            sb.Append(description[i]);
                        break;
                }
            }

            return sb.ToString();
        }

        static List<string> GetLines(string description)
        {
            var lines = new List<string>();
            if (string.IsNullOrEmpty(description))
            {
                lines.Add(string.Empty);
                return lines;
            }

            int length = 80 - OptionWidth - 2;
            int start = 0,
                end;
            do
            {
                end = GetLineEnd(start, length, description);
                var cont = false;
                if (end < description.Length)
                {
                    char c = description[end];
                    if (c == '-' || (char.IsWhiteSpace(c) && c != '\n'))
                        ++end;
                    else if (c != '\n')
                    {
                        cont = true;
                        --end;
                    }
                }

                lines.Add(description.Substring(start, end - start));
                if (cont)
                {
                    lines[lines.Count - 1] += "-";
                }

                start = end;
                if (start < description.Length && description[start] == '\n')
                    ++start;
            }
            while (end < description.Length);

            return lines;
        }

        static int GetLineEnd(int start, int length, string description)
        {
            int end = Math.Min(start + length, description.Length);
            int sep = -1;
            for (int i = start; i < end; ++i)
            {
                switch (description[i])
                {
                    case ' ':
                    case '\t':
                    case '\v':
                    case '-':
                    case ',':
                    case '.':
                    case ';':
                        sep = i;
                        break;
                    case '\n':
                        return i;
                }
            }

            if (sep == -1 || end == description.Length)
                return end;
            return sep;
        }
    }
}