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
    public partial class FormDebug : Form
    {
        private CmdViewpoint cmdViewpoint;
        public FormDebug(CmdViewpoint cmdvp)
        {
            InitializeComponent();
            cmdViewpoint = cmdvp;
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void FormDebug_Load(object sender, EventArgs e)
        {

        }
    }
}
