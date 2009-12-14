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
    using System.Windows.Forms;
    using Services.Subscriptions.Messages;

    public partial class EndpointInfo : UserControl
    {
        public EndpointInfo()
        {
            InitializeComponent();
        }

        public void Bind(SubscriptionInformation subscriptionInformation)
        {
            if (subscriptionInformation != null)
            {
                tbEndpoint.Text = subscriptionInformation.EndpointUri.ToString();
                tbClientId.Text = subscriptionInformation.ClientId.ToString();
                tbCorrelationId.Text = subscriptionInformation.CorrelationId;
                tbMessageName.Text =  subscriptionInformation.MessageName;
                tbSequenceNumber.Text =  subscriptionInformation.SequenceNumber.ToString();
                tbSubscriptionId.Text = subscriptionInformation.SubscriptionId.ToString();
            }
            else
                Clear();
        }

        private void Clear()
        {
            tbEndpoint.Text = string.Empty;
            tbClientId.Text = string.Empty;
            tbCorrelationId.Text = string.Empty;
            tbMessageName.Text = string.Empty;
            tbSequenceNumber.Text = string.Empty;
            tbSubscriptionId.Text = string.Empty;
        }
    }
}
