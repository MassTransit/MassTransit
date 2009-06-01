// Copyright 2007-2008 The Apache Software Foundation.
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
namespace MassTransit.SystemView
{
	using System;
	using System.Collections.Generic;
	using System.ComponentModel;
	using System.Drawing;
	using System.Windows.Forms;
	using Magnum;
	using Services.HealthMonitoring.Messages;
	using Services.Subscriptions.Messages;
	using StructureMap.Attributes;
	using Container=StructureMap.Container;
	using IContainer=StructureMap.IContainer;

	public partial class MainForm :
		Form,
		Consumes<SubscriptionRefresh>.All,
		Consumes<AddSubscription>.All,
		Consumes<RemoveSubscription>.All,
		Consumes<HealthUpdate>.All
	{
		private IServiceBus _bus;
		private Guid _clientId = CombGuid.Generate();
		private IContainer _container;
		private IEndpoint _subscriptionServiceEndpoint;
		private UnsubscribeAction _unsubscribe;

		public MainForm()
		{
			InitializeComponent();
		}

		public void Consume(AddSubscription message)
		{
			Action<SubscriptionInformation> method = x => AddSubscriptionToView(x);
			BeginInvoke(method, new object[] {message.Subscription});
		}

		public void Consume(HealthUpdate message)
		{
			//HeartBeatRefreshNeeded(message);
		}

		public void Consume(RemoveSubscription message)
		{
			Action<SubscriptionInformation> method = x => RemoveSubscriptionFromView(x);
			BeginInvoke(method, new object[] {message.Subscription});
		}

		public void Consume(SubscriptionRefresh message)
		{
			Action<IEnumerable<SubscriptionInformation>> method = x => RefreshSubscriptions(x);
			BeginInvoke(method, new object[] {message.Subscriptions});
		}

		protected override void OnClosing(CancelEventArgs e)
		{
			_unsubscribe();

			_bus.Dispose();
			_bus = null;

			base.OnClosing(e);
		}

		private void MainForm_Load(object sender, EventArgs e)
		{
			BootstrapContainer();

			BootstrapServiceBus();

			ConnectToSubscriptionService();
		}

		private void ConnectToSubscriptionService()
		{
			_subscriptionServiceEndpoint = _container.GetInstance<IEndpointFactory>()
				.GetEndpoint(_container.GetInstance<IConfiguration>()
					.SubscriptionServiceUri);

			_subscriptionServiceEndpoint.Send(new AddSubscriptionClient(_clientId, _bus.Endpoint.Uri, _bus.Endpoint.Uri));
		}

		private void BootstrapServiceBus()
		{
			_bus = _container.GetInstance<IServiceBus>();
			_unsubscribe = _bus.Subscribe(this);
		}

		private void BootstrapContainer()
		{
			_container = new Container();
			_container.Configure(x =>
				{
					x.ForRequestedType<IConfiguration>()
						.CacheBy(InstanceScope.Singleton)
						.AddConcreteType<Configuration>();
				});

			var registry = new SystemViewRegistry(_container);
			_container.Configure(x => x.AddRegistry(registry));
		}

		private TreeNode AddSubscriptionToView(SubscriptionInformation subscription)
		{
			TreeNode endpointNode;
			if (!subscriptionView.Nodes.ContainsKey(subscription.EndpointUri.ToString()))
			{
				endpointNode = new TreeNode(subscription.EndpointUri.ToString());
				endpointNode.Name = subscription.EndpointUri.ToString();

				subscriptionView.Nodes.Add(endpointNode);
			}
			else
			{
				endpointNode = subscriptionView.Nodes[subscription.EndpointUri.ToString()];
			}

			string messageName = subscription.MessageName;

			var parts = subscription.MessageName.Split(',');
			var d = parts.Length > 0 ? parts[0] : subscription.MessageName;
			var dd = d.Split('.');

			string description = dd[dd.Length - 1];
			if (!string.IsNullOrEmpty(subscription.CorrelationId))
				description += " (" + subscription.CorrelationId + ")";

			TreeNode messageNode;
			if (!endpointNode.Nodes.ContainsKey(messageName))
			{
				messageNode = new TreeNode(description);

				if (messageName.StartsWith("MassTransit"))
					messageNode.ForeColor = Color.DimGray;

				messageNode.Name = messageName;

				endpointNode.Nodes.Add(messageNode);
			}
			else
			{
				messageNode = endpointNode.Nodes[messageName];
				if (messageNode.Text != description)
					messageNode.Text = description;
			}

			return messageNode;
		}

		private void RemoveSubscriptionFromView(SubscriptionInformation subscription)
		{
			if (!subscriptionView.Nodes.ContainsKey(subscription.EndpointUri.ToString()))
				return;

			TreeNode endpointNode = subscriptionView.Nodes[subscription.EndpointUri.ToString()];

			string messageName = subscription.MessageName;

			if (!endpointNode.Nodes.ContainsKey(messageName))
				return;

			TreeNode messageNode = endpointNode.Nodes[messageName];

			messageNode.Remove();
		}

		private void RefreshSubscriptions(IEnumerable<SubscriptionInformation> subscriptions)
		{
			var existingNodes = new List<TreeNode>();
			foreach (TreeNode endpointNode in subscriptionView.Nodes)
			{
				foreach (TreeNode subscriptionNode in endpointNode.Nodes)
				{
					existingNodes.Add(subscriptionNode);
				}
			}

			foreach (SubscriptionInformation subscription in subscriptions)
			{
				TreeNode messageNode = AddSubscriptionToView(subscription);

				if (existingNodes.Contains(messageNode))
					existingNodes.Remove(messageNode);
			}

			foreach (TreeNode node in existingNodes)
			{
				node.Remove();
			}
		}
	}
}