namespace Numerisation_GIST
{
    partial class CustomMessageBox
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
            this.btn_left = new System.Windows.Forms.Button();
            this.btn_right = new System.Windows.Forms.Button();
            this.lbl_message = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // btn_left
            // 
            this.btn_left.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btn_left.DialogResult = System.Windows.Forms.DialogResult.Yes;
            this.btn_left.Location = new System.Drawing.Point(12, 59);
            this.btn_left.Name = "btn_left";
            this.btn_left.Size = new System.Drawing.Size(75, 23);
            this.btn_left.TabIndex = 0;
            this.btn_left.Text = "Left";
            this.btn_left.UseVisualStyleBackColor = true;
            // 
            // btn_right
            // 
            this.btn_right.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btn_right.DialogResult = System.Windows.Forms.DialogResult.No;
            this.btn_right.Location = new System.Drawing.Point(164, 59);
            this.btn_right.Name = "btn_right";
            this.btn_right.Size = new System.Drawing.Size(75, 23);
            this.btn_right.TabIndex = 1;
            this.btn_right.Text = "Right";
            this.btn_right.UseVisualStyleBackColor = true;
            // 
            // lbl_message
            // 
            this.lbl_message.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lbl_message.Location = new System.Drawing.Point(12, 9);
            this.lbl_message.Name = "lbl_message";
            this.lbl_message.Size = new System.Drawing.Size(227, 47);
            this.lbl_message.TabIndex = 2;
            this.lbl_message.Text = "Message...";
            this.lbl_message.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // CustomMessageBox
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(251, 94);
            this.Controls.Add(this.lbl_message);
            this.Controls.Add(this.btn_right);
            this.Controls.Add(this.btn_left);
            this.Name = "CustomMessageBox";
            this.Text = "Titre";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btn_left;
        private System.Windows.Forms.Button btn_right;
        private System.Windows.Forms.Label lbl_message;
    }
}