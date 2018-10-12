namespace PCLaw_To_Staging
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
            this.buttonLawyerSelect = new System.Windows.Forms.Button();
            this.buttonClearLawyers = new System.Windows.Forms.Button();
            this.listViewLawyer = new System.Windows.Forms.ListView();
            this.SuspendLayout();
            // 
            // buttonLawyerSelect
            // 
            this.buttonLawyerSelect.Location = new System.Drawing.Point(132, 322);
            this.buttonLawyerSelect.Name = "buttonLawyerSelect";
            this.buttonLawyerSelect.Size = new System.Drawing.Size(75, 23);
            this.buttonLawyerSelect.TabIndex = 0;
            this.buttonLawyerSelect.Text = "Begin";
            this.buttonLawyerSelect.UseVisualStyleBackColor = true;
            this.buttonLawyerSelect.Click += new System.EventHandler(this.LawyerSelect_Click);
            // 
            // buttonClearLawyers
            // 
            this.buttonClearLawyers.Location = new System.Drawing.Point(112, 269);
            this.buttonClearLawyers.Name = "buttonClearLawyers";
            this.buttonClearLawyers.Size = new System.Drawing.Size(111, 23);
            this.buttonClearLawyers.TabIndex = 1;
            this.buttonClearLawyers.Text = "Clear Selection";
            this.buttonClearLawyers.UseVisualStyleBackColor = true;
            this.buttonClearLawyers.Click += new System.EventHandler(this.buttonClearLawyers_Click);
            // 
            // listViewLawyer
            // 
            this.listViewLawyer.Location = new System.Drawing.Point(35, 26);
            this.listViewLawyer.Name = "listViewLawyer";
            this.listViewLawyer.Size = new System.Drawing.Size(282, 223);
            this.listViewLawyer.TabIndex = 2;
            this.listViewLawyer.UseCompatibleStateImageBehavior = false;
            this.listViewLawyer.ColumnClick += new System.Windows.Forms.ColumnClickEventHandler(this.listViewLawyer_ColumnClick);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(360, 408);
            this.Controls.Add(this.listViewLawyer);
            this.Controls.Add(this.buttonClearLawyers);
            this.Controls.Add(this.buttonLawyerSelect);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button buttonLawyerSelect;
        private System.Windows.Forms.Button buttonClearLawyers;
        private System.Windows.Forms.ListView listViewLawyer;
    }
}

