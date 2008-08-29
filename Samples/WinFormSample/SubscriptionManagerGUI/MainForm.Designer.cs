namespace SubscriptionManagerGUI
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
			this._startButton = new System.Windows.Forms.Button();
			this._stopButton = new System.Windows.Forms.Button();
			this._subscriptions = new System.Windows.Forms.TreeView();
			this.splitContainer1 = new System.Windows.Forms.SplitContainer();
			this.splitContainer1.Panel1.SuspendLayout();
			this.splitContainer1.Panel2.SuspendLayout();
			this.splitContainer1.SuspendLayout();
			this.SuspendLayout();
			// 
			// _startButton
			// 
			this._startButton.Location = new System.Drawing.Point(3, 3);
			this._startButton.Name = "_startButton";
			this._startButton.Size = new System.Drawing.Size(75, 23);
			this._startButton.TabIndex = 0;
			this._startButton.Text = "&Start";
			this._startButton.UseVisualStyleBackColor = true;
			this._startButton.Click += new System.EventHandler(this.StartButton_Click);
			// 
			// _stopButton
			// 
			this._stopButton.Location = new System.Drawing.Point(84, 3);
			this._stopButton.Name = "_stopButton";
			this._stopButton.Size = new System.Drawing.Size(75, 23);
			this._stopButton.TabIndex = 1;
			this._stopButton.Text = "S&top";
			this._stopButton.UseVisualStyleBackColor = true;
			this._stopButton.Click += new System.EventHandler(this.StopButton_Click);
			// 
			// _subscriptions
			// 
			this._subscriptions.Dock = System.Windows.Forms.DockStyle.Fill;
			this._subscriptions.Location = new System.Drawing.Point(0, 0);
			this._subscriptions.Name = "_subscriptions";
			this._subscriptions.Size = new System.Drawing.Size(425, 385);
			this._subscriptions.TabIndex = 3;
			// 
			// splitContainer1
			// 
			this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.splitContainer1.IsSplitterFixed = true;
			this.splitContainer1.Location = new System.Drawing.Point(0, 0);
			this.splitContainer1.Name = "splitContainer1";
			this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
			// 
			// splitContainer1.Panel1
			// 
			this.splitContainer1.Panel1.Controls.Add(this._startButton);
			this.splitContainer1.Panel1.Controls.Add(this._stopButton);
			// 
			// splitContainer1.Panel2
			// 
			this.splitContainer1.Panel2.Controls.Add(this._subscriptions);
			this.splitContainer1.Size = new System.Drawing.Size(425, 421);
			this.splitContainer1.SplitterDistance = 32;
			this.splitContainer1.TabIndex = 4;
			// 
			// MainForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(425, 421);
			this.Controls.Add(this.splitContainer1);
			this.Name = "MainForm";
			this.Text = "Subscription Manager";
			this.Load += new System.EventHandler(this.MainForm_Load);
			this.splitContainer1.Panel1.ResumeLayout(false);
			this.splitContainer1.Panel2.ResumeLayout(false);
			this.splitContainer1.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Button _startButton;
		private System.Windows.Forms.Button _stopButton;
		private System.Windows.Forms.TreeView _subscriptions;
		private System.Windows.Forms.SplitContainer splitContainer1;
	}
}

