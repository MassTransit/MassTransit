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
	using Distributor.Messages;
	using System;
	using System.Collections.Generic;
	using System.ComponentModel;
	using System.Drawing;
    using System.Linq;
	using System.Windows.Forms;
	using Magnum;
	using Services.HealthMonitoring.Messages;
	using Services.Subscriptions.Messages;
	using Services.Timeout.Messages;

	public partial class MainForm :
		Form,
		Consumes<SubscriptionRefresh>.All,
		Consumes<AddSubscription>.All,
		Consumes<RemoveSubscription>.All,
		Consumes<HealthUpdate>.All,
		Consumes<TimeoutScheduled>.All,
		Consumes<TimeoutRescheduled>.All,
		Consumes<TimeoutExpired>.All,
		Consumes<EndpointIsHealthy>.All,
		Consumes<EndpointIsDown>.All,
		Consumes<EndpointIsSuspect>.All,
		Consumes<EndpointIsOffline>.All,
        Consumes<IWorkerAvailable>.All
	{
		private IServiceBus _bus;
		private readonly Guid _clientId = CombGuid.Generate();
		private StructureMap.IContainer _container;
		private IEndpoint _subscriptionServiceEndpoint;
		private UnsubscribeAction _unsubscribe;

		public MainForm()
		{
			InitializeComponent();
		}

        public void Consume(IWorkerAvailable message)
        {
        }

	    public void Consume(AddSubscription message)
		{
			Action<SubscriptionInformation> method = x => AddSubscriptionToView(x);
			BeginInvoke(method, new object[] {message.Subscription});
		}

		public void Consume(EndpointIsDown message)
		{
			Action<EndpointIsDown> method = x => AddOrUpdateHealthItem(x.ControlUri, x.LastHeartbeat, x.State);
			BeginInvoke(method, new object[] { message });
		}

		public void Consume(EndpointIsHealthy message)
		{
			Action<EndpointIsHealthy> method = x => AddOrUpdateHealthItem(x.ControlUri, x.LastHeartbeat, x.State);
			BeginInvoke(method, new object[] { message });
		}

		public void Consume(EndpointIsOffline message)
		{
			Action<EndpointIsOffline> method = x => AddOrUpdateHealthItem(x.ControlUri, x.LastHeartbeat, x.State);
			BeginInvoke(method, new object[] { message });
		}

		public void Consume(EndpointIsSuspect message)
		{
			Action<EndpointIsSuspect> method = x => AddOrUpdateHealthItem(x.ControlUri, x.LastHeartbeat, x.State);
			BeginInvoke(method, new object[] { message });
		}

		public void Consume(HealthUpdate message)
		{
			Action<IEnumerable<HealthInformation>> method = RefreshHealthView;
			BeginInvoke(method, new object[] {message.Information});
		}

		public void Consume(RemoveSubscription message)
		{
			Action<SubscriptionInformation> method = RemoveSubscriptionFromView;
			BeginInvoke(method, new object[] {message.Subscription});
		}

		public void Consume(SubscriptionRefresh message)
		{
			Action<IEnumerable<SubscriptionInformation>> method = RefreshSubscriptions;
			BeginInvoke(method, new object[] {message.Subscriptions});
		}

		public void Consume(TimeoutExpired message)
		{
			Action<TimeoutExpired> method = x => RemoveTimeoutFromListView(x.CorrelationId, x.Tag);
			BeginInvoke(method, new object[] {message});
		}

		public void Consume(TimeoutRescheduled message)
		{
			Action<TimeoutRescheduled> method = x => AddOrUpdateTimeoutListView(x.CorrelationId, x.Tag, x.TimeoutAt);
			BeginInvoke(method, new object[] {message});
		}

		public void Consume(TimeoutScheduled message)
		{
			Action<TimeoutScheduled> method = x => AddOrUpdateTimeoutListView(x.CorrelationId, x.Tag, x.TimeoutAt);
			BeginInvoke(method, new object[] {message});
		}

		protected override void OnClosing(CancelEventArgs e)
		{
			_subscriptionServiceEndpoint.Send(new RemoveSubscriptionClient(_clientId, _bus.Endpoint.Address.Uri, _bus.Endpoint.Address.Uri));
			
			_unsubscribe();

			_bus.Dispose();
			_bus = null;

			base.OnClosing(e);
		}

	    private void RefreshHealthView(IEnumerable<HealthInformation> informations)
		{
			var existing = healthListView.Items.Cast<ListViewItem>().ToList();

	    	foreach (HealthInformation entry in informations)
			{
				ListViewItem item = AddOrUpdateHealthItem(entry.ControlUri, entry.LastHeartbeat, entry.State);

				if (existing.Contains(item))
					existing.Remove(item);
			}

			foreach (ListViewItem item in existing)
			{
				item.Remove();
			}
		}

		private ListViewItem AddOrUpdateHealthItem(Uri controlUri, DateTime lastHeartbeat, string state)
		{
			string key = controlUri.ToString();

			ListViewItem item;
			if (healthListView.Items.ContainsKey(key))
			{
				item = healthListView.Items[key];
				item.SubItems[1].Text = lastHeartbeat.ToLocalTime().ToShortTimeString();
				item.SubItems[2].Text = state;
			}
			else
			{
				item = healthListView.Items.Add(key, controlUri.ToString(), 0);

				item.SubItems.Add(new ListViewItem.ListViewSubItem(item, lastHeartbeat.ToLocalTime().ToShortTimeString()));
				item.SubItems.Add(new ListViewItem.ListViewSubItem(item, state));
			}
			return item;
		}

		private void MainFormLoad(object sender, EventArgs e)
		{
			BootstrapContainer();

			BootstrapServiceBus();

			ConnectToSubscriptionService();
		}

		private void ConnectToSubscriptionService()
		{
			_subscriptionServiceEndpoint = _bus.GetEndpoint(_container.GetInstance<IConfiguration>().SubscriptionServiceUri);

			_subscriptionServiceEndpoint.Send(new AddSubscriptionClient(_clientId, _bus.Endpoint.Address.Uri, _bus.Endpoint.Address.Uri));
		}

		private void BootstrapServiceBus()
		{
			_bus = _container.GetInstance<IServiceBus>();
			_unsubscribe = _bus.SubscribeInstance(this);
		}

		private void BootstrapContainer()
		{
			_container = new StructureMap.Container();
			_container.Configure(x =>
				{
					x.For<IConfiguration>()
						.Singleton()
						.Use<Configuration>();
				});

			var registry = new SystemViewRegistry(_container.GetInstance<IConfiguration>());
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

			string description = GetDescription(subscription);

			TreeNode messageNode;
			if (!endpointNode.Nodes.ContainsKey(messageName))
			{
				messageNode = new TreeNode(description);

				if (messageName.StartsWith("MassTransit"))
					messageNode.ForeColor = Color.DimGray;

				messageNode.Name = messageName;
				messageNode.Tag = subscription;

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

        private static string GetDescription(SubscriptionInformation subscription)
        {
            var parts = subscription.MessageName.Split(',');
            var d = parts.Length > 0 ? parts[0] : subscription.MessageName;
            var dd = d.Split('.');

            string description = dd[dd.Length - 1];

            var gs = subscription.MessageName.Split('`');
            if (gs.Length > 1)
            {
                var generics = new Queue<string>(gs.Reverse().Skip(1).Reverse());

                while (generics.Count > 0)
                {
                    var g = generics.Dequeue();
                    var gg = g.Split('.');
                    var ggg = gg.Length > 0 ? gg[gg.Length - 1] : g;

                    description = string.Format("{0}<{1}>", ggg, description);
                }
            }

			if (!string.IsNullOrEmpty(subscription.CorrelationId))
				description += " (" + subscription.CorrelationId + ")";
			return description;
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

			if (endpointNode.Nodes.Count == 0)
			{
				endpointNode.Remove();
			}
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

		private void AddOrUpdateTimeoutListView(Guid correlationId, int tag, DateTime timeoutAt)
		{
			string key = correlationId + "." + tag;

			ListViewItem item;
			if (timeoutListView.Items.ContainsKey(key))
			{
				item = timeoutListView.Items[key];
				item.SubItems[0].Text = timeoutAt.ToLocalTime().ToLongTimeString();
			}
			else
			{
				item = timeoutListView.Items.Add(key, timeoutAt.ToLocalTime().ToLongTimeString(), 0);

				item.SubItems.Add(new ListViewItem.ListViewSubItem(item, correlationId.ToString()));
				item.SubItems.Add(new ListViewItem.ListViewSubItem(item, tag.ToString()));
			}
		}

		private void RemoveTimeoutFromListView(Guid correlationId, int tag)
		{
			string key = correlationId + "." + tag;

			if (timeoutListView.Items.ContainsKey(key))
			{
				timeoutListView.Items.RemoveByKey(key);
			}
		}

        private void SubscriptionViewAfterSelect(object sender, TreeViewEventArgs e)
        {
            subscriptionView.SelectedNode = e.Node;
            endpointInfo.Bind(e.Node.Tag as SubscriptionInformation);
        }

		private void RemoveToolStripMenuItemClick(object sender, EventArgs e)
		{
			if (subscriptionView.SelectedNode != null)
			{
				RemoveSubscriptions(subscriptionView.SelectedNode);
			}
		}

		private void SubscriptionViewPreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
		{
			if (e.KeyCode == Keys.Delete)
			{
				RemoveSubscriptions(subscriptionView.SelectedNode);
			}
		}

		void RemoveSubscriptions(TreeNode node)
		{
			var toRemove = new List<SubscriptionInformation>();
			if (IsRemovable(node))
			{
				toRemove.Add((SubscriptionInformation)node.Tag);
			}

			toRemove.AddRange(node.Nodes.Cast<TreeNode>().Where(IsRemovable).Select(x => x.Tag)
				.Cast<SubscriptionInformation>().ToList());

			if (toRemove.Count == 0)
			{
				return;
			}

			var confirmMessage = string.Format("Are you sure you want to remove these subscriptions?{0}{0}{1}",
				Environment.NewLine, string.Join(Environment.NewLine, toRemove.Select(x =>
				string.Format("{0} -> {1}", x.EndpointUri, GetDescription(x))).ToArray()));

			if (DialogResult.OK != MessageBox.Show(confirmMessage, "Confirm Remove Subscriptions", MessageBoxButtons.OKCancel))
			{
				return;
			}

			toRemove.ForEach(x => _subscriptionServiceEndpoint.Send(new RemoveSubscription(x)));
		}

		private static bool IsRemovable(TreeNode node)
		{
			return node.Tag is SubscriptionInformation &&
				   !((SubscriptionInformation)node.Tag).MessageName.StartsWith("MassTransit.Services");
		}
	}
}