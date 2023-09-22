using Device.Motion;
using Device.Polhemus;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AcquisitionTest
{
    public partial class Form1 : Form
    {
        PDIClass pDIClass;
        public Form1()
        {
            InitializeComponent();
            pDIClass = new PDIClass();
            pDIClass.OnConnectionStatusChanged += PDIClass_OnConnectionStatusChanged;
            pDIClass.OnNewFrameAvailable += PDIClass_OnNewFrameAvailable;
        }

        private void PDIClass_OnNewFrameAvailable(List<Device.Motion.FRAME> frames)
        {
            FRAME f = frames[0];
            SetText(lblOut, $"S:{f.Sensor} FC:{f.FrameCount:0000} POS:{f.Pos.X:0.00} {f.Pos.Y:0.00} {f.Pos.Z:0.00} ");

        }

        delegate void SetTextDelegate(Label lbl, string text);

        void SetText(Label lbl, string text)
        {
            if (this.InvokeRequired)
            {
                SetTextDelegate d = new SetTextDelegate(SetText);
                this.Invoke(d, new object[] { lbl, text });
            }
            else
            {
                lbl.Text = text;
            }

        }


        private void PDIClass_OnConnectionStatusChanged(Device.Motion.CONNECTIONSTATUS status)
        {
        }

        private void button1_Click(object sender, EventArgs e)
        {

            pDIClass.Connect(this.Handle);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            pDIClass.Disconnect();

        }
    }
}
