using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace PlanetAvoid
{
    public partial class FormPriZapnuti : Form
    {
        public FormPriZapnuti()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            FormHra fh = new FormHra();
            try
            {
                fh.Show();
                fh.InitializeComponent(int.Parse(this.textBox2.Text), int.Parse(this.textBox1.Text));
            }
            catch
            {
                fh.Show();
                fh.InitializeComponent(1024, 768);
            }
            if (this.checkBox1.Checked == !this.checkBox4.Checked)
            {
                Hra.HraSettings.Disco = Hra.HraSettings.DiscoSettings.OnPermamently;
            }
            if (this.checkBox2.Checked)
            {
                Hra.HraSettings.Debug = Hra.HraSettings.DebugSettings.On;
            }
            if (this.checkBox4.Checked)
            {
                Hra.HraSettings.EyeCandySettings.MotionBlur = Hra.HraSettings.EyeCandySettings.MotionBlurSettings.On;
            }

            Hra.Instance(fh.pictureBoxHra);
            this.Hide();
        }

        private void checkBox4_CheckedChanged(object sender, EventArgs e)
        {
            if (this.checkBox4.Checked && this.checkBox1.Checked)
            {
                this.checkBox1.Checked = false;
            }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (this.checkBox1.Checked && this.checkBox4.Checked)
            {
                this.checkBox4.Checked = false;
            }
        }
    }
}
