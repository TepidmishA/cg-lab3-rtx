using OpenTK;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace cg_lab3_rtx
{
    public partial class MaterialEditorForm : Form
    {
        private View view;
        public Button btnUpdate;

        public MaterialEditorForm(View view)
        {
            this.Text = "Material Editor";
            this.view = view;

            InitializeComponent();
            InitializeMaterials();

            btnUpdate = new Button { Text = "Обновить", Dock = DockStyle.Bottom };
            btnUpdate.Click += BtnUpdate_Click;

            dataGridView_materials.Dock = DockStyle.Fill;
            this.Controls.Add(dataGridView_materials);
            this.Controls.Add(btnUpdate);
        }

        private void InitializeMaterials()
        {
            for (int i = 0; i < view.getTotalMaterials(); i++)
            {
                dataGridView_materials.Rows.Add(i, "1.0", "0.0", "0.0", "0.0", "0.5", "DIFFUSE_REFLECTION");
                Vector3 color = view.GetMaterialColor(i);
                float reflection = view.GetMaterialReflection(i);
                float refraction = view.GetMaterialRefraction(i);
                int type = view.GetMaterialType(i);
                string materialType = type == 1 ? "DIFFUSE_REFLECTION" :
                                       type == 2 ? "MIRROR_REFLECTION" : 
                                       "REFRACTION";
                dataGridView_materials.Rows[i].Cells["Index"].Value = i;
                dataGridView_materials.Rows[i].Cells["colorRed"].Value = color.X;
                dataGridView_materials.Rows[i].Cells["colorGreen"].Value = color.Y;
                dataGridView_materials.Rows[i].Cells["colorBlue"].Value = color.Z;
                dataGridView_materials.Rows[i].Cells["reflectionCoef"].Value = reflection;
                dataGridView_materials.Rows[i].Cells["refractionCoef"].Value = refraction;
                dataGridView_materials.Rows[i].Cells["materialType"].Value = materialType;

            }
        }


        // TODO: переделать 
        private void BtnUpdate_Click(object sender, EventArgs e)
        {
            foreach (DataGridViewRow row in dataGridView_materials.Rows)
            {
                if (row.IsNewRow) continue;

                int index = (int)row.Cells["Index"].Value;

                // Обновление цвета
                Vector3 color = new Vector3(
                    float.Parse(row.Cells["colorRed"].Value.ToString()),
                    float.Parse(row.Cells["colorGreen"].Value.ToString()),
                    float.Parse(row.Cells["colorBlue"].Value.ToString())
                );
                view.UpdateMaterialColor(index, color);

                // Обновление отражения
                float reflection = float.Parse(row.Cells["reflectionCoef"].Value.ToString());
                view.UpdateMaterialReflection(index, reflection);

                // Обновление преломления
                float refraction = float.Parse(row.Cells["refractionCoef"].Value.ToString());
                view.UpdateMaterialRefraction(index, refraction);

                // Обновление типа материала
                string materialType = row.Cells["MaterialType"].Value.ToString();
                int type = materialType == "DIFFUSE_REFLECTION" ? 1 :
                           materialType == "MIRROR_REFLECTION" ? 2 : 3;
                view.UpdateMaterialType(index, type);
            }
        }
    }
}