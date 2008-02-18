using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Reflection;
using System.Xml;
using log4net;
using MassTransit.ServiceBus;

namespace MassTransit.Host.Config
{
	public class HostConfigurator :
		IConfigurator
	{
		private static readonly ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

		private readonly Dictionary<string, EndpointConfigurator> _endpoints = new Dictionary<string, EndpointConfigurator>();
		private FileInfo _fileInfo;

		public HostConfigurator()
		{
			_fileInfo = new FileInfo(Path.Combine(Path.GetDirectoryName(Assembly.GetCallingAssembly().Location), "host.config"));
		}

		#region IConfigurator Members

		public string File
		{
			get { return _fileInfo.Name; }
			set { _fileInfo = new FileInfo(value); }
		}

		#endregion

		public void Configure()
		{
			if (_fileInfo.Exists)
			{
				FileStream fs;
				try
				{
					fs = _fileInfo.Open(FileMode.Open, FileAccess.Read, FileShare.Read);
				}
				catch (IOException ex)
				{
					fs = null;
				}

				if (fs != null)
				{
					InternalConfigure(fs);
				}
			}
		}

		public Dictionary<string, EndpointConfigurator>.Enumerator Endpoints
		{
			get
			{
				return _endpoints.GetEnumerator();
			}
		}

		public void InternalConfigure(FileStream fs)
		{
			XmlReaderSettings settings = new XmlReaderSettings();
			settings.ProhibitDtd = false;
			settings.IgnoreWhitespace = true;

			XmlReader xmlReader = XmlReader.Create(fs, settings);
			while (xmlReader.Read())
			{
				_log.InfoFormat("{0} - {1}", xmlReader.NodeType, xmlReader.LocalName);

				if (xmlReader.NodeType == XmlNodeType.Element)
				{
					if (xmlReader.LocalName == "endpoint")
					{
						ConfigureEndpoint(xmlReader);
					}
					else if (xmlReader.LocalName == "assembly")
					{
						ConfigureAssembly(xmlReader);
					}
				}
			}
		}

		private void ConfigureAssembly(XmlReader xmlReader)
		{
			string name = xmlReader.GetAttribute("name");
			Assembly assembly = Assembly.Load(name);

			string endpointName = null;

			while ( xmlReader.Read())
			{
				if (xmlReader.NodeType == XmlNodeType.EndElement && xmlReader.LocalName == "assembly")
					break;

				_log.InfoFormat("{0} - {1}", xmlReader.NodeType, xmlReader.LocalName);

				if (xmlReader.LocalName == "endpoint-ref")
				{
					endpointName = xmlReader.GetAttribute("name");
					if (_endpoints.ContainsKey(endpointName) == false)
					{
						_log.ErrorFormat("Invalid endpoint reference: {0}", endpointName);
						throw new HostConfigurationException("Invalid Endpoint Reference: " + endpointName);
					}
				}
				else if ( xmlReader.LocalName == "component")
				{
					string componentName = xmlReader.GetAttribute("name");

					Type t = assembly.GetType(componentName);
					if (t == null)
					{
						_log.ErrorFormat("Invalid component name: {0}", componentName);
						throw new HostConfigurationException("Invalid type specified: " + componentName);
					}

					if (t.IsAssignableFrom(typeof(IAutoSubscriber)) && !t.IsAbstract)
						_endpoints[endpointName].Types.Add(t);
					else
					{
						_log.ErrorFormat("Type does not support auto registration: {0}", componentName);
						throw new HostConfigurationException("Unsupported type specified: " + componentName);
					}
				}

				_log.InfoFormat("Service: {0}, Endpoint: {1}", name, endpointName);
			}

			if (_endpoints[endpointName].Types.Count == 0)
			{
				Type[] types = assembly.GetTypes();
				foreach (Type t in types)
				{
					if (t.IsAssignableFrom(typeof (IAutoSubscriber)) && !t.IsAbstract)
						_endpoints[endpointName].Types.Add(t);
				}
			}
		}

		private void ConfigureEndpoint(XmlReader xmlReader)
		{
			string name = xmlReader.GetAttribute("name");

			xmlReader.Read();
			if (xmlReader.LocalName == "uri")
			{
				xmlReader.Read();
				if (xmlReader.NodeType == XmlNodeType.Text)
				{
					string uriString = xmlReader.ReadContentAsString();

					_endpoints.Add(name, new EndpointConfigurator(new MessageQueueEndpoint(uriString)));

					_log.InfoFormat("Endpoint: {0}, URI: {1}", name, uriString);
				}
			}
		}

		#region Nested type: EndpointConfigurator

		#endregion
	}
}