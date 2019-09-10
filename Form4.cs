using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace tugasakhir
{
    public partial class Form4 : Form
    {
        public Form4()
        {
            InitializeComponent();
        }

        private void pROGRAMERToolStripMenuItem_Click(object sender, EventArgs e)
        {
            
        }

        private void learningToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LEARNING frm = new LEARNING();
            frm.Show();
            frm.StartPosition = FormStartPosition.CenterScreen;
            frm = null;
            this.Hide();
        }

        private void kELUARToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            Application.Exit();

        }

        private void hOMEToolStripMenuItem_Click(object sender, EventArgs e)
        {
            HOME frm = new HOME();
            frm.Show();
            frm.StartPosition = FormStartPosition.CenterScreen;
            frm = null;
            this.Hide();
        }

        private void bANTUANToolStripMenuItem_Click(object sender, EventArgs e)
        {
      

        }

        private void Form4_Load(object sender, EventArgs e)
        {

        }

        private void identifikasiToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form1 frm = new Form1();
            frm.Show();
            frm.StartPosition = FormStartPosition.CenterScreen;
            frm = null;
            this.Hide();
        }

        private void kOPIToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form2 frm = new Form2();
            frm.Show();
            frm.StartPosition = FormStartPosition.CenterScreen;
            frm = null;
            this.Hide();
        }

        private void statusStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
          
        }
    }
}


