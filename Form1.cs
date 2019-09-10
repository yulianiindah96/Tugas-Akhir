using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using AForge;
using AForge.Video;
using AForge.Video.DirectShow;
using AForge.Imaging.Filters;
using AForge.Imaging;
using AForge.Math.Geometry;
using System.Data.OleDb;
using tugasakhir;
using tugasakhir.deteksi;
using System.Threading.Tasks;
using System.IO;
using System.Runtime.InteropServices;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using AForge.Imaging.Formats;

namespace tugasakhir
{
    public partial class Form1 : Form
    {
        public static DataSet data = new DataSet();
        
        private Bitmap imagestat = null;
        
        FilterInfoCollection _fInfoCollection;
        VideoCaptureDevice _vCaptureDevice;
        private BlobCounter blobCounter = new BlobCounter(); //membuat blob baru
        private Blob[] blobs;
        Dictionary<int, List<IntPoint>> leftEdges = new Dictionary<int, List<IntPoint>>();
        Dictionary<int, List<IntPoint>> rightEdges = new Dictionary<int, List<IntPoint>>();
        Dictionary<int, List<IntPoint>> topEdges = new Dictionary<int, List<IntPoint>>();
        Dictionary<int, List<IntPoint>> bottomEdges = new Dictionary<int, List<IntPoint>>();
        Dictionary<int, List<IntPoint>> hulls = new Dictionary<int, List<IntPoint>>();
        Dictionary<int, List<IntPoint>> quadrilaterals = new Dictionary<int, List<IntPoint>>();
        int count1 = 1;
        int count2 = 1;
        int count3 = 1;
        int count4 = 1;
        int count5 = 1;
        int count6 = 1;
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
                dt.Fill(data, "Utama");
                dataGridView1.DataSource = data;
                dataGridView1.DataMember = "Utama";
                koneksi.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        public Form1()
        {
            InitializeComponent();
        }

        /// <summary>
        /// CLASS MENENTUKAN NILAI RGB PADA OBJEK DENGAN BLACK
        /// </summary>
        public class ColorImageStatisticsDescription
        {
            public ImageStatistics statRGB;
            [Category("0: General")]
            public int PixelsCount
            {
                get { return statRGB.PixelsCount; }
            }
            [Category("0: General")]
            public int PixelsWithoutBlack
            {
                get { return statRGB.PixelsCountWithoutBlack; }
            }
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
            [Category("1: Red Dengan black")]
            public int RedMedian
            {
                get { return statRGB.Red.Median; }
            }
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
            [Category("2: Green Dengan black")]
            public int GreenMedian
            {
                get { return statRGB.Green.Median; }
            }
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
            [Category("3: Blue Dengan black")]
            public int BlueMedian
            {
                get { return statRGB.Blue.Median; }
            }

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
        
        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                _fInfoCollection = new FilterInfoCollection(FilterCategory.VideoInputDevice);
                foreach (FilterInfo _fcategory in _fInfoCollection)
                {
                    comboBox1.Items.Add(_fcategory.Name);
                }
                    comboBox1.SelectedIndex = 0;
            }
            catch (Exception _exception)
            {
                MessageBox.Show("Sorry something went wroung" + _exception);
            }
        }
        
        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                if (button2.Text == "START")
                {
                    _vCaptureDevice = new VideoCaptureDevice(_fInfoCollection[comboBox1.SelectedIndex].MonikerString);
                    _vCaptureDevice.NewFrame += new NewFrameEventHandler(get_Frame);
                    //Start the Capture Device
                    _vCaptureDevice.Start();
                    button2.Text = "STOP";
                }
                else
                {
                    button2.Text = "START";
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
            pictureBox2.Image = (Bitmap)eventArgs.Frame.Clone();
            Bitmap _BsourceFrame = (Bitmap)eventArgs.Frame.Clone();
            pictureBox1.Image = BlobDetection(_BsourceFrame);
        }

