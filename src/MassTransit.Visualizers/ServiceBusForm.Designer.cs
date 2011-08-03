namespace MassTransit.Visualizers
{
    partial class ServiceBusForm
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
			this.InputContent = new System.Windows.Forms.TextBox();
			this.SuspendLayout();
			// 
			// InputContent
			// 
			this.InputContent.Dock = System.Windows.Forms.DockStyle.Fill;
			this.InputContent.Location = new System.Drawing.Point(0, 0);
			this.InputContent.Multiline = true;
			this.InputContent.Name = "InputContent";
			this.InputContent.ReadOnly = true;
			this.InputContent.ScrollBars = System.Windows.Forms.ScrollBars.Both;
			this.InputContent.Size = new System.Drawing.Size(453, 315);
			this.InputContent.TabIndex = 0;
			// 
			// ServiceBusForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(453, 315);
			this.Controls.Add(this.InputContent);
			this.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.Name = "ServiceBusForm";
			this.Text = "ServiceBus Visualizer";
			this.ResumeLayout(false);
			this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox InputContent;
    }
}