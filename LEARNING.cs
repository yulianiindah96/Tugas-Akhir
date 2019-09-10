using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using AForge;
using AForge.Imaging;
using AForge.Imaging.Filters;
using AForge.Imaging.Formats;
using AForge.Video;
using AForge.Video.DirectShow;
using AForge.Math.Geometry;
using System.Data.OleDb;
using tugasakhir;
using tugasakhir.deteksi;


namespace tugasakhir
{

    public partial class LEARNING : Form
    {   /// <summary>
        /// CLASS MENENTUKAN DATABASE YANG DIGUNAKAN
        /// </summary>
        public string database = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=|DataDirectory|/DataKopi.mdb";
        public static DataSet data = new DataSet();
        private Bitmap image = null;
        private Bitmap imagestat = null;
        int jumlahdt;
        FilterInfoCollection _fInfoCollection;
        VideoCaptureDevice _vCaptureDevice;
        
        /// <summary>
        /// CLASS MENAMPILKAN DATABASE
        /// </summary>
        void panggildatabase()
        {
            OleDbConnection koneksi = new OleDbConnection("Provider=Microsoft.Jet.OLEDB.4.0;Data Source=|DataDirectory|/DataKopi.mdb");
            OleDbCommand command = new OleDbCommand("SELECT *from tabel_utama");
            try
            {
                koneksi.Open();
                command.Connection = koneksi;
                command.CommandType = CommandType.Text;
                command.ExecuteNonQuery();
                DataSet data1 = new DataSet();
                OleDbDataAdapter dt = new OleDbDataAdapter(command);
                dt.Fill(data, "utama");
                dataGridView1.DataSource = data;
                dataGridView1.DataMember = "utama";
                koneksi.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        /// <summary>
        /// CLASS TAMBAH DATA KE DATABASE
        /// </summary>
        void tambahdata()
        {
            OleDbConnection koneksi = new OleDbConnection("Provider=Microsoft.Jet.OLEDB.4.0;Data Source=|DataDirectory|/DataKopi.mdb");
            OleDbCommand command = new OleDbCommand("SELECT * from tabel_utama");
            try
            {
                koneksi.Open();
                command.Connection = koneksi;
                command.CommandType = CommandType.Text;
                command.CommandText = "INSERT into tabel_utama (Gambar_Buah) values('" + jumlahdt.ToString() + ".bmp" + "')";
                command.ExecuteNonQuery();
                koneksi.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            data.Clear();
            panggildatabase();
            dataGridView1.DataSource = data;
            dataGridView1.DataMember = "utama";
        }
        /// <summary>
        /// CLASS MENENTUKAN JUMLAH DATA DALAM DATABASE
        /// </summary>
        void jumlahdata()
        {
            OleDbConnection koneksi = new OleDbConnection("Provider=Microsoft.Jet.OLEDB.4.0;Data Source=|DataDirectory|/DataKopi.mdb");
            OleDbCommand command = new OleDbCommand("SELECT * from tabel_utama");
            try
            {
                koneksi.Open();
                command.Connection = koneksi;
                command.CommandType = CommandType.Text;
                command.CommandText = "SELECT COUNT (*) from tabel_utama";
                jumlahdt = (int)command.ExecuteScalar();
                //label19.Text = jumlahdt.ToString();
                koneksi.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        /// <summary>
        /// CLASS MENGUBAH DATA SETELAH MENYIMPAN NAMA GAMBAR 
        /// </summary>
        public void updatedata()
        {
            imagestat = (Bitmap)pictureBox3.Image.Clone();
            ColorImageStatisticsDescription statDesc = new ColorImageStatisticsDescription(imagestat);
            OleDbConnection koneksi = new OleDbConnection("Provider=Microsoft.Jet.OLEDB.4.0;Data Source=|DataDirectory|/DataKopi.mdb");
            OleDbCommand command = new OleDbCommand("UPDATE tabel_utama SET [Tingkat_Kematangan] = '" + comboBox1.SelectedItem + "',[Mean_Kadar_R] = '" + statDesc.RedMean.ToString() + "',[StDev_Kadar_R] = '" + statDesc.RedStdDev.ToString() + "',[Min_Kadar_R] ='" + statDesc.RedMin.ToString() + "',[Max_Kadar_R] = '" + statDesc.RedMax.ToString() + "',[Mean_Kadar_G] = '" + statDesc.GreenMean.ToString() + "',[StDev_Kadar_G] = '" + statDesc.GreenStdDev.ToString() + "',[Min_Kadar_G] ='" + statDesc.GreenMin.ToString() + "',[Max_Kadar_G] = '" + statDesc.GreenMax.ToString() + "',[Mean_Kadar_B] = '" + statDesc.BlueMean.ToString() + "',[StDev_Kadar_B] = '" + statDesc.BlueStdDev.ToString() + "',[Min_Kadar_B] ='" + statDesc.BlueMin.ToString() + "',[Max_Kadar_B] = '" + statDesc.BlueMax.ToString() + "' WHERE Gambar_Buah = '" + label14.Text + "'");
            try
            {
                koneksi.Open();
                command.Connection = koneksi;
                command.CommandType = CommandType.Text;
                command.ExecuteNonQuery();
                MessageBox.Show("Saved!");
                koneksi.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            data.Clear();
            panggildatabase();
            dataGridView1.DataSource = data;
            dataGridView1.DataMember = "utama";
            dataGridView1.Refresh();
        }
        /// <summary>
        /// CLASS MENENTUKAN NILAI RGB PADA OBJEK DENGAN BLACK
        /// </summary>
        public class ColorImageStatisticsDescription
        {
            public ImageStatistics statRGB;
            /*[Category("0: General")]
            public int PixelsCount
            {
                get { return statRGB.PixelsCount; }
            }
            [Category("0: General")]
            public int PixelsWithoutBlack
            {
                get { return statRGB.PixelsCountWithoutBlack; }
            }*/	
            [Category("1: Red Dengan black")]
            public int RedMin
            {
                get { return statRGB.Red.Min; }
            }
            [Category("1: Red Dengan black")]
            public int RedMax
            {
                get { return statRGB.Red.Max; }
            }
            [Category("1: Red Dengan black")]
            public double RedMean
            {
                get { return statRGB.Red.Mean; }
            }
            [Category("1: Red Dengan black")]
            public double RedStdDev
            {
                get { return statRGB.Red.StdDev; }
            }
            /*[Category("1: Red Dengan black")]
            public int RedMedian
            {
                get { return statRGB.Red.Median; }
            }*/
            [Category("2: Green Dengan black")]
            public int GreenMin
            {
                get { return statRGB.Green.Min; }
            }
            [Category("2: Green Dengan black")]
            public int GreenMax
            {
                get { return statRGB.Green.Max; }
            }
            [Category("2: Green Dengan black")]
            public double GreenMean
            {
                get { return statRGB.Green.Mean; }
            }
            [Category("2: Green Dengan black")]
            public double GreenStdDev
            {
                get { return statRGB.Green.StdDev; }
            }
            /*[Category("2: Green Dengan black")]
            public int GreenMedian
            {
                get { return statRGB.Green.Median; }
            }*/
            [Category("3: Blue Dengan black")]
            public int BlueMin
            {
                get { return statRGB.Blue.Min; }
            }
            [Category("3: Blue Dengan black")]
            public int BlueMax
            {
                get { return statRGB.Blue.Max; }
            }
            [Category("3: Blue Dengan black")]
            public double BlueMean
            {
                get { return statRGB.Blue.Mean; }
            }
            [Category("3: Blue Dengan black")]
            public double BlueStdDev
            {
                get { return statRGB.Blue.StdDev; }
            }
            /*[Category("3: Blue Dengan black")]
            public int BlueMedian
            {
                get { return statRGB.Blue.Median; }
            }*/
            
            
            /// <summary>
            /// CLASS MEMBANGUN NILAI RGB
            /// </summary>
            public ColorImageStatisticsDescription(Bitmap imagestat)
            {
                //membuat variabel dimensi gambar untuk waktu yg sebaik-baiknya
                int width = imagestat.Width;
                int height = imagestat.Height;
                //Bit kunci untuk sistem memori supaya pemrosesan cepat
                BitmapData imgData = imagestat.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);
                //Mengumpulkan statistik
                statRGB = new ImageStatistics(new UnmanagedImage(imgData));
                // unlock image
                imagestat.UnlockBits(imgData);
            }
        }
        /// <summary>
        /// CLASS MENENTUKAN DILASI
        /// </summary>
        private Bitmap Morph(Bitmap srcImg, int kernelSize)
        {
            //membuat variabel dimensi gambar untuk waktu yg sebaik-baiknya
            int width = srcImg.Width;
            int height = srcImg.Height;
            //Bit kunci untuk sistem memori supaya pemrosesan cepat
            Rectangle canvas = new Rectangle(0, 0, width, height);
            BitmapData srcData = srcImg.LockBits(canvas, ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
            int stride = srcData.Stride;
            int bytes = stride * srcData.Height;
            //membuat byte array yang akan memegang semua data piksel, satu untuk pemrosesan, satu untuk output
            byte[] pixelBuffer = new byte[bytes];
            byte[] resultBuffer = new byte[bytes];
            //menulis data piksel ke array maksudnya untuk pemrosesan
            Marshal.Copy(srcData.Scan0, pixelBuffer, 0, bytes);//ini
            srcImg.UnlockBits(srcData);
            //mengaplikasikan dilasi
            int kernelDim = kernelSize;
            int kernelOffset = (kernelDim - 1) / 2;
            int calcOffset = 0;
            int byteOffset = 0;
            byte blue = 0;
            byte green = 0;
            byte red = 0;
            for (int y = kernelOffset; y < height - kernelOffset; y++)
            {
                for (int x = kernelOffset; x < width - kernelOffset; x++)
                {
                    //byte value = 0;
                    byteOffset = y * stride + x * 4;
                    blue = 0;
                    green = 0;
                    red = 0;
                    //apply dilation 
                    for (int ykernel = -kernelOffset; ykernel <= kernelOffset; ykernel++)
                    {
                        for (int xkernel = -kernelOffset; xkernel <= kernelOffset; xkernel++)
                        {
                            calcOffset = byteOffset + (xkernel * 4) + (ykernel * stride);
                            if (pixelBuffer[calcOffset] > blue)
                            {
                                blue = pixelBuffer[calcOffset];
                            }
                            if (pixelBuffer[calcOffset + 1] > green)
                            {
                                green = pixelBuffer[calcOffset + 1];
                            }
                            if (pixelBuffer[calcOffset + 2] > red)
                            {
                                red = pixelBuffer[calcOffset + 2];
                            }
                        }
                    }
                    //menulis data yang terproses kedalam array kedua
                    resultBuffer[byteOffset] = blue;
                    resultBuffer[byteOffset + 1] = green;
                    resultBuffer[byteOffset + 2] = red;
                    resultBuffer[byteOffset + 3] = 255;
                }
            }
            //membuat output bitmap pada fungsi ini
            Bitmap rsltImg = new Bitmap(width, height);
            BitmapData rsltData = rsltImg.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);
            //menulis data yang terproses kedalam form bitmap
            Marshal.Copy(resultBuffer, 0, rsltData.Scan0, bytes);
            rsltImg.UnlockBits(rsltData);
            return rsltImg;
        }
        /// <summary>
        /// CLASS MENENTUKAN EROSI
        /// </summary>
        private Bitmap ErodeImage(Bitmap src, int kernelSize)
        {
            //membuat variabel dimensi gambar untuk waktu yang sebaik-baiknya
            int width = src.Width;
            int height = src.Height;
            //bit kunci untuk sistem memori supaya pemrosesan cepat
            Rectangle canvas = new Rectangle(0, 0, width, height);
            BitmapData srcData = src.LockBits(canvas, ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
            //membuat byte array yang akan memegang semua data piksel, satu untuk pemrosesan, satu untuk output
            int stride = srcData.Stride;
            int bytes = srcData.Stride * srcData.Height;
            byte[] pixelBuffer = new byte[bytes];
            byte[] resultBuffer = new byte[bytes];
            Marshal.Copy(srcData.Scan0, pixelBuffer, 0, bytes);//ini
            src.UnlockBits(srcData);
            //int kernelSize = 3;
            int kernelOffset = (kernelSize - 1) / 2;
            int calcOffset = 0;
            int byteOffset = 0;
            byte blue = 255;
            byte green = 255;
            byte red = 255;
            for (int y = kernelOffset; y < height - kernelOffset; y++)
            {
                for (int x = kernelOffset; x < width - kernelOffset; x++)
                {
                    //byte value = 255;
                    byteOffset = y * srcData.Stride + x * 4;
                    //menentukan rgb
                    blue = 255;
                    green = 255;
                    red = 255;
                    //erosi
                    for (int ykernel = -kernelOffset; ykernel <= kernelOffset; ykernel++)
                    {
                        for (int xkernel = -kernelOffset; xkernel <= kernelOffset; xkernel++)
                        {
                            calcOffset = byteOffset + (xkernel * 4) + (ykernel * stride);
                            if (pixelBuffer[calcOffset] < blue)
                            {
                                blue = pixelBuffer[calcOffset];
                            }
                            if (pixelBuffer[calcOffset + 1] < green)
                            {
                                green = pixelBuffer[calcOffset + 1];
                            }
                            if (pixelBuffer[calcOffset + 2] < red)
                            {
                                red = pixelBuffer[calcOffset + 2];
                            }
                        }
                    }
                    resultBuffer[byteOffset] = blue;
                    resultBuffer[byteOffset + 1] = green;
                    resultBuffer[byteOffset + 2] = red;
                    resultBuffer[byteOffset + 3] = 255;
                }
            }
            Bitmap result = new Bitmap(width, height);
            BitmapData resultData = result.LockBits(canvas, ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);
            Marshal.Copy(resultBuffer, 0, resultData.Scan0, bytes);
            result.UnlockBits(resultData);
            return result;
        }
        /// <summary>
        /// #########
        /// </summary>
        System.Drawing.Image img;
        //string[] exten = { ".bmp" };
        public LEARNING()
        {
            InitializeComponent();
        }
        /// <summary>
        /// #########
        /// </summary>
        /// 
        private void Form2_Load(object sender, EventArgs e)
        {
            panggildatabase();
           
        }
        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                if (btnStartCamera.Text == "START")
                {
                    _vCaptureDevice = new VideoCaptureDevice(_fInfoCollection[cboCamera.SelectedIndex].MonikerString);
                    _vCaptureDevice.NewFrame += new NewFrameEventHandler(get_Frame);
                    //Start the Capture Device
                    _vCaptureDevice.Start();
                    btnStartCamera.Text = "STOP";
                }
                else
                {
                    btnStartCamera.Text = "START";
                    _vCaptureDevice.Stop();
                }
            }
            catch (Exception)
            {
                MessageBox.Show("No video capture device found");
            }
        }
        private void get_Frame(object sender, NewFrameEventArgs eventArgs)
        {
            //Insert image into Picuture Box
            Bitmap _BsourceFrame = (Bitmap)eventArgs.Frame.Clone();
            picVideo.Image = _BsourceFrame;
        }
       
        
        private void button3_Click(object sender, EventArgs e)
        {
            operasi(); 
        }
        /// <summary>
        /// CLASS MENENTUKAN LOAD CAMERA YANG TERDETEKSI
        /// </summary>
        
        /// <summary>
        /// TIMER UNTUK KMEANS
        /// </summary>
        private void timer1_Tick(object sender, EventArgs e)
        {
            if (!_kMeans.Converged)
            {
                _kMeans.Iterate();//ITERATE BEKERJA
                pictureBox3.Image = _kMeans.ProcessedImage;
                pictureBox3.Refresh();
                _count++;
            }
            else
            {
                _count = 0;
                timer1.Enabled = false;
                timer1.Stop();
                pictureBox3.Image.Save(@"E:\Gambar\Gambar_kmeans" + "\\" + jumlahdt.ToString() + ".bmp");
                warna();

            }
        }
        /// <summary>
        /// CLASS MENENTUKAN OBJEK TERBESAR
        /// </summary>
        void extractbiggestkmeans()
        {
            Bitmap gambar = (Bitmap)pictureBox5.Image.Clone();
            ExtractBiggestBlob filter = new ExtractBiggestBlob();
            pictureBox3.Image = filter.Apply(gambar);
        }
        /// <summary>
        /// CLASS MENENTUKAN FILE KMEANS
        /// </summary>
        #region "CONTROL EVENTS"
        void proseskmeans()
        {
            extractbiggestkmeans();
            _kMeans = new KMeans((Bitmap)pictureBox3.Image, (2)/*variabel untuk menambahkan nomor kluster*/, tugasakhir.deteksi.Colour.Types.RGB);
            timer1.Enabled = true;
            timer1.Start();
        }
        #endregion
        KMeans _kMeans;
        int _count = 0;
        
        private void button2_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "BMP Images(*.bmp)|*.bmp|Jpeg Images(*.jpg)|*.jpg";
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                label2.Text = "Lokasi File :" + ofd.FileName;
                img = System.Drawing.Image.FromFile(ofd.FileName);
                picVideo.Image = img;
            }
           
        }
        public void warna()
        {
            imagestat = (Bitmap)pictureBox3.Image.Clone();
            ColorImageStatisticsDescription statDesc = new ColorImageStatisticsDescription(imagestat);
            label3.Text = "Red     = " + statDesc.RedMax.ToString() + 
                        "\nGreen  = " + statDesc.GreenMax.ToString() + 
                        "\nBlue     = " + statDesc.BlueMax.ToString();
        
        }

        private void button5_Click(object sender, EventArgs e)
        {
            updatedata();
        }
        /// <summary>
        /// CLASS MENGUBAH UKURAN PIKSEL
        /// </summary>
        //fungsi untuk mengubah ukuran piksel citra 
        System.Drawing.Image Rsize(System.Drawing.Image image, int w, int h)
        {
            Bitmap bmp = new Bitmap(w, h, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            Graphics graphic = Graphics.FromImage(bmp);
            graphic.DrawImage(image, 0, 0, w, h);
            graphic.Dispose();
            return bmp;
        }
        private void resizeimage()
        {
            img = (Bitmap)picVideo.Image.Clone();
            img = Rsize(img, 320, 240);
            jumlahdata();
            jumlahdt = jumlahdt++;
            pictureBox2.Image = img;
            pictureBox2.Image.Save(@"E:\Gambar\Gambar_Resize" + "\\" + jumlahdt.ToString() + ".bmp");
            
            label19.Text = "Nama File Terbaru : ";
            label14.Text = jumlahdt.ToString() + ".bmp";
            label2.Text = "Lokasi File : E:/Gambar/Gambar_Resize/" + label14.Text + ".bmp";
            tambahdata();
        }
        /// <summary>
        /// CLASS MELAKUKAN EROSI DAN DILASI
        /// </summary>
        void erosidandilasi()
        {
            //proses erosi dari b
            Bitmap src = (Bitmap)pictureBox10.Image.Clone();
            src = ErodeImage(src, 6);
            pictureBox5.Image = src;
            //proses dilasi dari src
            Bitmap srcImg = (Bitmap)pictureBox5.Image.Clone();
            srcImg = Morph(srcImg, 6);
            pictureBox5.Image = srcImg;
            Bitmap erdil = (Bitmap)pictureBox5.Image.Clone();
            Bitmap erdilpro = ConvertTo24bpp(erdil);
            pictureBox5.Image = erdilpro;
            pictureBox5.Image.Save(@"E:\Gambar\Gambar_dilasidanerosi" + "\\" + jumlahdt.ToString() + ".bmp");
        }
        /// <summary>
        /// CLASS MELAKUKAN PROSES EUCLIDEAN COLOR FILTERING
        /// </summary>
        void prosesECF()
        {
                image = (Bitmap)pictureBox2.Image.Clone();
                EuclideanColorFiltering filter = new EuclideanColorFiltering();
                filter.ApplyInPlace(image);
                pictureBox10.Image = image;
                pictureBox10.Image.Save(@"E:\Gambar\Gambar_euclide" + "\\" + jumlahdt.ToString() + ".bmp");    
        }
        /// <summary>
        /// CLASS MENENTUKAN OBJEK TERBESAR DARI OBJEK ASLI
        /// </summary>
        void extractbiggest()
        {
            Bitmap gambar = (Bitmap)pictureBox10.Image.Clone();
            ExtractBiggestBlob filter = new ExtractBiggestBlob();
            Bitmap img = new Bitmap(gambar);
            Bitmap imageext = filter.Apply(img);
            pictureBox9.BackColor = Color.Empty;
            pictureBox9.Image = imageext;
            pictureBox9.Image.Save(@"E:\Gambar\Gambar_ExctractBiggest" + "\\" + jumlahdt.ToString() + ".bmp");
        }
        /// <summary>
        /// CLASS MENENTUKAN THERSHOLD
        /// </summary>
      
        /// <summary>
        /// CLASS OPERASI
        /// </summary>
        void operasi()
        {
            resizeimage();
            prosesECF();
            extractbiggest();
            erosidandilasi();
            proseskmeans();
            
        }
        public static Bitmap ConvertTo24bpp(System.Drawing.Image img)
        {
            var bmp = new Bitmap(img.Width, img.Height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            using (var gr = Graphics.FromImage(bmp))
                gr.DrawImage(img, new Rectangle(0, 0, img.Width, img.Height));
            return bmp;
        }
        
        private void button3_Click_1(object sender, EventArgs e)
        {
            operasi();
        }

        private void LEARNING_FormClosed(object sender, FormClosedEventArgs e)
        {
            _vCaptureDevice.Stop();
        }

        private void identifikasiToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form1 frm = new Form1();
            frm.Show();
            frm.StartPosition = FormStartPosition.CenterScreen;
            frm = null;
            this.Hide();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            try
            {
                _fInfoCollection = new FilterInfoCollection(FilterCategory.VideoInputDevice);
                foreach (FilterInfo _fcategory in _fInfoCollection)
                {
                    cboCamera.Items.Add(_fcategory.Name);
                }
                cboCamera.SelectedIndex = 0;
            }
            catch (Exception _exception)
            {
                MessageBox.Show("Sorry something went wroung" + _exception);
            }
        }

        private void hOMEToolStripMenuItem_Click(object sender, EventArgs e)
        {
            HOME frm = new HOME();
            frm.Show();
            frm.StartPosition = FormStartPosition.CenterScreen;
            frm = null;
            this.Hide();
        }

        private void pROGRAMERToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form4 frm = new Form4();
            frm.Show();
            frm.StartPosition = FormStartPosition.CenterScreen;
            frm = null;
            this.Hide();
        }

        private void kELUARToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void label18_Click(object sender, EventArgs e)
        {

        }

    }
}
