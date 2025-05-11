using OpenTK;
using OpenTK.Graphics;
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
    public partial class Form1 : Form
    {
        private GLControl glControl;
        private View view;
        private MaterialEditorForm materialSettingsForm;

        public Form1()
        {
            InitializeComponent();

            this.Text = "Ray Tracing";
            this.Width = 600;
            this.Height = 600;

            // Создаем GLControl
            glControl = new GLControl();
            glControl.Dock = DockStyle.Fill;
            glControl.Load += GlControl_Load;
            glControl.Paint += GlControl_Paint;
            glControl.Resize += GlControl_Resize;
            this.Controls.Add(glControl);

            var settingsButton = new Button
            {
                Text = "Material Settings",
                Dock = DockStyle.Top
            };
            settingsButton.Click += (s, e) => ShowMaterialSettings();
            this.Controls.Add(settingsButton);

            view = new View();
        }

        private void GlControl_Load(object sender, EventArgs e)
        {
            view.InitShaders();
            view.SetupScene(glControl);
        }

        private void GlControl_Paint(object sender, PaintEventArgs e)
        {
            view.Render(glControl);
        }

        private void GlControl_Resize(object sender, EventArgs e)
        {
            if (glControl.ClientSize.Height == 0)
            {
                glControl.ClientSize = new Size(glControl.ClientSize.Width, 1);
            }

            GL.Viewport(0, 0, glControl.ClientSize.Width, glControl.ClientSize.Height);
            view.setAspect((float)glControl.Width / glControl.Height);
            glControl.Invalidate();
        }

        private void ShowMaterialSettings()
        {
            materialSettingsForm = new MaterialEditorForm(view);
            materialSettingsForm.btnUpdate.Click += (s, e) =>
            {
                view.Render(glControl);
            };
            materialSettingsForm.Show();
        }
    }
}