        //Blob Detection
        private Bitmap BlobDetection(Bitmap _bitmapSourceImage)
        {
            //qimageterblob = null;
            //imageterblob = new List<Bitmap>();
            leftEdges.Clear();
            rightEdges.Clear();
            topEdges.Clear();
            bottomEdges.Clear();
            hulls.Clear();
            quadrilaterals.Clear();
            
            
            GrayscaleBT709 _grayscale = new GrayscaleBT709();
            Bitmap _bitmapGreyImage = _grayscale.Apply(_bitmapSourceImage);
            //create a edge detector instance

            DifferenceEdgeDetector _differeceEdgeDetector = new DifferenceEdgeDetector();
            Bitmap _bitmapEdgeImage = _differeceEdgeDetector.Apply(_bitmapGreyImage);

            Threshold _threshold = new Threshold(40);
            Bitmap _bitmapBinaryImage = _threshold.Apply(_bitmapEdgeImage);
            blobCounter.MinWidth = 70;
            blobCounter.MinHeight = 70;

            blobCounter.FilterBlobs = true;
            blobCounter.ObjectsOrder = ObjectsOrder.XY;
            blobCounter.ProcessImage(_bitmapBinaryImage);
            blobs = blobCounter.GetObjectsInformation();
            //Blob[] blobq = blobCounter.GetObjects(_bitmapSourceImage, false);
            int Count = blobs.Length;
          
            GrahamConvexHull grahamScan = new GrahamConvexHull();

            foreach (Blob blob in blobs)
            {
                List<IntPoint> leftEdge = new List<IntPoint>();
                List<IntPoint> rightEdge = new List<IntPoint>();
                List<IntPoint> topEdge = new List<IntPoint>();
                List<IntPoint> bottomEdge = new List<IntPoint>();
                // collect edge points
                blobCounter.GetBlobsLeftAndRightEdges(blob, out leftEdge, out rightEdge);
                blobCounter.GetBlobsTopAndBottomEdges(blob, out topEdge, out bottomEdge);
                leftEdges.Add(blob.ID, leftEdge);
                rightEdges.Add(blob.ID, rightEdge);
                topEdges.Add(blob.ID, topEdge);
                bottomEdges.Add(blob.ID, bottomEdge);
                // find convex hull
                List<IntPoint> edgePoints = new List<IntPoint>();
                edgePoints.AddRange(leftEdge);
                edgePoints.AddRange(rightEdge);
                List<IntPoint> hull = grahamScan.FindHull(edgePoints);
                hulls.Add(blob.ID, hull);
                List<IntPoint> quadrilateral = null;
                // find quadrilateral
                if (hull.Count < 4)
                {
                    quadrilateral = new List<IntPoint>(hull);
                }
                else
                {
                    quadrilateral = PointsCloud.FindQuadrilateralCorners(hull);
                }
                    quadrilaterals.Add(blob.ID, quadrilateral);
                    // Paint the control
                Graphics g = Graphics.FromImage(_bitmapSourceImage);
                Pen yellowPen = new Pen(Color.Red, 3);
                g.DrawPolygon(yellowPen, ToPointsArray(hulls[blob.ID]));
                System.Drawing.Font _font = new System.Drawing.Font("Segoe UI", 16);
                System.Drawing.SolidBrush _brush = new System.Drawing.SolidBrush(System.Drawing.Color.Blue);
                System.Drawing.Point[] _coordinates = ToPointsArray(edgePoints);
                System.Drawing.Point[] _poin = ToPointsArray(hulls[blob.ID]);
                // AForge.Point center;
                if (blobs.Length == 4)
                {
                    int x1 = blobs[0].Rectangle.Location.X;
                    int y1 = blobs[0].Rectangle.Location.Y;
                    int x2 = blobs[1].Rectangle.Location.X;
                    int y2 = blobs[1].Rectangle.Location.Y;
                    int x3 = blobs[2].Rectangle.Location.X;
                    int y3 = blobs[2].Rectangle.Location.Y;
                    int x4 = blobs[3].Rectangle.Location.X;
                    int y4 = blobs[3].Rectangle.Location.Y;
                    g.DrawString(label2.Text, _font, _brush, x1, y1);
                    g.DrawString(label3.Text, _font, _brush, x2, y2);
                    g.DrawString(label5.Text, _font, _brush, x3, y3);
                    g.DrawString(label7.Text, _font, _brush, x4, y4);
                    g.DrawString("Jumlah Buah : " + (Count.ToString()), _font, _brush, 0, 0);
                }
                else if (blobs.Length == 3)
                {
                    int x1 = blobs[0].Rectangle.Location.X;
                    int y1 = blobs[0].Rectangle.Location.Y;
                    int x2 = blobs[1].Rectangle.Location.X;
                    int y2 = blobs[1].Rectangle.Location.Y;
                    int x3 = blobs[2].Rectangle.Location.X;
                    int y3 = blobs[2].Rectangle.Location.Y;
                    g.DrawString(label2.Text, _font, _brush, x1, y1);
                    g.DrawString(label3.Text, _font, _brush, x2, y2);
                    g.DrawString(label5.Text, _font, _brush, x3, y3);
                    g.DrawString("Jumlah Buah : " + (Count.ToString()), _font, _brush, 0, 0);
                }
                else if (blobs.Length == 2)
                {
                    int x1 = blobs[0].Rectangle.Location.X;
                    int y1 = blobs[0].Rectangle.Location.Y;
                    int x2 = blobs[1].Rectangle.Location.X;
                    int y2 = blobs[1].Rectangle.Location.Y;
                    g.DrawString(label2.Text, _font, _brush, x1, y1);
                    g.DrawString(label3.Text, _font, _brush, x2, y2);
                    g.DrawString("Jumlah Buah : " + (Count.ToString()), _font, _brush, 0, 0);
                }
                else if (blobs.Length == 1)
                {
                    int x1 = blobs[0].Rectangle.Location.X;
                    int y1 = blobs[0].Rectangle.Location.Y;
                    g.DrawString(label2.Text, _font, _brush, x1, y1);
                    g.DrawString("Jumlah Buah : " + (Count.ToString()), _font, _brush, 0, 0);
                }
                else if (blobs.Length >= 5)
                {
                    int _x = _coordinates[0].X;
                    int _y = _coordinates[0].Y;
                    g.DrawString("Buah Kopi", _font, _brush, _x, _y);
                    g.DrawString("Jumlah Buah : " + (Count.ToString()), _font, _brush, 0, 0);
                }
                }     
            return _bitmapSourceImage;
        }

        private System.Drawing.Point[] ToPointsArray(List<IntPoint> points)
        {
            System.Drawing.Point[] array = new System.Drawing.Point[points.Count];
            
            for (int i = 0, n = points.Count; i < n; i++)
            {
                array[i] = new System.Drawing.Point(points[i].X, points[i].Y);
            }
            
            return array;
        }
        
