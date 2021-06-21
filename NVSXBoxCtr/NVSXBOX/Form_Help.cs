using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NVSXBOX
{
    public partial class Form_Help : Form
    {
        public Form_Help()
        {
            InitializeComponent();
            ConfigurationButton();

            
        }

        private void ConfigurationButton()
        {
            #region Button Left
            this.BLeft.Parent = this.pictureBox1;
            this.BLeft.BackColor = Color.Transparent;
            this.BLeft.FlatAppearance.MouseDownBackColor = Color.Transparent;
            this.BLeft.FlatAppearance.MouseOverBackColor = Color.FromArgb(100, Color.Green);
            #endregion

            #region Button Mid
            this.BMid.Parent = this.pictureBox1;
            this.BMid.BackColor = Color.Transparent;
            this.BMid.FlatAppearance.MouseDownBackColor = Color.Transparent;
            this.BMid.FlatAppearance.MouseOverBackColor = Color.FromArgb(100, Color.Green);
            #endregion

            #region Button Right
            this.BRight.Parent = this.pictureBox1;
            this.BRight.BackColor = Color.Transparent;
            this.BRight.FlatAppearance.MouseDownBackColor = Color.Transparent;
            this.BRight.FlatAppearance.MouseOverBackColor = Color.FromArgb(100, Color.Green);
            #endregion


        }
        //private void button1_Click(object sender, EventArgs e)
        //{
        //    Close();
        //}

        //private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        //{
        //    System.Diagnostics.Process.Start("https://atlasindustries.com");
        //}

        //private void linkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        //{
        //    System.Diagnostics.Process.Start("https://buildfore.com");
        //}

        private void BLeft_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("https://buildfore.com/ctrlwiz/xboxnavis");
        }

        private void BMid_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("https://buildfore.com/ctrlwiz?utm_source=Navisworks");
        }

        private void BRight_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("https://www.viatechnik.com/");
        }
    }
}
