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
    using MassTransit.ServiceBus;
    using MassTransit.ServiceBus.Services.HealthMonitoring;
    using MassTransit.ServiceBus.Services.Timeout;
    using MassTransit.ServiceBus.Subscriptions;

    public partial class SubscriptionManagerForm : Form
    {
        private readonly ISubscriptionCache _cache;
        private readonly IHealthCache _healthCache;
        private readonly ITimeoutRepository _timeoutRepository;

        public SubscriptionManagerForm(ISubscriptionCache cache, ITimeoutRepository timeoutStorage,
                                       IHealthCache healthCache)
        {
            _cache = cache;
            _timeoutRepository = timeoutStorage;
            _healthCache = healthCache;

            InitializeComponent();
        }

        private void SubscriptionManagerForm_Load(object sender, EventArgs e)
        {
            _cache.OnAddSubscription += SubscriptionAdded;
            _cache.OnRemoveSubscription += SubscriptionRemoved;

            RefreshSubscriptions(null);

            _timeoutRepository.TimeoutAdded += TimeoutRefreshNeeded;
            _timeoutRepository.TimeoutUpdated += TimeoutRefreshNeeded;
            _timeoutRepository.TimeoutRemoved += TimeoutRefreshNeeded;

            _healthCache.NewHealthInformation += HeartBeatRefreshNeeded;
            _healthCache.UpdatedHealthInformation += HeartBeatRefreshNeeded;
        }

        private void HeartBeatRefreshNeeded(HealthInformation obj)
        {
            ThreadSafeUpdate2 tsu = RefreshHealth;
            BeginInvoke(tsu, new object[] {obj});
        }

        private void RefreshHealth(HealthInformation ignored)
        {
            heartbeatList.Items.Clear();

            foreach (HealthInformation information in _healthCache.List())
            {
                var items = new[]
                                {
                                    information.Uri.ToString(),
                                    information.FirstDetectedAt.Value.ToString(),
                                    information.LastDetectedAt.Value.ToLongTimeString()
                                };
                var lvi = new ListViewItem(items);
                heartbeatList.Items.Add(lvi);
            }
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
            var existingNodes = new List<TreeNode>();
            foreach (TreeNode endpointNode in subscriptionTree.Nodes)
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

            foreach (var item in _timeoutRepository.List())
            {
                string key = item.Key.ToString();

                if (!timeoutList.Items.ContainsKey(key))
                {
                    var listItem = new ListViewItem(item.Value.ToLocalTime().ToLongTimeString());

                    listItem.SubItems.Add(new ListViewItem.ListViewSubItem(listItem, key));

                    timeoutList.Items.Add(listItem);
                }
                else
                {
                    ListViewItem listViewItem = timeoutList.Items[key];
                    listViewItem.SubItems[0].Text = item.Value.ToLocalTime().ToLongTimeString();

                    existing.Remove(listViewItem);
                }
            }

            foreach (ListViewItem item in existing)
            {
                item.Remove();
            }
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);

            e.Cancel = false;
        }

        #region Nested type: ThreadSafeTimeoutUpdate

        private delegate void ThreadSafeTimeoutUpdate(Guid id);

        #endregion

        #region Nested type: ThreadSafeUpdate

        private delegate void ThreadSafeUpdate(Subscription subscription);

        #endregion

        #region Nested type: ThreadSafeUpdate2

        private delegate void ThreadSafeUpdate2(HealthInformation information);

        #endregion
    }
}