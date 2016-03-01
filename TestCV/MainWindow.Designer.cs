namespace Numerisation_GIST
{
    partial class MainWindow
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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.in_numeriser = new System.Windows.Forms.CheckBox();
            this.in_numeriserOption = new System.Windows.Forms.ComboBox();
            this.in_DPI = new System.Windows.Forms.ComboBox();
            this.btn_sauvegarder = new System.Windows.Forms.Button();
            this.btn_initValider = new System.Windows.Forms.Button();
            this.label9 = new System.Windows.Forms.Label();
            this.lbl_initError = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.in_tailleImgX = new System.Windows.Forms.NumericUpDown();
            this.in_tailleImgY = new System.Windows.Forms.NumericUpDown();
            this.btn_cheminTess = new System.Windows.Forms.Button();
            this.btn_cheminTemp = new System.Windows.Forms.Button();
            this.btn_cheminImages = new System.Windows.Forms.Button();
            this.btn_cheminModeles = new System.Windows.Forms.Button();
            this.in_cheminTess = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.in_cheminTemp = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.in_cheminImages = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.in_cheminModeles = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStrip_Category = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStrip_Info = new System.Windows.Forms.ToolStripStatusLabel();
            this.txb_resultat = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.btn_execute = new System.Windows.Forms.Button();
            this.btn_initialiser = new System.Windows.Forms.Button();
            this.tree_result = new System.Windows.Forms.TreeView();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.in_tailleImgX)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.in_tailleImgY)).BeginInit();
            this.statusStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.in_numeriser);
            this.groupBox1.Controls.Add(this.in_numeriserOption);
            this.groupBox1.Controls.Add(this.in_DPI);
            this.groupBox1.Controls.Add(this.btn_sauvegarder);
            this.groupBox1.Controls.Add(this.btn_initValider);
            this.groupBox1.Controls.Add(this.label9);
            this.groupBox1.Controls.Add(this.lbl_initError);
            this.groupBox1.Controls.Add(this.label7);
            this.groupBox1.Controls.Add(this.in_tailleImgX);
            this.groupBox1.Controls.Add(this.in_tailleImgY);
            this.groupBox1.Controls.Add(this.btn_cheminTess);
            this.groupBox1.Controls.Add(this.btn_cheminTemp);
            this.groupBox1.Controls.Add(this.btn_cheminImages);
            this.groupBox1.Controls.Add(this.btn_cheminModeles);
            this.groupBox1.Controls.Add(this.in_cheminTess);
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.in_cheminTemp);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.in_cheminImages);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.in_cheminModeles);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(377, 257);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Initialisation";
            // 
            // in_numeriser
            // 
            this.in_numeriser.Location = new System.Drawing.Point(9, 19);
            this.in_numeriser.Name = "in_numeriser";
            this.in_numeriser.Size = new System.Drawing.Size(152, 21);
            this.in_numeriser.TabIndex = 30;
            this.in_numeriser.Text = "Numériser ?";
            this.in_numeriser.UseVisualStyleBackColor = true;
            this.in_numeriser.CheckedChanged += new System.EventHandler(this.in_numeriser_CheckedChanged);
            // 
            // in_numeriserOption
            // 
            this.in_numeriserOption.Enabled = false;
            this.in_numeriserOption.FormattingEnabled = true;
            this.in_numeriserOption.Items.AddRange(new object[] {
            "Twain",
            "Windows Image Acquisition"});
            this.in_numeriserOption.Location = new System.Drawing.Point(167, 19);
            this.in_numeriserOption.Name = "in_numeriserOption";
            this.in_numeriserOption.Size = new System.Drawing.Size(173, 21);
            this.in_numeriserOption.TabIndex = 27;
            // 
            // in_DPI
            // 
            this.in_DPI.FormattingEnabled = true;
            this.in_DPI.Items.AddRange(new object[] {
            "240",
            "300",
            "400",
            "600",
            "1200"});
            this.in_DPI.Location = new System.Drawing.Point(167, 46);
            this.in_DPI.Name = "in_DPI";
            this.in_DPI.Size = new System.Drawing.Size(173, 21);
            this.in_DPI.TabIndex = 26;
            // 
            // btn_sauvegarder
            // 
            this.btn_sauvegarder.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btn_sauvegarder.Location = new System.Drawing.Point(167, 228);
            this.btn_sauvegarder.Name = "btn_sauvegarder";
            this.btn_sauvegarder.Size = new System.Drawing.Size(86, 23);
            this.btn_sauvegarder.TabIndex = 22;
            this.btn_sauvegarder.Text = "Sauvegarder";
            this.btn_sauvegarder.UseVisualStyleBackColor = true;
            this.btn_sauvegarder.Click += new System.EventHandler(this.btn_sauvegarder_Click);
            // 
            // btn_initValider
            // 
            this.btn_initValider.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btn_initValider.Location = new System.Drawing.Point(296, 228);
            this.btn_initValider.Name = "btn_initValider";
            this.btn_initValider.Size = new System.Drawing.Size(75, 23);
            this.btn_initValider.TabIndex = 1;
            this.btn_initValider.Text = "Valider";
            this.btn_initValider.UseVisualStyleBackColor = true;
            this.btn_initValider.Click += new System.EventHandler(this.btn_initValider_Click);
            // 
            // label9
            // 
            this.label9.Location = new System.Drawing.Point(346, 151);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(25, 20);
            this.label9.TabIndex = 21;
            this.label9.Text = "px";
            this.label9.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lbl_initError
            // 
            this.lbl_initError.ForeColor = System.Drawing.Color.Red;
            this.lbl_initError.Location = new System.Drawing.Point(6, 200);
            this.lbl_initError.Name = "lbl_initError";
            this.lbl_initError.Size = new System.Drawing.Size(365, 23);
            this.lbl_initError.TabIndex = 19;
            this.lbl_initError.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label7
            // 
            this.label7.Location = new System.Drawing.Point(245, 151);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(17, 20);
            this.label7.TabIndex = 18;
            this.label7.Text = "X";
            this.label7.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // in_tailleImgX
            // 
            this.in_tailleImgX.Location = new System.Drawing.Point(167, 151);
            this.in_tailleImgX.Maximum = new decimal(new int[] {
            100000,
            0,
            0,
            0});
            this.in_tailleImgX.Name = "in_tailleImgX";
            this.in_tailleImgX.Size = new System.Drawing.Size(72, 20);
            this.in_tailleImgX.TabIndex = 17;
            // 
            // in_tailleImgY
            // 
            this.in_tailleImgY.Location = new System.Drawing.Point(268, 151);
            this.in_tailleImgY.Maximum = new decimal(new int[] {
            100000,
            0,
            0,
            0});
            this.in_tailleImgY.Name = "in_tailleImgY";
            this.in_tailleImgY.Size = new System.Drawing.Size(72, 20);
            this.in_tailleImgY.TabIndex = 16;
            // 
            // btn_cheminTess
            // 
            this.btn_cheminTess.Location = new System.Drawing.Point(346, 176);
            this.btn_cheminTess.Name = "btn_cheminTess";
            this.btn_cheminTess.Size = new System.Drawing.Size(25, 20);
            this.btn_cheminTess.TabIndex = 15;
            this.btn_cheminTess.Text = "...";
            this.btn_cheminTess.UseVisualStyleBackColor = true;
            this.btn_cheminTess.Click += new System.EventHandler(this.btn_chemins_Click);
            // 
            // btn_cheminTemp
            // 
            this.btn_cheminTemp.Location = new System.Drawing.Point(346, 125);
            this.btn_cheminTemp.Name = "btn_cheminTemp";
            this.btn_cheminTemp.Size = new System.Drawing.Size(25, 20);
            this.btn_cheminTemp.TabIndex = 14;
            this.btn_cheminTemp.Text = "...";
            this.btn_cheminTemp.UseVisualStyleBackColor = true;
            this.btn_cheminTemp.Click += new System.EventHandler(this.btn_chemins_Click);
            // 
            // btn_cheminImages
            // 
            this.btn_cheminImages.Location = new System.Drawing.Point(346, 98);
            this.btn_cheminImages.Name = "btn_cheminImages";
            this.btn_cheminImages.Size = new System.Drawing.Size(25, 20);
            this.btn_cheminImages.TabIndex = 13;
            this.btn_cheminImages.Text = "...";
            this.btn_cheminImages.UseVisualStyleBackColor = true;
            this.btn_cheminImages.Click += new System.EventHandler(this.btn_chemins_Click);
            // 
            // btn_cheminModeles
            // 
            this.btn_cheminModeles.Location = new System.Drawing.Point(346, 72);
            this.btn_cheminModeles.Name = "btn_cheminModeles";
            this.btn_cheminModeles.Size = new System.Drawing.Size(25, 20);
            this.btn_cheminModeles.TabIndex = 12;
            this.btn_cheminModeles.Text = "...";
            this.btn_cheminModeles.UseVisualStyleBackColor = true;
            this.btn_cheminModeles.Click += new System.EventHandler(this.btn_chemins_Click);
            // 
            // in_cheminTess
            // 
            this.in_cheminTess.Location = new System.Drawing.Point(167, 177);
            this.in_cheminTess.Name = "in_cheminTess";
            this.in_cheminTess.Size = new System.Drawing.Size(173, 20);
            this.in_cheminTess.TabIndex = 11;
            // 
            // label6
            // 
            this.label6.Location = new System.Drawing.Point(6, 177);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(155, 20);
            this.label6.TabIndex = 10;
            this.label6.Text = "Chemin données Tesseract : ";
            this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label5
            // 
            this.label5.Location = new System.Drawing.Point(6, 151);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(155, 20);
            this.label5.TabIndex = 8;
            this.label5.Text = "Taille des images :";
            this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // in_cheminTemp
            // 
            this.in_cheminTemp.Location = new System.Drawing.Point(167, 125);
            this.in_cheminTemp.Name = "in_cheminTemp";
            this.in_cheminTemp.Size = new System.Drawing.Size(173, 20);
            this.in_cheminTemp.TabIndex = 7;
            // 
            // label4
            // 
            this.label4.Location = new System.Drawing.Point(6, 125);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(155, 20);
            this.label4.TabIndex = 6;
            this.label4.Text = "Chemin temporaire :";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // in_cheminImages
            // 
            this.in_cheminImages.Location = new System.Drawing.Point(167, 99);
            this.in_cheminImages.Name = "in_cheminImages";
            this.in_cheminImages.Size = new System.Drawing.Size(173, 20);
            this.in_cheminImages.TabIndex = 5;
            // 
            // label3
            // 
            this.label3.Location = new System.Drawing.Point(6, 99);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(155, 20);
            this.label3.TabIndex = 4;
            this.label3.Text = "Chemin des images à traiter :";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // in_cheminModeles
            // 
            this.in_cheminModeles.Location = new System.Drawing.Point(167, 73);
            this.in_cheminModeles.Name = "in_cheminModeles";
            this.in_cheminModeles.Size = new System.Drawing.Size(173, 20);
            this.in_cheminModeles.TabIndex = 3;
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(6, 73);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(155, 20);
            this.label2.TabIndex = 2;
            this.label2.Text = "Chemin des modèles : ";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(6, 47);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(155, 20);
            this.label1.TabIndex = 0;
            this.label1.Text = "DPI de numérisation :";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStrip_Category,
            this.toolStrip_Info});
            this.statusStrip1.Location = new System.Drawing.Point(0, 276);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(788, 22);
            this.statusStrip1.TabIndex = 1;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // toolStrip_Category
            // 
            this.toolStrip_Category.Name = "toolStrip_Category";
            this.toolStrip_Category.Size = new System.Drawing.Size(102, 17);
            this.toolStrip_Category.Text = "     Tout va bien ....";
            // 
            // toolStrip_Info
            // 
            this.toolStrip_Info.Name = "toolStrip_Info";
            this.toolStrip_Info.Size = new System.Drawing.Size(81, 17);
            this.toolStrip_Info.Text = "Tout va bien...";
            // 
            // txb_resultat
            // 
            this.txb_resultat.Location = new System.Drawing.Point(395, 32);
            this.txb_resultat.Multiline = true;
            this.txb_resultat.Name = "txb_resultat";
            this.txb_resultat.ReadOnly = true;
            this.txb_resultat.Size = new System.Drawing.Size(381, 72);
            this.txb_resultat.TabIndex = 2;
            // 
            // label8
            // 
            this.label8.Location = new System.Drawing.Point(395, 9);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(155, 20);
            this.label8.TabIndex = 22;
            this.label8.Text = "Résultats : ";
            this.label8.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // btn_execute
            // 
            this.btn_execute.Location = new System.Drawing.Point(701, 246);
            this.btn_execute.Name = "btn_execute";
            this.btn_execute.Size = new System.Drawing.Size(75, 23);
            this.btn_execute.TabIndex = 23;
            this.btn_execute.Text = "Executer";
            this.btn_execute.UseVisualStyleBackColor = true;
            this.btn_execute.Click += new System.EventHandler(this.btn_execute_Click);
            // 
            // btn_initialiser
            // 
            this.btn_initialiser.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btn_initialiser.Location = new System.Drawing.Point(620, 246);
            this.btn_initialiser.Name = "btn_initialiser";
            this.btn_initialiser.Size = new System.Drawing.Size(75, 23);
            this.btn_initialiser.TabIndex = 23;
            this.btn_initialiser.Text = "Initialiser";
            this.btn_initialiser.UseVisualStyleBackColor = true;
            this.btn_initialiser.Click += new System.EventHandler(this.btn_initialiser_Click);
            // 
            // tree_result
            // 
            this.tree_result.Location = new System.Drawing.Point(395, 111);
            this.tree_result.Name = "tree_result";
            this.tree_result.Size = new System.Drawing.Size(381, 129);
            this.tree_result.TabIndex = 24;
            // 
            // MainWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(788, 298);
            this.Controls.Add(this.tree_result);
            this.Controls.Add(this.btn_execute);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.btn_initialiser);
            this.Controls.Add(this.txb_resultat);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.groupBox1);
            this.Name = "MainWindow";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.in_tailleImgX)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.in_tailleImgY)).EndInit();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox in_cheminImages;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox in_cheminModeles;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox in_cheminTess;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox in_cheminTemp;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.NumericUpDown in_tailleImgX;
        private System.Windows.Forms.NumericUpDown in_tailleImgY;
        private System.Windows.Forms.Button btn_cheminTess;
        private System.Windows.Forms.Button btn_cheminTemp;
        private System.Windows.Forms.Button btn_cheminImages;
        private System.Windows.Forms.Button btn_cheminModeles;
        private System.Windows.Forms.Label lbl_initError;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Button btn_initValider;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel toolStrip_Info;
        private System.Windows.Forms.ToolStripStatusLabel toolStrip_Category;
        private System.Windows.Forms.TextBox txb_resultat;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Button btn_sauvegarder;
        private System.Windows.Forms.Button btn_execute;
        private System.Windows.Forms.Button btn_initialiser;
        private System.Windows.Forms.ComboBox in_numeriserOption;
        private System.Windows.Forms.ComboBox in_DPI;
        private System.Windows.Forms.CheckBox in_numeriser;
        private System.Windows.Forms.TreeView tree_result;
    }
}