        private void Form1_Load(object sender, EventArgs e)
        {
            panggildatabase();            
        }
        
        public void prosesdeteksi()
        {
            resizeimage();
            operasikanblob();
            proseskmeans();            
        }
        
        private void button3_Click(object sender, EventArgs e)
        {
            prosesdeteksi();
            
            try
            {
                
                if (label2.Text == "Hijau(Masih muda)")
                {
                    textBox1.Visible = true;
                    //textBox1.Text = (System.Math.Abs(Int32.Parse(textBox1.Text)) + 1).ToString();
                    textBox1.Text = (count1++).ToString();
                }
                else if (label2.Text == "Hijau Kekuningan(Masih muda)")
                {
                    textBox1.Visible = true;
                    //textBox2.Text = (System.Math.Abs(Int32.Parse(textBox2.Text)) + 1).ToString();
                    textBox1.Text = (count1++).ToString();
                }
                else if (label2.Text == "Kuning Kemerahan(Mulai Matang)")
                {
                    textBox2.Visible = true;
                    //textBox4.Text = (System.Math.Abs(Int32.Parse(textBox4.Text)) + 1).ToString();
                    textBox2.Text = (count2++).ToString();
                }
                else if (label2.Text == "Merah Penuh(Matang Sempurna)")
                {
                    textBox4.Visible = true;
                    //textBox3.Text = (System.Math.Abs(Int32.Parse(textBox3.Text)) + 1).ToString();
                    textBox4.Text = (count4++).ToString();
                }
                else if (label2.Text == "Hijau(Masih muda)")
                {
                    textBox1.Visible = true;
                    //textBox3.Text = (System.Math.Abs(Int32.Parse(textBox3.Text)) + 1).ToString();
                    textBox1.Text = (count1++).ToString();
                }
                else if (label2.Text == "Merah Tua(Kelewat Matang)")
                {
                    textBox3.Visible = true;
                    //textBox6.Text = (System.Math.Abs(Int32.Parse(textBox6.Text)) + 1).ToString();
                    textBox3.Text = (count3++).ToString();
                }
                else if (label2.Text == "Hitam(Busuk)")
                {
                    textBox5.Visible = true;
                    //textBox5.Text = (System.Math.Abs(Int32.Parse(textBox5.Text)) + 1).ToString();
                    textBox5.Text = (count5++).ToString();
                }


                if (label3.Text == "Hijau(Masih muda)")
                {
                    textBox1.Visible = true;
                    //textBox1.Text = (System.Math.Abs(Int32.Parse(textBox1.Text)) + 1).ToString();
                    textBox1.Text = (count1++).ToString();
                }
                else if (label3.Text == "Hijau Kekuningan(Masih muda)")
                {
                    textBox1.Visible = true;
                    //textBox2.Text = (System.Math.Abs(Int32.Parse(textBox2.Text)) + 1).ToString();
                    textBox1.Text = (count1++).ToString();
                }
                else if (label3.Text == "Kuning Kemerahan(Mulai Matang)")
                {
                    textBox2.Visible = true;
                    //textBox4.Text = (System.Math.Abs(Int32.Parse(textBox4.Text)) + 1).ToString();
                    textBox2.Text = (count2++).ToString();
                }
                else if (label3.Text == "Merah Penuh(Matang Sempurna)")
                {
                    textBox4.Visible = true;
                    //textBox3.Text = (System.Math.Abs(Int32.Parse(textBox3.Text)) + 1).ToString();
                    textBox4.Text = (count4++).ToString();
                }
                else if (label3.Text == "Hijau(Masih muda)")
                {
                    textBox1.Visible = true;
                    //textBox3.Text = (System.Math.Abs(Int32.Parse(textBox3.Text)) + 1).ToString();
                    textBox1.Text = (count1++).ToString();
                }
                else if (label3.Text == "Merah Tua(Kelewat Matang)")
                {
                    textBox3.Visible = true;
                    //textBox6.Text = (System.Math.Abs(Int32.Parse(textBox6.Text)) + 1).ToString();
                    textBox3.Text = (count3++).ToString();
                }
                else if (label3.Text == "Hitam(Busuk)")
                {
                    textBox5.Visible = true;
                    //textBox5.Text = (System.Math.Abs(Int32.Parse(textBox5.Text)) + 1).ToString();
                    textBox5.Text = (count5++).ToString();
                }

                if (label5.Text == "Hijau(Masih muda)")
                {
                    textBox1.Visible = true;
                    //textBox1.Text = (System.Math.Abs(Int32.Parse(textBox1.Text)) + 1).ToString();
                    textBox1.Text = (count1++).ToString();
                }
                else if (label5.Text == "Hijau Kekuningan(Masih muda)")
                {
                    textBox1.Visible = true;
                    //textBox2.Text = (System.Math.Abs(Int32.Parse(textBox2.Text)) + 1).ToString();
                    textBox1.Text = (count1++).ToString();
                }
                else if (label5.Text == "Kuning Kemerahan(Mulai Matang)")
                {
                    textBox2.Visible = true;
                    //textBox4.Text = (System.Math.Abs(Int32.Parse(textBox4.Text)) + 1).ToString();
                    textBox2.Text = (count2++).ToString();
                }
                else if (label5.Text == "Merah Penuh(Matang Sempurna)")
                {
                    textBox4.Visible = true;
                    //textBox3.Text = (System.Math.Abs(Int32.Parse(textBox3.Text)) + 1).ToString();
                    textBox4.Text = (count4++).ToString();
                }
                else if (label5.Text == "Hijau(Masih muda)")
                {
                    textBox1.Visible = true;
                    //textBox3.Text = (System.Math.Abs(Int32.Parse(textBox3.Text)) + 1).ToString();
                    textBox1.Text = (count1++).ToString();
                }
                else if (label5.Text == "Merah Tua(Kelewat Matang)")
                {
                    textBox3.Visible = true;
                    //textBox6.Text = (System.Math.Abs(Int32.Parse(textBox6.Text)) + 1).ToString();
                    textBox3.Text = (count3++).ToString();
                }
                else if (label5.Text == "Hitam(Busuk)")
                {
                    textBox5.Visible = true;
                    //textBox5.Text = (System.Math.Abs(Int32.Parse(textBox5.Text)) + 1).ToString();
                    textBox5.Text = (count5++).ToString();
                }

                if (label7.Text == "Hijau(Masih muda)")
                {
                    textBox1.Visible = true;
                    //textBox1.Text = (System.Math.Abs(Int32.Parse(textBox1.Text)) + 1).ToString();
                    textBox1.Text = (count1++).ToString();
                }
                else if (label7.Text == "Hijau Kekuningan(Masih muda)")
                {
                    textBox1.Visible = true;
                    //textBox2.Text = (System.Math.Abs(Int32.Parse(textBox2.Text)) + 1).ToString();
                    textBox1.Text = (count1++).ToString();
                }
                else if (label7.Text == "Kuning Kemerahan(Mulai Matang)")
                {
                    textBox2.Visible = true;
                    //textBox4.Text = (System.Math.Abs(Int32.Parse(textBox4.Text)) + 1).ToString();
                    textBox2.Text = (count2++).ToString();
                }
                else if (label7.Text == "Merah Penuh(Matang Sempurna)")
                {
                    textBox4.Visible = true;
                    //textBox3.Text = (System.Math.Abs(Int32.Parse(textBox3.Text)) + 1).ToString();
                    textBox4.Text = (count4++).ToString();
                }
                else if (label7.Text == "Hijau(Masih muda)")
                {
                    textBox1.Visible = true;
                    //textBox3.Text = (System.Math.Abs(Int32.Parse(textBox3.Text)) + 1).ToString();
                    textBox1.Text = (count1++).ToString();
                }
                else if (label7.Text == "Merah Tua(Kelewat Matang)")
                {
                    textBox3.Visible = true;
                    //textBox6.Text = (System.Math.Abs(Int32.Parse(textBox6.Text)) + 1).ToString();
                    textBox3.Text = (count3++).ToString();
                }
                else if (label7.Text == "Hitam(Busuk)")
                {
                    textBox5.Visible = true;
                    //textBox5.Text = (System.Math.Abs(Int32.Parse(textBox5.Text)) + 1).ToString();
                    textBox5.Text = (count5++).ToString();
                }
                textBox6.Text = (Convert.ToDouble(textBox1.Text) + Convert.ToDouble(textBox2.Text) + Convert.ToDouble(textBox3.Text) + Convert.ToDouble(textBox4.Text) + Convert.ToDouble(textBox5.Text)).ToString();
            }
            catch (Exception)
            { }
        }
       
