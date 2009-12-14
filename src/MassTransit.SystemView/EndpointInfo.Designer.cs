namespace MassTransit.SystemView
{
    partial class EndpointInfo
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.lblEndpointLabel = new System.Windows.Forms.Label();
            this.tbEndpoint = new System.Windows.Forms.TextBox();
            this.tbClientId = new System.Windows.Forms.TextBox();
            this.lblClientId = new System.Windows.Forms.Label();
            this.tbCorrelationId = new System.Windows.Forms.TextBox();
            this.lblCorrelationId = new System.Windows.Forms.Label();
            this.tbMessageName = new System.Windows.Forms.TextBox();
            this.lblMessageName = new System.Windows.Forms.Label();
            this.tbSequenceNumber = new System.Windows.Forms.TextBox();
            this.lblSequenceNumber = new System.Windows.Forms.Label();
            this.tbSubscriptionId = new System.Windows.Forms.TextBox();
            this.lblSubscriptionId = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // lblEndpointLabel
            // 
            this.lblEndpointLabel.AutoSize = true;
            this.lblEndpointLabel.Location = new System.Drawing.Point(50, 7);
            this.lblEndpointLabel.Name = "lblEndpointLabel";
            this.lblEndpointLabel.Size = new System.Drawing.Size(49, 13);
            this.lblEndpointLabel.TabIndex = 0;
            this.lblEndpointLabel.Text = "Endpoint";
            this.lblEndpointLabel.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // tbEndpoint
            // 
            this.tbEndpoint.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tbEndpoint.Location = new System.Drawing.Point(105, 4);
            this.tbEndpoint.Name = "tbEndpoint";
            this.tbEndpoint.ReadOnly = true;
            this.tbEndpoint.Size = new System.Drawing.Size(335, 20);
            this.tbEndpoint.TabIndex = 1;
            // 
            // tbClientId
            // 
            this.tbClientId.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tbClientId.Location = new System.Drawing.Point(105, 30);
            this.tbClientId.Name = "tbClientId";
            this.tbClientId.ReadOnly = true;
            this.tbClientId.Size = new System.Drawing.Size(335, 20);
            this.tbClientId.TabIndex = 3;
            // 
            // lblClientId
            // 
            this.lblClientId.AutoSize = true;
            this.lblClientId.Location = new System.Drawing.Point(52, 33);
            this.lblClientId.Name = "lblClientId";
            this.lblClientId.Size = new System.Drawing.Size(47, 13);
            this.lblClientId.TabIndex = 2;
            this.lblClientId.Text = "Client ID";
            this.lblClientId.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // tbCorrelationId
            // 
            this.tbCorrelationId.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tbCorrelationId.Location = new System.Drawing.Point(105, 56);
            this.tbCorrelationId.Name = "tbCorrelationId";
            this.tbCorrelationId.ReadOnly = true;
            this.tbCorrelationId.Size = new System.Drawing.Size(335, 20);
            this.tbCorrelationId.TabIndex = 5;
            // 
            // lblCorrelationId
            // 
            this.lblCorrelationId.AutoSize = true;
            this.lblCorrelationId.Location = new System.Drawing.Point(28, 59);
            this.lblCorrelationId.Name = "lblCorrelationId";
            this.lblCorrelationId.Size = new System.Drawing.Size(71, 13);
            this.lblCorrelationId.TabIndex = 4;
            this.lblCorrelationId.Text = "Correlation ID";
            this.lblCorrelationId.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // tbMessageName
            // 
            this.tbMessageName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tbMessageName.Location = new System.Drawing.Point(105, 82);
            this.tbMessageName.Name = "tbMessageName";
            this.tbMessageName.ReadOnly = true;
            this.tbMessageName.Size = new System.Drawing.Size(335, 20);
            this.tbMessageName.TabIndex = 7;
            // 
            // lblMessageName
            // 
            this.lblMessageName.AutoSize = true;
            this.lblMessageName.Location = new System.Drawing.Point(20, 85);
            this.lblMessageName.Name = "lblMessageName";
            this.lblMessageName.Size = new System.Drawing.Size(81, 13);
            this.lblMessageName.TabIndex = 6;
            this.lblMessageName.Text = "Message Name";
            this.lblMessageName.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // tbSequenceNumber
            // 
            this.tbSequenceNumber.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tbSequenceNumber.Location = new System.Drawing.Point(105, 108);
            this.tbSequenceNumber.Name = "tbSequenceNumber";
            this.tbSequenceNumber.ReadOnly = true;
            this.tbSequenceNumber.Size = new System.Drawing.Size(335, 20);
            this.tbSequenceNumber.TabIndex = 9;
            // 
            // lblSequenceNumber
            // 
            this.lblSequenceNumber.AutoSize = true;
            this.lblSequenceNumber.Location = new System.Drawing.Point(3, 111);
            this.lblSequenceNumber.Name = "lblSequenceNumber";
            this.lblSequenceNumber.Size = new System.Drawing.Size(96, 13);
            this.lblSequenceNumber.TabIndex = 8;
            this.lblSequenceNumber.Text = "Sequence Number";
            this.lblSequenceNumber.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // tbSubscriptionId
            // 
            this.tbSubscriptionId.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tbSubscriptionId.Location = new System.Drawing.Point(105, 134);
            this.tbSubscriptionId.Name = "tbSubscriptionId";
            this.tbSubscriptionId.ReadOnly = true;
            this.tbSubscriptionId.Size = new System.Drawing.Size(335, 20);
            this.tbSubscriptionId.TabIndex = 11;
            // 
            // lblSubscriptionId
            // 
            this.lblSubscriptionId.AutoSize = true;
            this.lblSubscriptionId.Location = new System.Drawing.Point(20, 137);
            this.lblSubscriptionId.Name = "lblSubscriptionId";
            this.lblSubscriptionId.Size = new System.Drawing.Size(79, 13);
            this.lblSubscriptionId.TabIndex = 10;
            this.lblSubscriptionId.Text = "Subscription ID";
            this.lblSubscriptionId.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // EndpointInfo
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tbSubscriptionId);
            this.Controls.Add(this.lblSubscriptionId);
            this.Controls.Add(this.tbSequenceNumber);
            this.Controls.Add(this.lblSequenceNumber);
            this.Controls.Add(this.tbMessageName);
            this.Controls.Add(this.lblMessageName);
            this.Controls.Add(this.tbCorrelationId);
            this.Controls.Add(this.lblCorrelationId);
            this.Controls.Add(this.tbClientId);
            this.Controls.Add(this.lblClientId);
            this.Controls.Add(this.tbEndpoint);
            this.Controls.Add(this.lblEndpointLabel);
            this.Name = "EndpointInfo";
            this.Size = new System.Drawing.Size(443, 309);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblEndpointLabel;
        private System.Windows.Forms.TextBox tbEndpoint;
        private System.Windows.Forms.TextBox tbClientId;
        private System.Windows.Forms.Label lblClientId;
        private System.Windows.Forms.TextBox tbCorrelationId;
        private System.Windows.Forms.Label lblCorrelationId;
        private System.Windows.Forms.TextBox tbMessageName;
        private System.Windows.Forms.Label lblMessageName;
        private System.Windows.Forms.TextBox tbSequenceNumber;
        private System.Windows.Forms.Label lblSequenceNumber;
        private System.Windows.Forms.TextBox tbSubscriptionId;
        private System.Windows.Forms.Label lblSubscriptionId;
    }
}
