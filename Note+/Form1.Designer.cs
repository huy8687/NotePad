using System.Windows.Forms;

namespace Note_
{
    partial class Form1
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.lbQues = new System.Windows.Forms.Label();
            this.lbAns = new System.Windows.Forms.Label();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.SuspendLayout();
            // 
            // lbQues
            // 
            this.lbQues.Font = new System.Drawing.Font("Tahoma", 9.7F);
            this.lbQues.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(146)))), ((int)(((byte)(146)))), ((int)(((byte)(146)))));
            this.lbQues.Location = new System.Drawing.Point(3, 0);
            this.lbQues.Name = "lbQues";
            this.lbQues.Size = new System.Drawing.Size(635, 34);
            this.lbQues.TabIndex = 1;
            this.lbQues.Text = resources.GetString("lbQues.Text");
            // 
            // lbAns
            // 
            this.lbAns.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbAns.ForeColor = System.Drawing.SystemColors.ButtonShadow;
            this.lbAns.Location = new System.Drawing.Point(0, 33);
            this.lbAns.Name = "lbAns";
            this.lbAns.Size = new System.Drawing.Size(638, 18);
            this.lbAns.TabIndex = 2;
            // 
            // timer1
            // 
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.ClientSize = new System.Drawing.Size(640, 49);
            this.Controls.Add(this.lbAns);
            this.Controls.Add(this.lbQues);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Form1";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "Notepad";
            this.TopMost = true;
            this.TransparencyKey = System.Drawing.SystemColors.Control;
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label lbQues;
        private System.Windows.Forms.Label lbAns;
        private System.Windows.Forms.Timer timer1;

    }
}

