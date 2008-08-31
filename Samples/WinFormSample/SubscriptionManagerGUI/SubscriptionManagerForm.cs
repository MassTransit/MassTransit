namespace SubscriptionManagerGUI
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Windows.Forms;
    using Castle.Core;
    using Castle.Windsor;
    using log4net;
    using MassTransit.ServiceBus;
    using MassTransit.ServiceBus.Subscriptions;
    using MassTransit.WindsorIntegration;

    public partial class SubscriptionManagerForm : Form
    {
        private static readonly ILog _log = LogManager.GetLogger(typeof (SubscriptionManagerForm));

        private IWindsorContainer _container;
        private IServiceBus _subscriptionBus;
        private ISubscriptionCache _subscriptionCache;
        private SubscriptionService _subscriptionService;

        private delegate void ThreadSafeUpdate(Subscription subscription);

        public SubscriptionManagerForm()
        {
            InitializeComponent();
        }

        private void SubscriptionManagerForm_Load(object sender, EventArgs e)
        {
            StartSubscriptionManager();
        }

        private void StartSubscriptionManager()
        {
            StopSubscriptionManager();

            try
            {
                _startButton.Enabled = false;
                _stopButton.Enabled = true;

                _container = new DefaultMassTransitContainer("SubscriptionManager.Castle.xml");

                _subscriptionCache = _container.Resolve<ISubscriptionCache>();
                _subscriptionCache.OnAddSubscription += SubscriptionAdded;
                _subscriptionCache.OnRemoveSubscription += SubscriptionRemoved;

                _container.AddComponentLifeStyle("followerRepository", typeof (FollowerRepository), LifestyleType.Singleton);

                _subscriptionBus = _container.Resolve<IServiceBus>("subscriptions");
                _subscriptionService = _container.Resolve<SubscriptionService>();
                _subscriptionService.Start();
            }
            catch (Exception ex)
            {
                _log.Error(ex);
                MessageBox.Show("The subscription service failed to start:\n\n" + ex.Message);
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
            BeginInvoke(tsu, new object[] { e.Subscription });
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

            IList<Subscription> subscriptions = _subscriptionCache.List();

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

        private void StopSubscriptionManager()
        {
            try
            {
                if (_subscriptionService != null)
                {
                    _subscriptionService.Stop();
                    _subscriptionService.Dispose();
                }
            }
            catch (Exception ex)
            {
                _log.Error(ex);
            }

            try
            {
                if (_subscriptionBus != null)
                {
                    _subscriptionCache.OnAddSubscription -= SubscriptionAdded;
                    _subscriptionCache.OnRemoveSubscription -= SubscriptionRemoved;

                    _subscriptionBus.Dispose();
                    _subscriptionBus = null;
                }
            }
            catch (Exception ex)
            {
                _log.Error(ex);
            }

            try
            {
                if (_container != null)
                {
                    _container.Dispose();
                    _container = null;
                }
            }
            catch (Exception ex)
            {
                _log.Error(ex);
            }

            _startButton.Enabled = true;
            _stopButton.Enabled = false;
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);

            StopSubscriptionManager();

            e.Cancel = false;
        }

        private void StartButton_Click(object sender, EventArgs e)
        {
            StartSubscriptionManager();
        }

        private void StopButton_Click(object sender, EventArgs e)
        {
            StopSubscriptionManager();
        }
    }
}