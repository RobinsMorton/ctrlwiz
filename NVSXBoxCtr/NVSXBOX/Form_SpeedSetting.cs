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
    public partial class Form_SpeedSetting : Form
    {
        private string tooltipvalue1;
        private string tooltipvalue2;
        public Form_SpeedSetting()
        {
            InitializeComponent();

            trackBar1.Minimum = 0;
            trackBar1.Maximum = 100;

            trackBar2.Minimum = 0;
            trackBar2.Maximum = 100;

            trackBar1.Value = (int)(CmdViewpoint.SpeedSetting*100/45);
            trackBar2.Value = (int)(CmdViewpoint.AngularSetting*100/90);


            //System.Windows.Forms.MessageBox.Show(CmdViewpoint.AngularSetting + " deg/s");
            tooltipvalue1 = CmdViewpoint.SpeedSetting + " m/s";
            tooltipvalue2 = CmdViewpoint.AngularSetting + " deg/s";

            label1.Text = tooltipvalue1;
            label2.Text = tooltipvalue2;
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            tooltipvalue1 = CmdViewpoint.SpeedSetting + " m/s";
            toolTip1.SetToolTip(trackBar1,tooltipvalue1);
            label1.Text = tooltipvalue1;

            CmdViewpoint.SpeedSetting = trackBar1.Value * 45 / 100;
            if (CmdViewpoint.oDoc != null)
            {
                var oVP = CmdViewpoint.oDoc.CurrentViewpoint.CreateCopy();
                //Degree to Radian
                oVP.LinearSpeed = CmdViewpoint.SpeedSetting;
                CmdViewpoint.oDoc.CurrentViewpoint.CopyFrom(oVP);
                oVP.Dispose();
            }
        }
        private void trackBar1_ValueChanged(object sender, EventArgs e)
        {
            tooltipvalue1 = CmdViewpoint.SpeedSetting + " m/s";
            toolTip1.SetToolTip(trackBar1, tooltipvalue1);
            label1.Text = tooltipvalue1;

            CmdViewpoint.SpeedSetting = trackBar1.Value * 45/ 100;
            if (CmdViewpoint.oDoc != null)
            {
                var oVP = CmdViewpoint.oDoc.CurrentViewpoint.CreateCopy();
                //Degree to Radian
                oVP.LinearSpeed = CmdViewpoint.SpeedSetting;
                CmdViewpoint.oDoc.CurrentViewpoint.CopyFrom(oVP);
                oVP.Dispose();
            }
        }
//===============================================================================================================================
        private void trackBar2_Scroll(object sender, EventArgs e)
        {
            tooltipvalue2 = CmdViewpoint.AngularSetting + " deg/s";
            toolTip2.SetToolTip(trackBar2,tooltipvalue2);
            label2.Text = tooltipvalue2;

            CmdViewpoint.AngularSetting = trackBar2.Value * 90 / 100;
            if (CmdViewpoint.oDoc != null)
            {
                var oVP = CmdViewpoint.oDoc.CurrentViewpoint.CreateCopy();
                //Degree to Radian
                oVP.AngularSpeed = CmdViewpoint.AngularSetting * Math.PI / 180;
                CmdViewpoint.oDoc.CurrentViewpoint.CopyFrom(oVP);
                oVP.Dispose();
            }
        }
        private void trackBar2_ValueChanged(object sender, EventArgs e)
        {
            tooltipvalue2 = CmdViewpoint.AngularSetting + " deg/s";
            toolTip2.SetToolTip(trackBar2, tooltipvalue2);
            label2.Text = tooltipvalue2;

            CmdViewpoint.AngularSetting = trackBar2.Value * 90 / 100;
            if (CmdViewpoint.oDoc != null)
            {
                var oVP = CmdViewpoint.oDoc.CurrentViewpoint.CreateCopy();
                //Degree to Radian
                oVP.AngularSpeed = CmdViewpoint.AngularSetting * Math.PI / 180;
                CmdViewpoint.oDoc.CurrentViewpoint.CopyFrom(oVP);
                oVP.Dispose();

            }
        }

        private void toolTip1_Popup(object sender, PopupEventArgs e)
        {

        }
    }
}
