﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TST.Vision.Thirdparty;

namespace TSTEmguCVDemo
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Open_Click(object sender, EventArgs e)
        {
            OpenFileDialog fm = new OpenFileDialog();
            fm.Filter = "BMP|*.bmp";
            if (fm.ShowDialog() == DialogResult.OK)
            {
                string filename = fm.FileName;
                object imageobj = TST.Vision.Thirdparty.CvTypeConvert.GetImag(filename);
                this.cvControlEx1.DislpayObj(imageobj);
            }
        }
    }
}