        /// <summary>
        /// CLASS MENGUBAH UKURAN PIKSEL
        /// </summary>
        //fungsi untuk mengubah ukuran piksel citra 
        Bitmap Rsize(Bitmap image, int w, int h)
        {
            Bitmap bmp = new Bitmap(w, h, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            Graphics graphic = Graphics.FromImage(bmp);
            graphic.DrawImage(image, 0, 0, w, h);
            graphic.Dispose();
            return bmp;
        }
        
        public void resizeimage()
        {
            Bitmap img = (Bitmap)pictureBox2.Image.Clone();
            img = Rsize(img, 320, 240);
            pictureBox4.Image = img;
            Bitmap imageECF = (Bitmap)pictureBox4.Image.Clone();
            EuclideanColorFiltering filter = new EuclideanColorFiltering();
            filter.ApplyInPlace(imageECF);
            pictureBox4.Image = imageECF;
            Bitmap src = (Bitmap)pictureBox4.Image.Clone();
            src = ErodeImage(src, 6);
            pictureBox4.Image = src;
            Bitmap srcImg = (Bitmap)pictureBox4.Image.Clone();
            srcImg = Morph(srcImg, 6);
            pictureBox4.Image = srcImg;
            Bitmap erdil = (Bitmap)pictureBox4.Image.Clone();
            erdil = ConvertTo24bpp(erdil);
            pictureBox4.Image = erdil;
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
        
        public static Bitmap ConvertTo24bpp(System.Drawing.Image img)
        {
            var bmp = new Bitmap(img.Width, img.Height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            using (var gr = Graphics.FromImage(bmp))
                gr.DrawImage(img, new Rectangle(0, 0, img.Width, img.Height));
            return bmp;
        }
       
        
        public void operasikanblob()
        {
           
            pictureBox5.Image = null; pictureBox5.BackColor = Color.Empty;
            pictureBox6.Image = null; pictureBox6.BackColor = Color.Empty;
            pictureBox3.Image = null; pictureBox3.BackColor = Color.Empty;
            pictureBox7.Image = null; pictureBox7.BackColor = Color.Empty;
            Bitmap img1 = (Bitmap)pictureBox4.Image.Clone();
            List<Bitmap> foundimages;
            bool result = ApplyBlobExtractor(img1, 1, out foundimages);
            //string resultnya;
            if (foundimages.Count == 4)
            {
                pictureBox5.Image = foundimages[0];
                pictureBox6.Image = foundimages[1];
                pictureBox3.Image = foundimages[2];
                pictureBox7.Image = foundimages[3];
                label19.Text = "Euclidean Distance";
                
                label22.Text = "Euclidean Distance";
                
                label24.Text = "Euclidean Distance";
               
                label26.Text = "Euclidean Distance";
                
            }
            else if (foundimages.Count == 3)
            {
                pictureBox5.Image = foundimages[0];
                pictureBox6.Image = foundimages[1];
                pictureBox3.Image = foundimages[2];
                pictureBox7.BackColor = Color.Empty;
                label19.Text = "Euclidean Distance";
                
                label22.Text = "Euclidean Distance";
                
                label24.Text = "Euclidean Distance";
                
                label26.Text = "";
                
            }
            else if (foundimages.Count == 2)
            {
                pictureBox5.Image = foundimages[0];
                pictureBox6.Image = foundimages[1];
                pictureBox3.BackColor = Color.Empty;
                pictureBox7.BackColor = Color.Empty;
                label19.Text = "Euclidean Distance";
                
                label22.Text = "Euclidean Distance";
               
                label24.Text = "";
               
                label26.Text = "";
               
            }
            else if (foundimages.Count == 1)
            {
                pictureBox5.Image = foundimages[0];
                pictureBox6.BackColor = Color.Empty;
                pictureBox3.BackColor = Color.Empty;
                pictureBox7.BackColor = Color.Empty;
                label19.Text = "Euclidean Distance";
                
                label22.Text = "";
               
                label24.Text = "";
                
                label26.Text = "";
                
            }
            else
            {
                pictureBox5.BackColor = Color.Empty;
                pictureBox6.BackColor = Color.Empty;
                pictureBox3.BackColor = Color.Empty;
                pictureBox7.BackColor = Color.Empty;
                label19.Text = "";
                
                label22.Text = "";
             
                label24.Text = "";
               
                label26.Text = "";
               
            }
        }
        
        public static bool ApplyBlobExtractor(Bitmap image, int jumlah_blob, out List<Bitmap> imageterblob)
        {
            imageterblob = null;
            imageterblob = new List<Bitmap>();
            BlobCounter blobCounter = new BlobCounter();
            //sort 
            blobCounter.MinWidth = 70;
            blobCounter.MinHeight = 70;

            blobCounter.ObjectsOrder = ObjectsOrder.XY;
            blobCounter.ProcessImage(image);
            Blob[] blobq = blobCounter.GetObjects(image, false);
            //Blob[] blobs = blobCounter.GetObjectsInformation();
            UnmanagedImage currentImage;
            foreach (Blob blob in blobq)
            {
                currentImage = blob.Image;
                imageterblob.Add(currentImage.ToManagedImage());
            }
            return imageterblob.Count == jumlah_blob;
        }
        
        void proseskmeans()
        {
            if (pictureBox5.Image != null)
            {
                _kMeans1 = new KMeans((Bitmap)pictureBox5.Image, (2)/*variabel untuk menambahkan nomor kluster*/, tugasakhir.deteksi.Colour.Types.RGB);
                timer1.Enabled = true;
                timer1.Start();
            }
            else
            {
                _kMeans1 = null;
                label1.Text = "";
                label2.Text = "";
            }
            if (pictureBox6.Image != null)
            {
                _kMeans2 = new KMeans((Bitmap)pictureBox6.Image, (2), tugasakhir.deteksi.Colour.Types.RGB);
                timer2.Enabled = true;
                timer2.Start();
            }
            else
            {
                _kMeans2 = null;
                label4.Text = "";
                label3.Text = "";
            }
            if (pictureBox3.Image != null)
            {
                _kMeans3 = new KMeans((Bitmap)pictureBox3.Image, (2), tugasakhir.deteksi.Colour.Types.RGB);
                timer3.Enabled = true;
                timer3.Start();
            }
            else
            {
                _kMeans3 = null;
                label6.Text = "";
                label5.Text = "";
            }
            if (pictureBox7.Image != null)
            {
                _kMeans4 = new KMeans((Bitmap)pictureBox7.Image, (2), tugasakhir.deteksi.Colour.Types.RGB);
                timer4.Enabled = true;
                timer4.Start();
            }
            else
            {
                _kMeans4 = null;
                label8.Text = "";
                label7.Text = "";
            }
        }
        KMeans _kMeans1;
        KMeans _kMeans2;
        KMeans _kMeans3;
        KMeans _kMeans4;
        int _count1 = 0;
        int _count2 = 0;
        int _count3 = 0;
        int _count4 = 0;
        
        private void timer1_Tick(object sender, EventArgs e)
        {
            if (_kMeans1 != null)
            {
                if (!_kMeans1.Converged)
                {
                    _kMeans1.Iterate();//ITERATE BEKERJA
                    pictureBox5.Image = _kMeans1.ProcessedImage;
                    pictureBox5.Refresh();
                    _count1++;
                }
                else
                {
                    _count1 = 0;
                    timer1.Enabled = false;
                    timer1.Stop();
                    
                    try
                    {
                        if (pictureBox5.Image != null)
                        {
                            imagestat = (Bitmap)pictureBox5.Image.Clone();
                            ColorImageStatisticsDescription statDesc = new ColorImageStatisticsDescription(imagestat);
                            foreach (DataGridViewRow row in dataGridView1.Rows)
                            {
                                ///menghitung pendekatan
                                dataGridView1.Rows[row.Index].Cells[15].Value = (
                                    System.Math.Pow((Double.Parse(dataGridView1.Rows[row.Index].Cells[3].Value.ToString())
                                            - Double.Parse(statDesc.RedMean.ToString())),2) + System.Math.Pow((Double.Parse(dataGridView1.Rows[row.Index].Cells[4].Value.ToString())
                                            - Double.Parse(statDesc.RedStdDev.ToString())),2) + System.Math.Pow((Double.Parse(dataGridView1.Rows[row.Index].Cells[5].Value.ToString())
                                            - Double.Parse(statDesc.RedMin.ToString())),2) + System.Math.Pow((Double.Parse(dataGridView1.Rows[row.Index].Cells[6].Value.ToString())
                                            - Double.Parse(statDesc.RedMax.ToString())),2) + System.Math.Pow((Double.Parse(dataGridView1.Rows[row.Index].Cells[7].Value.ToString())
                                            - Double.Parse(statDesc.GreenMean.ToString())),2) + System.Math.Pow((Double.Parse(dataGridView1.Rows[row.Index].Cells[8].Value.ToString())
                                            - Double.Parse(statDesc.GreenStdDev.ToString())),2) + System.Math.Pow((Double.Parse(dataGridView1.Rows[row.Index].Cells[9].Value.ToString())
                                            - Double.Parse(statDesc.GreenMin.ToString())),2) + System.Math.Pow((Double.Parse(dataGridView1.Rows[row.Index].Cells[10].Value.ToString())
                                            - Double.Parse(statDesc.GreenMax.ToString())),2) + System.Math.Pow((Double.Parse(dataGridView1.Rows[row.Index].Cells[11].Value.ToString())
                                            - Double.Parse(statDesc.BlueMean.ToString())),2) + System.Math.Pow((Double.Parse(dataGridView1.Rows[row.Index].Cells[12].Value.ToString())
                                            - Double.Parse(statDesc.BlueStdDev.ToString())),2) + System.Math.Pow((Double.Parse(dataGridView1.Rows[row.Index].Cells[13].Value.ToString())
                                            - Double.Parse(statDesc.BlueMin.ToString())),2) + System.Math.Pow((Double.Parse(dataGridView1.Rows[row.Index].Cells[14].Value.ToString())
                                            - Double.Parse(statDesc.BlueMax.ToString())),2)
                                            ).ToString();
                                var min = (from DataGridViewRow r in dataGridView1.Rows
                                           where r.Cells[15].FormattedValue.ToString() != string.Empty
                                           select
                                           Convert.ToDouble(r.Cells[15].FormattedValue)).Min().ToString();
                                
                                label1.Text = min;
                                int rowindex = -1;
                                DataGridViewRow rowo = dataGridView1.Rows
                                    .Cast<DataGridViewRow>()
                                    .Where(r => r.Cells[15].Value.ToString().Equals(min))
                                    .First();
                                rowindex = rowo.Index;
                                label2.Text = dataGridView1.Rows[rowindex].Cells[2].Value.ToString(); 
                                
                            }
                    }
                    else
                    { 
                    }
                    }
                    catch (Exception)
                    {
                    }
                }
            }
            else
            {   
            }
        }
    
    private void timer2_Tick(object sender, EventArgs e)
        {
            if (_kMeans2 != null)
            {
                if (!_kMeans2.Converged)
                {
                    _kMeans2.Iterate();//ITERATE BEKERJA
                    pictureBox6.Image = _kMeans2.ProcessedImage;
                    pictureBox6.Refresh();
                    _count2++;
                }

                else
                {
                    _count2 = 0;

                    timer2.Enabled = false;
                    timer2.Stop();
                    //================================
                    try
                    {

                        if (pictureBox6.Image != null)
                        {
                            imagestat = (Bitmap)pictureBox6.Image.Clone();
                            ColorImageStatisticsDescription statDesc = new ColorImageStatisticsDescription(imagestat);

                            foreach (DataGridViewRow row in dataGridView1.Rows)
                            {
                                ///menghitung pendekatan
                                dataGridView1.Rows[row.Index].Cells[16].Value = (
                                    System.Math.Pow((Double.Parse(dataGridView1.Rows[row.Index].Cells[3].Value.ToString())
                                            - Double.Parse(statDesc.RedMean.ToString())),2) + System.Math.Pow((Double.Parse(dataGridView1.Rows[row.Index].Cells[4].Value.ToString())
                                            - Double.Parse(statDesc.RedStdDev.ToString())),2) + System.Math.Pow((Double.Parse(dataGridView1.Rows[row.Index].Cells[5].Value.ToString())
                                            - Double.Parse(statDesc.RedMin.ToString())),2) + System.Math.Pow((Double.Parse(dataGridView1.Rows[row.Index].Cells[6].Value.ToString())
                                            - Double.Parse(statDesc.RedMax.ToString())),2) + System.Math.Pow((Double.Parse(dataGridView1.Rows[row.Index].Cells[7].Value.ToString())
                                            - Double.Parse(statDesc.GreenMean.ToString())),2) + System.Math.Pow((Double.Parse(dataGridView1.Rows[row.Index].Cells[8].Value.ToString())
                                            - Double.Parse(statDesc.GreenStdDev.ToString())),2) + System.Math.Pow((Double.Parse(dataGridView1.Rows[row.Index].Cells[9].Value.ToString())
                                            - Double.Parse(statDesc.GreenMin.ToString())),2) + System.Math.Pow((Double.Parse(dataGridView1.Rows[row.Index].Cells[10].Value.ToString())
                                            - Double.Parse(statDesc.GreenMax.ToString())),2) + System.Math.Pow((Double.Parse(dataGridView1.Rows[row.Index].Cells[11].Value.ToString())
                                            - Double.Parse(statDesc.BlueMean.ToString())),2) + System.Math.Pow((Double.Parse(dataGridView1.Rows[row.Index].Cells[12].Value.ToString())
                                            - Double.Parse(statDesc.BlueStdDev.ToString())),2) + System.Math.Pow((Double.Parse(dataGridView1.Rows[row.Index].Cells[13].Value.ToString())
                                            - Double.Parse(statDesc.BlueMin.ToString())),2) + System.Math.Pow((Double.Parse(dataGridView1.Rows[row.Index].Cells[14].Value.ToString())
                                            - Double.Parse(statDesc.BlueMax.ToString())),2)
                                            ).ToString();

                                var min = (from DataGridViewRow r in dataGridView1.Rows
                                           where r.Cells[16].FormattedValue.ToString() != string.Empty
                                           select
                                               Convert.ToDouble(r.Cells[16].FormattedValue)).Min().ToString();

                                label4.Text = min;
                                int rowindex = -1;
                                DataGridViewRow rowo = dataGridView1.Rows
                                    .Cast<DataGridViewRow>()
                                    .Where(r => r.Cells[16].Value.ToString().Equals(min))
                                    .First();
                                rowindex = rowo.Index;
                                label3.Text = dataGridView1.Rows[rowindex].Cells[2].Value.ToString();
                                
                                
                            }   
                        }
                        else
                        {     
                        }
                    }
                    catch (Exception)
                    {
                    }
                }
            }
            else
            {
                
            }
        }

        private void timer3_Tick(object sender, EventArgs e)
        {
            if (_kMeans3 != null)
            {
                if (!_kMeans3.Converged)
                {
                    _kMeans3.Iterate();//ITERATE BEKERJA
                    pictureBox3.Image = _kMeans3.ProcessedImage;
                    pictureBox3.Refresh();
                    _count3++;
                }

                else
                {
                    _count3 = 0;

                    timer3.Enabled = false;
                    timer3.Stop();
                    //================================
                    try
                    {

                        if (pictureBox3.Image != null)
                        {
                            imagestat = (Bitmap)pictureBox3.Image.Clone();
                            ColorImageStatisticsDescription statDesc = new ColorImageStatisticsDescription(imagestat);

                            foreach (DataGridViewRow row in dataGridView1.Rows)
                            {
                                ///menghitung pendekatan
                                dataGridView1.Rows[row.Index].Cells[17].Value = (
                                    System.Math.Pow((Double.Parse(dataGridView1.Rows[row.Index].Cells[3].Value.ToString())
                                            - Double.Parse(statDesc.RedMean.ToString())),2) + System.Math.Pow((Double.Parse(dataGridView1.Rows[row.Index].Cells[4].Value.ToString())
                                            - Double.Parse(statDesc.RedStdDev.ToString())),2) + System.Math.Pow((Double.Parse(dataGridView1.Rows[row.Index].Cells[5].Value.ToString())
                                            - Double.Parse(statDesc.RedMin.ToString())),2) + System.Math.Pow((Double.Parse(dataGridView1.Rows[row.Index].Cells[6].Value.ToString())
                                            - Double.Parse(statDesc.RedMax.ToString())),2) + System.Math.Pow((Double.Parse(dataGridView1.Rows[row.Index].Cells[7].Value.ToString())
                                            - Double.Parse(statDesc.GreenMean.ToString())),2) + System.Math.Pow((Double.Parse(dataGridView1.Rows[row.Index].Cells[8].Value.ToString())
                                            - Double.Parse(statDesc.GreenStdDev.ToString())),2) + System.Math.Pow((Double.Parse(dataGridView1.Rows[row.Index].Cells[9].Value.ToString())
                                            - Double.Parse(statDesc.GreenMin.ToString())),2) + System.Math.Pow((Double.Parse(dataGridView1.Rows[row.Index].Cells[10].Value.ToString())
                                            - Double.Parse(statDesc.GreenMax.ToString())),2) + System.Math.Pow((Double.Parse(dataGridView1.Rows[row.Index].Cells[11].Value.ToString())
                                            - Double.Parse(statDesc.BlueMean.ToString())),2) + System.Math.Pow((Double.Parse(dataGridView1.Rows[row.Index].Cells[12].Value.ToString())
                                            - Double.Parse(statDesc.BlueStdDev.ToString())),2) + System.Math.Pow((Double.Parse(dataGridView1.Rows[row.Index].Cells[13].Value.ToString())
                                            - Double.Parse(statDesc.BlueMin.ToString())),2) + System.Math.Pow((Double.Parse(dataGridView1.Rows[row.Index].Cells[14].Value.ToString())
                                            - Double.Parse(statDesc.BlueMax.ToString())),2)
                                            ).ToString();

                                var min = (from DataGridViewRow r in dataGridView1.Rows
                                           where r.Cells[17].FormattedValue.ToString() != string.Empty
                                           select
                                               Convert.ToDouble(r.Cells[17].FormattedValue)).Min().ToString();

                                label6.Text = min;
                                int rowindex = -1;
                                DataGridViewRow rowo = dataGridView1.Rows
                                    .Cast<DataGridViewRow>()
                                    .Where(r => r.Cells[17].Value.ToString().Equals(min))
                                    .First();
                                rowindex = rowo.Index;
                                label5.Text = dataGridView1.Rows[rowindex].Cells[2].Value.ToString();
                                 
                            }
                        }
                        else
                        {    
                        }
                    }
                    catch (Exception)
                    {
                    }
                }
            }
            else
            {  
            }
        }

        private void timer4_Tick(object sender, EventArgs e)
        {
            if (_kMeans4 != null)
            {
                if (!_kMeans4.Converged)
                {
                    _kMeans4.Iterate();//ITERATE BEKERJA
                    pictureBox7.Image = _kMeans4.ProcessedImage;
                    pictureBox7.Refresh();
                    _count4++;
                }
                else
                {
                    _count4 = 0;

                    timer4.Enabled = false;
                    timer4.Stop();
                    //================================
                    try
                    {

                        if (pictureBox7.Image != null)
                        {
                            imagestat = (Bitmap)pictureBox7.Image.Clone();
                            ColorImageStatisticsDescription statDesc = new ColorImageStatisticsDescription(imagestat);

                            foreach (DataGridViewRow row in dataGridView1.Rows)
                            {
                                ///menghitung pendekatan
                                dataGridView1.Rows[row.Index].Cells[18].Value = (
                                  System.Math.Pow((Double.Parse(dataGridView1.Rows[row.Index].Cells[3].Value.ToString())
                                            - Double.Parse(statDesc.RedMean.ToString())), 2) + System.Math.Pow((Double.Parse(dataGridView1.Rows[row.Index].Cells[4].Value.ToString())
                                            - Double.Parse(statDesc.RedStdDev.ToString())), 2) + System.Math.Pow((Double.Parse(dataGridView1.Rows[row.Index].Cells[5].Value.ToString())
                                            - Double.Parse(statDesc.RedMin.ToString())), 2) + System.Math.Pow((Double.Parse(dataGridView1.Rows[row.Index].Cells[6].Value.ToString())
                                            - Double.Parse(statDesc.RedMax.ToString())), 2) + System.Math.Pow((Double.Parse(dataGridView1.Rows[row.Index].Cells[7].Value.ToString())
                                            - Double.Parse(statDesc.GreenMean.ToString())), 2) + System.Math.Pow((Double.Parse(dataGridView1.Rows[row.Index].Cells[8].Value.ToString())
                                            - Double.Parse(statDesc.GreenStdDev.ToString())), 2) + System.Math.Pow((Double.Parse(dataGridView1.Rows[row.Index].Cells[9].Value.ToString())
                                            - Double.Parse(statDesc.GreenMin.ToString())), 2) + System.Math.Pow((Double.Parse(dataGridView1.Rows[row.Index].Cells[10].Value.ToString())
                                            - Double.Parse(statDesc.GreenMax.ToString())), 2) + System.Math.Pow((Double.Parse(dataGridView1.Rows[row.Index].Cells[11].Value.ToString())
                                            - Double.Parse(statDesc.BlueMean.ToString())), 2) + System.Math.Pow((Double.Parse(dataGridView1.Rows[row.Index].Cells[12].Value.ToString())
                                            - Double.Parse(statDesc.BlueStdDev.ToString())), 2) + System.Math.Pow((Double.Parse(dataGridView1.Rows[row.Index].Cells[13].Value.ToString())
                                            - Double.Parse(statDesc.BlueMin.ToString())), 2) + System.Math.Pow((Double.Parse(dataGridView1.Rows[row.Index].Cells[14].Value.ToString())
                                            - Double.Parse(statDesc.BlueMax.ToString())), 2)
                                            ).ToString();

                                var min = (from DataGridViewRow r in dataGridView1.Rows
                                           where r.Cells[18].FormattedValue.ToString() != string.Empty
                                           select
                                               Convert.ToDouble(r.Cells[18].FormattedValue)).Min().ToString();

                                label8.Text = min;
                                int rowindex = -1;
                                DataGridViewRow rowo = dataGridView1.Rows
                                    .Cast<DataGridViewRow>()
                                    .Where(r => r.Cells[18].Value.ToString().Equals(min))
                                    .First();
                                rowindex = rowo.Index;
                                label7.Text = dataGridView1.Rows[rowindex].Cells[2].Value.ToString();                       
                            }
                        }
                        else
                        {
                        }
                    }
                    catch (Exception)
                    {
                    }
                }
            }
            else
            {
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "BMP Images(*.bmp)|*.bmp|Jpeg Images(*.jpg)|*.jpg";
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                //label2.Text = "Lokasi File :" + ofd.FileName;
                Bitmap img = new Bitmap(ofd.FileName);
                pictureBox2.Image = new Bitmap(ofd.FileName);
                pictureBox1.Image = BlobDetection(img);
                
            }
        }

        private void learningToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LEARNING frm = new LEARNING();
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
