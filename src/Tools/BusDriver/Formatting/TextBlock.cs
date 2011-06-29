// Copyright 2007-2011 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace BusDriver.Formatting
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;
	using Magnum.Extensions;

	public class TextBlock :
		ITextBlock
	{
		readonly StringBuilder _sb;
		int _indent;
		int _tabWidth;

		public TextBlock()
		{
			Width = Console.WindowWidth - 1;
			_sb = new StringBuilder();
			_indent = 0;
			_tabWidth = 4;
		}

		public int Width { get; private set; }

		public ITextBlock BeginBlock(object left, object right)
		{
			Separator();

			string leftString = left != null ? left.ToString() : "";
			string rightString = right != null ? right.ToString() : "";

			int padding = Width - _indent - 2 - leftString.Length - rightString.Length;

			_sb.Append(' ', _indent + 1);
			_sb.Append(leftString);
			if (padding > 0)
				_sb.Append(' ', padding);
			_sb.Append(rightString);
			_sb.AppendLine(" ");

			Separator();

			_indent += _tabWidth;

			return this;
		}

		public ITextBlock EndBlock()
		{
			_indent -= _tabWidth;

			return Break();
		}

		public ITextBlock Table(IDictionary<string, string> values, params string[] extra)
		{
			int count = extra.Length/2;
			for (int i = 0; i < count; i++)
			{
				values.Add(extra[i*2], extra[i*2 + 1] );
			}

			int labelWidth = values.Select(x => x.Key.Length).Max();

			string format = string.Format("{{0,-{0}}}{{1}}", labelWidth + 3);

			values.Each(x =>
				{
					BodyFormat(format, x.Key + ":", x.Value);
				});

			return this;
		}

		public ITextBlock Separator()
		{
			_sb.Append(' ', _indent).Append('-', Width - _indent).AppendLine();
			return this;
		}

		public ITextBlock Break()
		{
			_sb.AppendLine();
			return this;
		}

		public ITextBlock BodyFormat(string format, params object[] args)
		{
			string line = string.Format(format, args);

			return Body(line);
		}

		public ITextBlock Body(string text)
		{
			_sb.Append(' ', _indent).AppendLine(text);

			return this;
		}

		public override string ToString()
		{
			return _sb.ToString();
		}
	}
}