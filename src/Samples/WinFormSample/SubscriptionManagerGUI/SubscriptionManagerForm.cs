namespace SubscriptionManagerGUI
{
	using System;
	using System.Collections.Generic;
	using System.ComponentModel;
	using System.Drawing;
	using System.Windows.Forms;
	using Magnum;
	using MassTransit;
	using MassTransit.Configuration;
	using MassTransit.Services.HealthMonitoring;
	using MassTransit.Services.HealthMonitoring.Messages;
	using MassTransit.Services.Subscriptions.Configuration;
	using MassTransit.Services.Subscriptions.Messages;

	public partial class SubscriptionManagerForm :
		Form,
		Consumes<SubscriptionRefresh>.All,
		Consumes<AddSubscription>.All,
		Consumes<RemoveSubscription>.All,
        Consumes<HealthUpdate>.All
	{
		private readonly IObjectBuilder _builder;
		private IServiceBus _bus;
		private UnsubscribeAction _unsubscribe;
		private IEndpoint _subscriptionServiceEndpoint;
		private Guid _clientId = CombGuid.Generate();

		public SubscriptionManagerForm(IObjectBuilder builder)
		{
			_builder = builder;

			InitializeComponent();
		}

		private void SubscriptionManagerForm_Load(object sender, EventArgs e)
		{
            _bus = ServiceBusConfigurator.New(servicesBus =>
            {
                servicesBus.ReceiveFrom("msmq://localhost/mt_subscription_ui");
            	servicesBus.SetObjectBuilder(_builder);
                servicesBus.ConfigureService<SubscriptionClientConfigurator>(client =>
                {
                    // need to add the ability to read from configuratino settings somehow
                    client.SetSubscriptionServiceEndpoint("msmq://localhost/mt_subscriptions");
                });
            });

			_unsubscribe = _bus.Subscribe(this);


			_subscriptionServiceEndpoint = _builder.GetInstance<IEndpointFactory>().GetEndpoint("msmq://localhost/mt_subscriptions");


			_subscriptionServiceEndpoint.Send(new AddSubscriptionClient(_clientId, _bus.Endpoint.Uri, _bus.Endpoint.Uri));

//			_timeoutRepository.TimeoutAdded += TimeoutRefreshNeeded;
//			_timeoutRepository.TimeoutUpdated += TimeoutRefreshNeeded;
//			_timeoutRepository.TimeoutRemoved += TimeoutRefreshNeeded;
		}

		private void RemoveEvents()
		{
			_unsubscribe();

			_subscriptionServiceEndpoint.Send(new RemoveSubscriptionClient(_clientId, _bus.Endpoint.Uri, _bus.Endpoint.Uri));

			_bus.Dispose();

//			_timeoutRepository.TimeoutAdded -= TimeoutRefreshNeeded;
//			_timeoutRepository.TimeoutUpdated -= TimeoutRefreshNeeded;
//			_timeoutRepository.TimeoutRemoved -= TimeoutRefreshNeeded;
		}

		private void HeartBeatRefreshNeeded(HealthUpdate message)
		{
			ThreadSafeUpdate2 tsu = RefreshHealth;
			BeginInvoke(tsu, new object[] {message});
		}

		private void RefreshHealth(HealthUpdate ignored)
		{
			heartbeatList.Items.Clear();

            foreach (HealthInformation information in ignored.Information)
            {
                var items = new[]
				            	{
				            		information.DataUri.ToString(),
                                    information.State
				            		//information.FirstDetectedAt.Value.ToString("hh:mm:ss"),
				            		//information.LastDetectedAt.Value.ToString("hh:mm:ss")
				            	};
                var lvi = new ListViewItem(items);
                heartbeatList.Items.Add(lvi);
            }
		}

		private TreeNode AddSubscriptionToView(SubscriptionInformation subscription)
		{
			TreeNode endpointNode;
			if (!subscriptionTree.Nodes.ContainsKey(subscription.EndpointUri.ToString()))
			{
				endpointNode = new TreeNode(subscription.EndpointUri.ToString());
				endpointNode.Name = subscription.EndpointUri.ToString();

				subscriptionTree.Nodes.Add(endpointNode);
			}
			else
			{
				endpointNode = subscriptionTree.Nodes[subscription.EndpointUri.ToString()];
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
			if (!subscriptionTree.Nodes.ContainsKey(subscription.EndpointUri.ToString()))
				return;

			TreeNode endpointNode = subscriptionTree.Nodes[subscription.EndpointUri.ToString()];

			string messageName = subscription.MessageName;

			if (!endpointNode.Nodes.ContainsKey(messageName))
				return;

			TreeNode messageNode = endpointNode.Nodes[messageName];

			messageNode.Remove();
		}

		private void RefreshSubscriptions(IEnumerable<SubscriptionInformation> subscriptions)
		{
			var existingNodes = new List<TreeNode>();
			foreach (TreeNode endpointNode in subscriptionTree.Nodes)
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

		private void TimeoutRefreshNeeded(Guid id)
		{
			ThreadSafeTimeoutUpdate tsu = RefreshTimeouts;
			BeginInvoke(tsu, new object[] {id});
		}

		private void RefreshTimeouts(Guid ignored)
		{
			var existing = new List<ListViewItem>();
			foreach (ListViewItem item in timeoutList.Items)
			{
				existing.Add(item);
			}

//			foreach (var item in _timeoutRepository.List())
//			{
//				string key = item.Key.ToString();
//
//				if (!timeoutList.Items.ContainsKey(key))
//				{
//					var listItem = new ListViewItem(item.Value.ToLocalTime().ToLongTimeString());
//
//					listItem.SubItems.Add(new ListViewItem.ListViewSubItem(listItem, key));
//
//					timeoutList.Items.Add(listItem);
//				}
//				else
//				{
//					ListViewItem listViewItem = timeoutList.Items[key];
//					listViewItem.SubItems[0].Text = item.Value.ToLocalTime().ToLongTimeString();
//
//					existing.Remove(listViewItem);
//				}
//			}

			foreach (ListViewItem item in existing)
			{
				item.Remove();
			}
		}

		protected override void OnClosing(CancelEventArgs e)
		{
			base.OnClosing(e);

			e.Cancel = false;

			RemoveEvents();
		}

		#region Nested type: ThreadSafeTimeoutUpdate

		private delegate void ThreadSafeTimeoutUpdate(Guid id);

		#endregion

		#region Nested type: ThreadSafeUpdate2

		private delegate void ThreadSafeUpdate2(HealthUpdate information);

		#endregion

        public void Consume(HealthUpdate message)
        {
            HeartBeatRefreshNeeded(message);
        }
		public void Consume(SubscriptionRefresh message)
		{
			Action<IEnumerable<SubscriptionInformation>> method = x => RefreshSubscriptions(x);
			BeginInvoke(method, new object[] { message.Subscriptions });
		}

		public void Consume(AddSubscription message)
		{
			Action<SubscriptionInformation> method = x => AddSubscriptionToView(x);
			BeginInvoke(method, new object[] { message.Subscription });
		}

		public void Consume(RemoveSubscription message)
		{
			Action<SubscriptionInformation> method = x => RemoveSubscriptionFromView(x);
			BeginInvoke(method, new object[] { message.Subscription });
		}
	}
}