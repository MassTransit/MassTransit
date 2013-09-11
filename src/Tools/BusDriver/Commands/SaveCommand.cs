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
namespace BusDriver.Commands
{
	using System;
	using System.IO;
	using System.Text;
	using Formatting;
	using MassTransit.Logging;
	using Magnum.Extensions;
	using Magnum.FileSystem;
	using Magnum.FileSystem.Internal;
	using MassTransit;
	using MassTransit.Transports;

	public class SaveCommand :
		Command
	{
		static readonly ILog _log = Logger.Get(typeof (SaveCommand));
		readonly int _count;
		readonly string _name;
		readonly bool _remove;
		readonly string _uriString;
		int _nextFileNumber;

		public SaveCommand(string uriString, string name, int count, bool remove)
		{
			_uriString = uriString;
			_name = name;
			_count = count;
			_remove = remove;
			_nextFileNumber = 0;
		}

		public bool Execute()
		{
			Uri uri = _uriString.ToUri("The from URI was invalid");

			AbsolutePathName fullPath = PathName.GetAbsolutePathName(_name, Environment.CurrentDirectory);
			_log.DebugFormat("Using output path name: {0}", fullPath);

			string directoryName = Path.GetDirectoryName(fullPath.GetPath());
			if (!System.IO.Directory.Exists(directoryName))
				System.IO.Directory.CreateDirectory(directoryName);

			IInboundTransport fromTransport = Program.Transports.GetInboundTransport(uri);

			ITextBlock text = new TextBlock()
				.BeginBlock("Save messages from URI: " + uri, "");

			int lastCount;
			int saveCount = 0;
			do
			{
				lastCount = saveCount;

				fromTransport.Receive(receiveContext =>
					{
						if (saveCount >= _count)
							return null;

						string body = Encoding.UTF8.GetString(receiveContext.BodyStream.ReadToEnd());

						text.BodyFormat("Message-Id: {0}", receiveContext.MessageId ?? "");

						WriteMessageToFile(fullPath.ToString(), receiveContext, body);

						saveCount++;

						if (_remove)
							return context => { };

						return null;
                    }, 5.Seconds());
			} while (_remove && saveCount < _count && saveCount != lastCount);

			_log.Info(text.ToString());

			return true;
		}

		void WriteMessageToFile(string pathName, IReceiveContext context, string body)
		{
			string fileName = GetNextFileName(pathName);

			using (StreamWriter stream = System.IO.File.CreateText(fileName))
			{
				if (context.ContentType.IsNotEmpty())
					stream.WriteLine("Content-Type: {0}", context.ContentType);
				if (context.MessageId.IsNotEmpty())
					stream.WriteLine("Message-Id: {0}", context.MessageId);

				stream.WriteLine();
				stream.Write(body);
				stream.Close();
			}
		}

		string GetNextFileName(string pathName)
		{
			string nextFileName;
			do
			{
				nextFileName = string.Format("{0}-{1:00000}.msg", pathName, _nextFileNumber++);
			} while (System.IO.File.Exists(nextFileName));

			return nextFileName;
		}
	}
}