namespace MassTransit.SystemView
{
	partial class MainForm
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

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            this.components = new System.ComponentModel.Container();
            this.verticalSplit = new System.Windows.Forms.SplitContainer();
            this.subscriptionView = new System.Windows.Forms.TreeView();
            this.subscriptionsContextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.removeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.horizontalSplit = new System.Windows.Forms.SplitContainer();
            this.innerHorizontalSplit = new System.Windows.Forms.SplitContainer();
            this.timeoutListView = new System.Windows.Forms.ListView();
            this.timeoutExpiresAt = new System.Windows.Forms.ColumnHeader();
            this.timeoutId = new System.Windows.Forms.ColumnHeader();
            this.timeoutTag = new System.Windows.Forms.ColumnHeader();
            this.healthListView = new System.Windows.Forms.ListView();
            this.healthUri = new System.Windows.Forms.ColumnHeader();
            this.healthLastHeartbeat = new System.Windows.Forms.ColumnHeader();
            this.healthState = new System.Windows.Forms.ColumnHeader();
            this.endpointInfo = new MassTransit.SystemView.EndpointInfo();
            this.verticalSplit.Panel1.SuspendLayout();
            this.verticalSplit.Panel2.SuspendLayout();
            this.verticalSplit.SuspendLayout();
            this.subscriptionsContextMenu.SuspendLayout();
            this.horizontalSplit.Panel1.SuspendLayout();
            this.horizontalSplit.Panel2.SuspendLayout();
            this.horizontalSplit.SuspendLayout();
            this.innerHorizontalSplit.Panel1.SuspendLayout();
            this.innerHorizontalSplit.Panel2.SuspendLayout();
            this.innerHorizontalSplit.SuspendLayout();
            this.SuspendLayout();
            // 
            // verticalSplit
            // 
            this.verticalSplit.Dock = System.Windows.Forms.DockStyle.Fill;
            this.verticalSplit.Location = new System.Drawing.Point(0, 0);
            this.verticalSplit.Name = "verticalSplit";
            // 
            // verticalSplit.Panel1
            // 
            this.verticalSplit.Panel1.Controls.Add(this.subscriptionView);
            // 
            // verticalSplit.Panel2
            // 
            this.verticalSplit.Panel2.Controls.Add(this.horizontalSplit);
            this.verticalSplit.Size = new System.Drawing.Size(679, 508);
            this.verticalSplit.SplitterDistance = 227;
            this.verticalSplit.TabIndex = 0;
            // 
            // subscriptionView
            // 
            this.subscriptionView.ContextMenuStrip = this.subscriptionsContextMenu;
            this.subscriptionView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.subscriptionView.Location = new System.Drawing.Point(0, 0);
            this.subscriptionView.Name = "subscriptionView";
            this.subscriptionView.Size = new System.Drawing.Size(227, 508);
            this.subscriptionView.TabIndex = 0;
            this.subscriptionView.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.SubscriptionViewAfterSelect);
            this.subscriptionView.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.SubscriptionViewPreviewKeyDown);
            // 
            // subscriptionsContextMenu
            // 
            this.subscriptionsContextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.removeToolStripMenuItem});
            this.subscriptionsContextMenu.Name = "subscriptionsContextMenu";
            this.subscriptionsContextMenu.Size = new System.Drawing.Size(125, 26);
            // 
            // removeToolStripMenuItem
            // 
            this.removeToolStripMenuItem.Name = "removeToolStripMenuItem";
            this.removeToolStripMenuItem.Size = new System.Drawing.Size(124, 22);
            this.removeToolStripMenuItem.Text = "Remove";
            this.removeToolStripMenuItem.Click += new System.EventHandler(this.RemoveToolStripMenuItemClick);
            // 
            // horizontalSplit
            // 
            this.horizontalSplit.Dock = System.Windows.Forms.DockStyle.Fill;
            this.horizontalSplit.Location = new System.Drawing.Point(0, 0);
            this.horizontalSplit.Name = "horizontalSplit";
            this.horizontalSplit.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // horizontalSplit.Panel1
            // 
            this.horizontalSplit.Panel1.Controls.Add(this.innerHorizontalSplit);
            // 
            // horizontalSplit.Panel2
            // 
            this.horizontalSplit.Panel2.Controls.Add(this.endpointInfo);
            this.horizontalSplit.Size = new System.Drawing.Size(448, 508);
            this.horizontalSplit.SplitterDistance = 242;
            this.horizontalSplit.TabIndex = 0;
            // 
            // innerHorizontalSplit
            // 
            this.innerHorizontalSplit.Dock = System.Windows.Forms.DockStyle.Fill;
            this.innerHorizontalSplit.Location = new System.Drawing.Point(0, 0);
            this.innerHorizontalSplit.Name = "innerHorizontalSplit";
            this.innerHorizontalSplit.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // innerHorizontalSplit.Panel1
            // 
            this.innerHorizontalSplit.Panel1.Controls.Add(this.timeoutListView);
            // 
            // innerHorizontalSplit.Panel2
            // 
            this.innerHorizontalSplit.Panel2.Controls.Add(this.healthListView);
            this.innerHorizontalSplit.Size = new System.Drawing.Size(448, 242);
            this.innerHorizontalSplit.SplitterDistance = 120;
            this.innerHorizontalSplit.TabIndex = 0;
            // 
            // timeoutListView
            // 
            this.timeoutListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.timeoutExpiresAt,
            this.timeoutId,
            this.timeoutTag});
            this.timeoutListView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.timeoutListView.Location = new System.Drawing.Point(0, 0);
            this.timeoutListView.Name = "timeoutListView";
            this.timeoutListView.Size = new System.Drawing.Size(448, 120);
            this.timeoutListView.TabIndex = 1;
            this.timeoutListView.UseCompatibleStateImageBehavior = false;
            this.timeoutListView.View = System.Windows.Forms.View.Details;
            // 
            // timeoutExpiresAt
            // 
            this.timeoutExpiresAt.Text = "Expires At";
            this.timeoutExpiresAt.Width = 120;
            // 
            // timeoutId
            // 
            this.timeoutId.Text = "Id";
            this.timeoutId.Width = 200;
            // 
            // timeoutTag
            // 
            this.timeoutTag.Text = "Tag";
            this.timeoutTag.Width = 80;
            // 
            // healthListView
            // 
            this.healthListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.healthUri,
            this.healthLastHeartbeat,
            this.healthState});
            this.healthListView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.healthListView.Location = new System.Drawing.Point(0, 0);
            this.healthListView.Name = "healthListView";
            this.healthListView.Size = new System.Drawing.Size(448, 118);
            this.healthListView.TabIndex = 1;
            this.healthListView.UseCompatibleStateImageBehavior = false;
            this.healthListView.View = System.Windows.Forms.View.Details;
            // 
            // healthUri
            // 
            this.healthUri.Text = "Endpoint";
            this.healthUri.Width = 240;
            // 
            // healthLastHeartbeat
            // 
            this.healthLastHeartbeat.Text = "Last Update";
            this.healthLastHeartbeat.Width = 100;
            // 
            // healthState
            // 
            this.healthState.Text = "State";
            this.healthState.Width = 100;
            // 
            // endpointInfo
            // 
            this.endpointInfo.Dock = System.Windows.Forms.DockStyle.Fill;
            this.endpointInfo.Location = new System.Drawing.Point(0, 0);
            this.endpointInfo.Name = "endpointInfo";
            this.endpointInfo.Size = new System.Drawing.Size(448, 262);
            this.endpointInfo.TabIndex = 0;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(679, 508);
            this.Controls.Add(this.verticalSplit);
            this.Name = "MainForm";
            this.Text = "MassTransit System View";
            this.Load += new System.EventHandler(this.MainFormLoad);
            this.verticalSplit.Panel1.ResumeLayout(false);
            this.verticalSplit.Panel2.ResumeLayout(false);
            this.verticalSplit.ResumeLayout(false);
            this.subscriptionsContextMenu.ResumeLayout(false);
            this.horizontalSplit.Panel1.ResumeLayout(false);
            this.horizontalSplit.Panel2.ResumeLayout(false);
            this.horizontalSplit.ResumeLayout(false);
            this.innerHorizontalSplit.Panel1.ResumeLayout(false);
            this.innerHorizontalSplit.Panel2.ResumeLayout(false);
            this.innerHorizontalSplit.ResumeLayout(false);
            this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.SplitContainer verticalSplit;
		private System.Windows.Forms.TreeView subscriptionView;
        private System.Windows.Forms.SplitContainer horizontalSplit;
        private System.Windows.Forms.ContextMenuStrip subscriptionsContextMenu;
        private System.Windows.Forms.ToolStripMenuItem removeToolStripMenuItem;
        private System.Windows.Forms.SplitContainer innerHorizontalSplit;
        private System.Windows.Forms.ListView timeoutListView;
        private System.Windows.Forms.ColumnHeader timeoutExpiresAt;
        private System.Windows.Forms.ColumnHeader timeoutId;
        private System.Windows.Forms.ColumnHeader timeoutTag;
        private System.Windows.Forms.ListView healthListView;
        private System.Windows.Forms.ColumnHeader healthUri;
        private System.Windows.Forms.ColumnHeader healthLastHeartbeat;
        private System.Windows.Forms.ColumnHeader healthState;
	    private EndpointInfo endpointInfo;
	}
}

