using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;

namespace tugasakhir
{
    public partial class HOME : Form
    {
        public HOME()
        {
            InitializeComponent();
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

        private void keluarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void tentangPembuatToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form4 frm = new Form4();
            frm.Show();
            frm.StartPosition = FormStartPosition.CenterScreen;
            frm = null;
            this.Hide();
        }

        private void toolStripStatusLabel1_Click(object sender, EventArgs e)
        {

        }

        private void toolStripStatusLabel1_Click_1(object sender, EventArgs e)
        {
            toolStripStatusLabel1.Text = "Yuliani Indah Purnama Sari | NIM E32150934 | Politeknik Negeri Jember | Jurusan Teknologi Informasi | Program Studi Teknik Komputer | .::Aplikasi ini asli karya dari Tugas Akhir Saya::. ";
        }

        private void kELUARToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void pROGRAMERToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form4 frm = new Form4();
            frm.Show();
            frm.StartPosition = FormStartPosition.CenterScreen;
            pROGRAMERToolStripMenuItem.Checked = true;
            frm = null;
            this.Hide();
        }

        private void hOMEToolStripMenuItem_Click(object sender, EventArgs e)
        {
            
        }

        private void tambahanToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form1 frm = new Form1();
            frm.Show();
            frm.StartPosition = FormStartPosition.CenterScreen;
            frm = null;
            this.Hide();
            
        }

        private void label8_Click(object sender, EventArgs e)
        {

        }

        private void kOPIToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form2 frm = new Form2();
            frm.Show();
            frm.StartPosition = FormStartPosition.CenterScreen;
            frm = null;
            this.Hide();
        }
    }
}
