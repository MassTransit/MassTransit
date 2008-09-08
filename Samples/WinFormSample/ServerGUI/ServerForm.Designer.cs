namespace ServerGUI
{
    partial class ServerForm
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
			this.questionsReceived = new System.Windows.Forms.Label();
			this.panel1 = new System.Windows.Forms.Panel();
			this.answersSent = new System.Windows.Forms.Label();
			this.label1 = new System.Windows.Forms.Label();
			this.serverTime = new System.Windows.Forms.NumericUpDown();
			this.label2 = new System.Windows.Forms.Label();
			this.label32 = new System.Windows.Forms.Label();
			this.answerQuestions = new System.Windows.Forms.CheckBox();
			this.messageTimer = new System.Windows.Forms.Timer(this.components);
			this.panel1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.serverTime)).BeginInit();
			this.SuspendLayout();
			// 
			// questionsReceived
			// 
			this.questionsReceived.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.questionsReceived.Location = new System.Drawing.Point(6, 72);
			this.questionsReceived.Name = "questionsReceived";
			this.questionsReceived.Size = new System.Drawing.Size(114, 15);
			this.questionsReceived.TabIndex = 5;
			this.questionsReceived.Text = "0";
			this.questionsReceived.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// panel1
			// 
			this.panel1.BackColor = System.Drawing.SystemColors.Window;
			this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.panel1.Controls.Add(this.questionsReceived);
			this.panel1.Controls.Add(this.answersSent);
			this.panel1.Controls.Add(this.label1);
			this.panel1.Controls.Add(this.serverTime);
			this.panel1.Controls.Add(this.label2);
			this.panel1.Controls.Add(this.label32);
			this.panel1.Controls.Add(this.answerQuestions);
			this.panel1.ForeColor = System.Drawing.SystemColors.WindowText;
			this.panel1.Location = new System.Drawing.Point(12, 12);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(200, 100);
			this.panel1.TabIndex = 1;
			// 
			// answersSent
			// 
			this.answersSent.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.answersSent.Location = new System.Drawing.Point(6, 55);
			this.answersSent.Name = "answersSent";
			this.answersSent.Size = new System.Drawing.Size(114, 15);
			this.answersSent.TabIndex = 3;
			this.answersSent.Text = "0";
			this.answersSent.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label1.Location = new System.Drawing.Point(3, 26);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(105, 15);
			this.label1.TabIndex = 1;
			this.label1.Text = "Server Time (ms):";
			// 
			// serverTime
			// 
			this.serverTime.Increment = new decimal(new int[] {
            5,
            0,
            0,
            0});
			this.serverTime.Location = new System.Drawing.Point(109, 23);
			this.serverTime.Name = "serverTime";
			this.serverTime.Size = new System.Drawing.Size(49, 20);
			this.serverTime.TabIndex = 2;
			this.serverTime.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.serverTime.ValueChanged += new System.EventHandler(this.serverTime_ValueChanged);
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
			// answerQuestions
			// 
			this.answerQuestions.BackColor = System.Drawing.SystemColors.ActiveCaption;
			this.answerQuestions.Checked = true;
			this.answerQuestions.CheckState = System.Windows.Forms.CheckState.Checked;
			this.answerQuestions.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.answerQuestions.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
			this.answerQuestions.Location = new System.Drawing.Point(-2, -2);
			this.answerQuestions.Margin = new System.Windows.Forms.Padding(0);
			this.answerQuestions.Name = "answerQuestions";
			this.answerQuestions.Padding = new System.Windows.Forms.Padding(6, 0, 0, 0);
			this.answerQuestions.Size = new System.Drawing.Size(200, 22);
			this.answerQuestions.TabIndex = 0;
			this.answerQuestions.Text = "Answer Questions";
			this.answerQuestions.UseVisualStyleBackColor = false;
			this.answerQuestions.CheckedChanged += new System.EventHandler(this.answerQuestions_CheckedChanged);
			// 
			// messageTimer
			// 
			this.messageTimer.Tick += new System.EventHandler(this.messageTimer_Tick);
			// 
			// ServerForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(292, 273);
			this.Controls.Add(this.panel1);
			this.Name = "ServerForm";
			this.Text = "ServerForm";
			this.Load += new System.EventHandler(this.ServerForm_Load);
			this.panel1.ResumeLayout(false);
			this.panel1.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.serverTime)).EndInit();
			this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label questionsReceived;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label answersSent;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.NumericUpDown serverTime;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label32;
        private System.Windows.Forms.CheckBox answerQuestions;
		private System.Windows.Forms.Timer messageTimer;
    }
}

