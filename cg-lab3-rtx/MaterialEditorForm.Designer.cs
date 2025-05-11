namespace cg_lab3_rtx
{
    partial class MaterialEditorForm
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
            this.dataGridView_materials = new System.Windows.Forms.DataGridView();
            this.Index = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colorRed = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colorGreen = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colorBlue = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.reflectionCoef = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.refractionCoef = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.materialType = new System.Windows.Forms.DataGridViewComboBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView_materials)).BeginInit();
            this.SuspendLayout();
            // 
            // dataGridView_materials
            // 
            this.dataGridView_materials.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView_materials.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Index,
            this.colorRed,
            this.colorGreen,
            this.colorBlue,
            this.reflectionCoef,
            this.refractionCoef,
            this.materialType});
            this.dataGridView_materials.Location = new System.Drawing.Point(12, 12);
            this.dataGridView_materials.Name = "dataGridView_materials";
            this.dataGridView_materials.Size = new System.Drawing.Size(708, 236);
            this.dataGridView_materials.TabIndex = 0;
            // 
            // Index
            // 
            this.Index.HeaderText = "Index";
            this.Index.Name = "Index";
            this.Index.ReadOnly = true;
            this.Index.Width = 50;
            // 
            // colorRed
            // 
            this.colorRed.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.ColumnHeader;
            this.colorRed.HeaderText = "Color Red (0 -> 1)";
            this.colorRed.Name = "colorRed";
            this.colorRed.Width = 87;
            // 
            // colorGreen
            // 
            this.colorGreen.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.ColumnHeader;
            this.colorGreen.HeaderText = "Color Green (0 -> 1)";
            this.colorGreen.Name = "colorGreen";
            this.colorGreen.Width = 95;
            // 
            // colorBlue
            // 
            this.colorBlue.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.ColumnHeader;
            this.colorBlue.HeaderText = "Color Blue (0 -> 1)";
            this.colorBlue.Name = "colorBlue";
            this.colorBlue.Width = 88;
            // 
            // reflectionCoef
            // 
            this.reflectionCoef.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.ColumnHeader;
            this.reflectionCoef.HeaderText = "Reflection";
            this.reflectionCoef.Name = "reflectionCoef";
            this.reflectionCoef.Width = 80;
            // 
            // refractionCoef
            // 
            this.refractionCoef.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.ColumnHeader;
            this.refractionCoef.HeaderText = "Refraction";
            this.refractionCoef.Name = "refractionCoef";
            this.refractionCoef.Width = 81;
            // 
            // materialType
            // 
            this.materialType.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.materialType.HeaderText = "Material Type";
            this.materialType.Items.AddRange(new object[] {
            "DIFFUSE_REFLECTION",
            "MIRROR_REFLECTION",
            "REFRACTION"});
            this.materialType.Name = "materialType";
            // 
            // MaterialEditorForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(736, 343);
            this.Controls.Add(this.dataGridView_materials);
            this.Name = "MaterialEditorForm";
            this.Text = "MaterialEditorForm";
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView_materials)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView dataGridView_materials;
        private System.Windows.Forms.DataGridViewTextBoxColumn Index;
        private System.Windows.Forms.DataGridViewTextBoxColumn colorRed;
        private System.Windows.Forms.DataGridViewTextBoxColumn colorGreen;
        private System.Windows.Forms.DataGridViewTextBoxColumn colorBlue;
        private System.Windows.Forms.DataGridViewTextBoxColumn reflectionCoef;
        private System.Windows.Forms.DataGridViewTextBoxColumn refractionCoef;
        private System.Windows.Forms.DataGridViewComboBoxColumn materialType;
    }
}