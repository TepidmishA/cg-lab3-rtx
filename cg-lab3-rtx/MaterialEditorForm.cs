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
        private Button btnUpdate;

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

        // TODO: переделать 
        private void InitializeMaterials()
        {
            // Пример заполнения начальными значениями (должно быть заменено реальными данными из шейдера)
            for (int i = 0; i < 7; i++)
            {
                dataGridView_materials.Rows.Add(i, "1.0", "0.0", "0.0", "0.0", "0.5", "DIFFUSE_REFLECTION");
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
                string[] colorParts = row.Cells["Color"].Value.ToString().Split(',');
                Vector3 color = new Vector3(
                    float.Parse(colorParts[0]),
                    float.Parse(colorParts[1]),
                    float.Parse(colorParts[2])
                );
                view.UpdateMaterialColor(index, color);

                // Обновление коэффициентов освещения
                string[] coeffsParts = row.Cells["LightCoeffs"].Value.ToString().Split(',');
                Vector4 lightCoeffs = new Vector4(
                    float.Parse(coeffsParts[0]),
                    float.Parse(coeffsParts[1]),
                    float.Parse(coeffsParts[2]),
                    float.Parse(coeffsParts[3])
                );
                view.UpdateMaterialLightCoeffs(index, lightCoeffs);

                // Обновление отражения
                float reflection = float.Parse(row.Cells["Reflection"].Value.ToString());
                view.UpdateMaterialReflection(index, reflection);

                // Обновление преломления
                float refraction = float.Parse(row.Cells["Refraction"].Value.ToString());
                view.UpdateMaterialRefraction(index, refraction);

                // Обновление типа материала
                string materialType = row.Cells["MaterialType"].Value.ToString();
                int type = materialType == "DIFFUSE_REFLECTION" ? 1 :
                           materialType == "MIRROR_REFLECTION" ? 2 : 3;
                view.UpdateMaterialType(index, type);
            }

            // Перерисовка сцены (предполагается, что метод Render доступен)
            //view.Render();
        }
    }
}