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
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void pROGRAMERToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form1 frm = new Form1();
            frm.Show();
            frm.StartPosition = FormStartPosition.CenterScreen;
            frm = null;
            this.Hide();
        }

        private void kELUARToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void learningToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LEARNING frm = new LEARNING();
            frm.Show();
            frm.StartPosition = FormStartPosition.CenterScreen;
            frm = null;
            this.Hide();
        }

        private void identifikasiToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form1 frm = new Form1();
            frm.Show();
            frm.StartPosition = FormStartPosition.CenterScreen;
            frm = null;
            this.Hide();
        }

        private void hOMEToolStripMenuItem_Click(object sender, EventArgs e)
        {
            HOME frm = new HOME();
            frm.Show();
            frm.StartPosition = FormStartPosition.CenterScreen;
            frm = null;
            this.Hide();
        }
    }
}
