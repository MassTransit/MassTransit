namespace ClientGUI
{
    partial class ClientForm
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
            this.panel1 = new System.Windows.Forms.Panel();
            this.client1Received = new System.Windows.Forms.Label();
            this.client1Sent = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.client1WaitTime = new System.Windows.Forms.NumericUpDown();
            this.label2 = new System.Windows.Forms.Label();
            this.label32 = new System.Windows.Forms.Label();
            this.client1Active = new System.Windows.Forms.CheckBox();
            this.client2Received = new System.Windows.Forms.Label();
            this.client2Sent = new System.Windows.Forms.Label();
            this.panel2 = new System.Windows.Forms.Panel();
            this.label5 = new System.Windows.Forms.Label();
            this.client2WaitTime = new System.Windows.Forms.NumericUpDown();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.client2Active = new System.Windows.Forms.CheckBox();
            this.client3Sent = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.panel3 = new System.Windows.Forms.Panel();
            this.client3Received = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.client3WaitTime = new System.Windows.Forms.NumericUpDown();
            this.client3Active = new System.Windows.Forms.CheckBox();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.client1WaitTime)).BeginInit();
            this.panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.client2WaitTime)).BeginInit();
            this.panel3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.client3WaitTime)).BeginInit();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.SystemColors.Window;
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panel1.Controls.Add(this.client1Received);
            this.panel1.Controls.Add(this.client1Sent);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.client1WaitTime);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Controls.Add(this.label32);
            this.panel1.Controls.Add(this.client1Active);
            this.panel1.ForeColor = System.Drawing.SystemColors.WindowText;
            this.panel1.Location = new System.Drawing.Point(12, 12);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(200, 100);
            this.panel1.TabIndex = 0;
            // 
            // client1Received
            // 
            this.client1Received.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.client1Received.Location = new System.Drawing.Point(6, 72);
            this.client1Received.Name = "client1Received";
            this.client1Received.Size = new System.Drawing.Size(114, 15);
            this.client1Received.TabIndex = 5;
            this.client1Received.Text = "0";
            this.client1Received.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // client1Sent
            // 
            this.client1Sent.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.client1Sent.Location = new System.Drawing.Point(6, 55);
            this.client1Sent.Name = "client1Sent";
            this.client1Sent.Size = new System.Drawing.Size(114, 15);
            this.client1Sent.TabIndex = 3;
            this.client1Sent.Text = "0";
            this.client1Sent.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(3, 26);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(94, 15);
            this.label1.TabIndex = 1;
            this.label1.Text = "Wait Time (ms):";
            // 
            // client1WaitTime
            // 
            this.client1WaitTime.Increment = new decimal(new int[] {
            5,
            0,
            0,
            0});
            this.client1WaitTime.Location = new System.Drawing.Point(103, 25);
            this.client1WaitTime.Name = "client1WaitTime";
            this.client1WaitTime.Size = new System.Drawing.Size(49, 20);
            this.client1WaitTime.TabIndex = 2;
            this.client1WaitTime.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.client1WaitTime.Value = new decimal(new int[] {
            5,
            0,
            0,
            0});
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(126, 72);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(58, 15);
            this.label2.TabIndex = 6;
            this.label2.Text = "Received";
            // 
            // label32
            // 
            this.label32.AutoSize = true;
            this.label32.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label32.Location = new System.Drawing.Point(126, 55);
            this.label32.Name = "label32";
            this.label32.Size = new System.Drawing.Size(32, 15);
            this.label32.TabIndex = 4;
            this.label32.Text = "Sent";
            // 
            // client1Active
            // 
            this.client1Active.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.client1Active.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.client1Active.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.client1Active.Location = new System.Drawing.Point(0, 0);
            this.client1Active.Margin = new System.Windows.Forms.Padding(0);
            this.client1Active.Name = "client1Active";
            this.client1Active.Padding = new System.Windows.Forms.Padding(6, 0, 0, 0);
            this.client1Active.Size = new System.Drawing.Size(200, 22);
            this.client1Active.TabIndex = 0;
            this.client1Active.Text = "Client 1";
            this.client1Active.UseVisualStyleBackColor = false;
            this.client1Active.CheckedChanged += new System.EventHandler(this.Client1Active_CheckedChanged);
            // 
            // client2Received
            // 
            this.client2Received.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.client2Received.Location = new System.Drawing.Point(6, 72);
            this.client2Received.Name = "client2Received";
            this.client2Received.Size = new System.Drawing.Size(114, 15);
            this.client2Received.TabIndex = 5;
            this.client2Received.Text = "0";
            this.client2Received.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // client2Sent
            // 
            this.client2Sent.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.client2Sent.Location = new System.Drawing.Point(6, 55);
            this.client2Sent.Name = "client2Sent";
            this.client2Sent.Size = new System.Drawing.Size(114, 15);
            this.client2Sent.TabIndex = 3;
            this.client2Sent.Text = "0";
            this.client2Sent.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.SystemColors.Window;
            this.panel2.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panel2.Controls.Add(this.client2Received);
            this.panel2.Controls.Add(this.client2Sent);
            this.panel2.Controls.Add(this.label5);
            this.panel2.Controls.Add(this.client2WaitTime);
            this.panel2.Controls.Add(this.label6);
            this.panel2.Controls.Add(this.label7);
            this.panel2.Controls.Add(this.client2Active);
            this.panel2.ForeColor = System.Drawing.SystemColors.WindowText;
            this.panel2.Location = new System.Drawing.Point(12, 118);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(200, 100);
            this.panel2.TabIndex = 1;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(3, 26);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(94, 15);
            this.label5.TabIndex = 1;
            this.label5.Text = "Wait Time (ms):";
            // 
            // client2WaitTime
            // 
            this.client2WaitTime.Increment = new decimal(new int[] {
            5,
            0,
            0,
            0});
            this.client2WaitTime.Location = new System.Drawing.Point(103, 25);
            this.client2WaitTime.Name = "client2WaitTime";
            this.client2WaitTime.Size = new System.Drawing.Size(49, 20);
            this.client2WaitTime.TabIndex = 2;
            this.client2WaitTime.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.client2WaitTime.Value = new decimal(new int[] {
            5,
            0,
            0,
            0});
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.Location = new System.Drawing.Point(126, 72);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(58, 15);
            this.label6.TabIndex = 6;
            this.label6.Text = "Received";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label7.Location = new System.Drawing.Point(126, 55);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(32, 15);
            this.label7.TabIndex = 4;
            this.label7.Text = "Sent";
            // 
            // client2Active
            // 
            this.client2Active.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.client2Active.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.client2Active.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.client2Active.Location = new System.Drawing.Point(0, 0);
            this.client2Active.Margin = new System.Windows.Forms.Padding(0);
            this.client2Active.Name = "client2Active";
            this.client2Active.Padding = new System.Windows.Forms.Padding(6, 0, 0, 0);
            this.client2Active.Size = new System.Drawing.Size(200, 22);
            this.client2Active.TabIndex = 0;
            this.client2Active.Text = "Client 2";
            this.client2Active.UseVisualStyleBackColor = false;
            this.client2Active.CheckedChanged += new System.EventHandler(this.Client2Active_CheckedChanged);
            // 
            // client3Sent
            // 
            this.client3Sent.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.client3Sent.Location = new System.Drawing.Point(6, 55);
            this.client3Sent.Name = "client3Sent";
            this.client3Sent.Size = new System.Drawing.Size(114, 15);
            this.client3Sent.TabIndex = 3;
            this.client3Sent.Text = "0";
            this.client3Sent.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(126, 72);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(58, 15);
            this.label4.TabIndex = 6;
            this.label4.Text = "Received";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label8.Location = new System.Drawing.Point(126, 55);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(32, 15);
            this.label8.TabIndex = 4;
            this.label8.Text = "Sent";
            // 
            // panel3
            // 
            this.panel3.BackColor = System.Drawing.SystemColors.Window;
            this.panel3.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panel3.Controls.Add(this.client3Received);
            this.panel3.Controls.Add(this.client3Sent);
            this.panel3.Controls.Add(this.label10);
            this.panel3.Controls.Add(this.client3WaitTime);
            this.panel3.Controls.Add(this.label4);
            this.panel3.Controls.Add(this.label8);
            this.panel3.Controls.Add(this.client3Active);
            this.panel3.ForeColor = System.Drawing.SystemColors.WindowText;
            this.panel3.Location = new System.Drawing.Point(12, 224);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(200, 100);
            this.panel3.TabIndex = 2;
            // 
            // client3Received
            // 
            this.client3Received.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.client3Received.Location = new System.Drawing.Point(6, 72);
            this.client3Received.Name = "client3Received";
            this.client3Received.Size = new System.Drawing.Size(114, 15);
            this.client3Received.TabIndex = 5;
            this.client3Received.Text = "0";
            this.client3Received.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label10.Location = new System.Drawing.Point(3, 26);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(94, 15);
            this.label10.TabIndex = 1;
            this.label10.Text = "Wait Time (ms):";
            // 
            // client3WaitTime
            // 
            this.client3WaitTime.Increment = new decimal(new int[] {
            5,
            0,
            0,
            0});
            this.client3WaitTime.Location = new System.Drawing.Point(103, 25);
            this.client3WaitTime.Name = "client3WaitTime";
            this.client3WaitTime.Size = new System.Drawing.Size(49, 20);
            this.client3WaitTime.TabIndex = 2;
            this.client3WaitTime.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.client3WaitTime.Value = new decimal(new int[] {
            5,
            0,
            0,
            0});
            // 
            // client3Active
            // 
            this.client3Active.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.client3Active.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.client3Active.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.client3Active.Location = new System.Drawing.Point(0, 0);
            this.client3Active.Margin = new System.Windows.Forms.Padding(0);
            this.client3Active.Name = "client3Active";
            this.client3Active.Padding = new System.Windows.Forms.Padding(6, 0, 0, 0);
            this.client3Active.Size = new System.Drawing.Size(200, 22);
            this.client3Active.TabIndex = 0;
            this.client3Active.Text = "Client 3";
            this.client3Active.UseVisualStyleBackColor = false;
            this.client3Active.CheckedChanged += new System.EventHandler(this.client3Active_CheckedChanged);
            // 
            // ClientForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(484, 387);
            this.Controls.Add(this.panel3);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.Name = "ClientForm";
            this.Text = "Client Simulator";
            this.Load += new System.EventHandler(this.ClientForm_Load);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.client1WaitTime)).EndInit();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.client2WaitTime)).EndInit();
            this.panel3.ResumeLayout(false);
            this.panel3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.client3WaitTime)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.CheckBox client1Active;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label32;
        private System.Windows.Forms.NumericUpDown client1WaitTime;
        private System.Windows.Forms.Label client1Received;
        private System.Windows.Forms.Label client1Sent;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label client2Received;
        private System.Windows.Forms.Label client2Sent;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.NumericUpDown client2WaitTime;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.CheckBox client2Active;
        private System.Windows.Forms.Label client3Sent;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Label client3Received;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.NumericUpDown client3WaitTime;
        private System.Windows.Forms.CheckBox client3Active;

    }
}