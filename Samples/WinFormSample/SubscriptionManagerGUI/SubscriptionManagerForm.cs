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
namespace SubscriptionManagerGUI
{
	using System;
	using System.Collections.Generic;
	using System.ComponentModel;
	using System.Windows.Forms;
	using log4net;
	using MassTransit.ServiceBus;
	using MassTransit.ServiceBus.Subscriptions;

	public partial class SubscriptionManagerForm : Form
	{
		private static readonly ILog _log = LogManager.GetLogger(typeof (SubscriptionManagerForm));
		private readonly ISubscriptionCache _cache;

		public SubscriptionManagerForm(ISubscriptionCache cache)
		{
			_cache = cache;

			InitializeComponent();
		}

		private void SubscriptionManagerForm_Load(object sender, EventArgs e)
		{
			_cache.OnAddSubscription += SubscriptionAdded;
			_cache.OnRemoveSubscription += SubscriptionRemoved;

			RefreshSubscriptions(null);
		}

		private void SubscriptionAdded(object sender, SubscriptionEventArgs e)
		{
			ThreadSafeUpdate tsu = RefreshSubscriptions;
			BeginInvoke(tsu, new object[] {e.Subscription});
		}

		private void SubscriptionRemoved(object sender, SubscriptionEventArgs e)
		{
			ThreadSafeUpdate tsu = RefreshSubscriptions;
			BeginInvoke(tsu, new object[] {e.Subscription});
		}

		private void RefreshSubscriptions(Subscription ignored)
		{
			List<TreeNode> existingNodes = new List<TreeNode>();
			foreach (TreeNode endpointNode in _subscriptions.Nodes)
			{
				foreach (TreeNode subscriptionNode in endpointNode.Nodes)
				{
					existingNodes.Add(subscriptionNode);
				}
			}

			IList<Subscription> subscriptions = _cache.List();

			foreach (Subscription subscription in subscriptions)
			{
				TreeNode endpointNode;
				if (!_subscriptions.Nodes.ContainsKey(subscription.EndpointUri.ToString()))
				{
					endpointNode = new TreeNode(subscription.EndpointUri.ToString());
					endpointNode.Name = subscription.EndpointUri.ToString();

					_subscriptions.Nodes.Add(endpointNode);
				}
				else
				{
					endpointNode = _subscriptions.Nodes[subscription.EndpointUri.ToString()];
				}

				string messageName = subscription.MessageName;
				string description = subscription.MessageName;
				if (!string.IsNullOrEmpty(subscription.CorrelationId))
					description += " (" + subscription.CorrelationId + ")";

				TreeNode messageNode;
				if (!endpointNode.Nodes.ContainsKey(messageName))
				{
					messageNode = new TreeNode(description);
					messageNode.Name = messageName;

					endpointNode.Nodes.Add(messageNode);
				}
				else
				{
					messageNode = endpointNode.Nodes[messageName];
					if (messageNode.Text != description)
						messageNode.Text = description;
				}

				if (existingNodes.Contains(messageNode))
					existingNodes.Remove(messageNode);
			}

			foreach (TreeNode node in existingNodes)
			{
				node.Remove();
			}
		}

		protected override void OnClosing(CancelEventArgs e)
		{
			base.OnClosing(e);

			e.Cancel = false;
		}

		private delegate void ThreadSafeUpdate(Subscription subscription);
	}
}