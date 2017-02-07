namespace PictureMaker
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this._listBox = new System.Windows.Forms.ListBox();
            this.OptionsBox = new System.Windows.Forms.GroupBox();
            this._skipLayoutBox = new System.Windows.Forms.CheckBox();
            this.OptionsBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // _listBox
            // 
            this._listBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this._listBox.FormattingEnabled = true;
            this._listBox.Location = new System.Drawing.Point(0, 0);
            this._listBox.Name = "_listBox";
            this._listBox.Size = new System.Drawing.Size(1119, 628);
            this._listBox.TabIndex = 0;
            // 
            // OptionsBox
            // 
            this.OptionsBox.Controls.Add(this._skipLayoutBox);
            this.OptionsBox.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.OptionsBox.Location = new System.Drawing.Point(0, 528);
            this.OptionsBox.Name = "OptionsBox";
            this.OptionsBox.Size = new System.Drawing.Size(1119, 100);
            this.OptionsBox.TabIndex = 1;
            this.OptionsBox.TabStop = false;
            this.OptionsBox.Text = "Options";
            // 
            // _skipLayoutBox
            // 
            this._skipLayoutBox.AutoSize = true;
            this._skipLayoutBox.Location = new System.Drawing.Point(12, 36);
            this._skipLayoutBox.Name = "_skipLayoutBox";
            this._skipLayoutBox.Size = new System.Drawing.Size(82, 17);
            this._skipLayoutBox.TabIndex = 0;
            this._skipLayoutBox.Text = "Skip Layout";
            this._skipLayoutBox.UseVisualStyleBackColor = true;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1119, 628);
            this.Controls.Add(this.OptionsBox);
            this.Controls.Add(this._listBox);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Form1";
            this.Text = "GraphViz Picture Maker";
            this.OptionsBox.ResumeLayout(false);
            this.OptionsBox.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListBox _listBox;
        private System.Windows.Forms.GroupBox OptionsBox;
        private System.Windows.Forms.CheckBox _skipLayoutBox;
    }
}

