using System.Numerics;

namespace Fast_Fourier_Transform
{
    public partial class Form1 : Form
    {
        private Complex[,]? lastFFTData; // stores uncentered forward FFT data
        private int lastWidth;
        private int lastHeight;
        private bool hasFFT;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            loadImage();
        }

        private void loadImage()
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Title = "Select an image";
                openFileDialog.Filter = "Image Files|*.bmp;*.jpg;*.jpeg;*.png;*.gif;*.tif;*.tiff";
                openFileDialog.RestoreDirectory = true;

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        Image img = Image.FromFile(openFileDialog.FileName);
                        pictureBox1.Image = new Bitmap(img);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error loading image: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }

                    FourierPhasePlot();
                }
            }
        }

        private void loadImageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            loadImage();
        }

        private void FourierPhasePlot()
        {
            if (pictureBox1.Image == null)
            {
                MessageBox.Show("No image loaded.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            Bitmap bmp = new Bitmap(pictureBox1.Image);
            int width = bmp.Width;
            int height = bmp.Height;

            double[,] gray = new double[width, height];
            for (int y = 0; y < height; y++)
                for (int x = 0; x < width; x++)
                {
                    Color c = bmp.GetPixel(x, y);
                    gray[x, y] = (c.R + c.G + c.B) / 3.0;
                }

            Complex[,] data = new Complex[width, height];
            for (int y = 0; y < height; y++)
                for (int x = 0; x < width; x++)
                    data[x, y] = new Complex(gray[x, y], 0);

            FFT2D(data, true);

            // store for inverse
            lastFFTData = data;
            lastWidth = width;
            lastHeight = height;
            hasFFT = true;

            Complex[,] centered = new Complex[width, height];
            int halfW = width / 2, halfH = height / 2;
            for (int y = 0; y < height; y++)
                for (int x = 0; x < width; x++)
                {
                    int cx = (x + halfW) % width;
                    int cy = (y + halfH) % height;
                    centered[cx, cy] = data[x, y];
                }

            Bitmap phaseBmp = new Bitmap(width, height);
            for (int y = 0; y < height; y++)
                for (int x = 0; x < width; x++)
                {
                    double phase = centered[x, y].Phase;
                    int val = (int)(255 * (phase + Math.PI) / (2 * Math.PI));
                    val = Math.Max(0, Math.Min(255, val));
                    phaseBmp.SetPixel(x, y, Color.FromArgb(val, val, val));
                }

            if (pictureBox3.Image != null)
            {
                pictureBox3.Image.Dispose();
                pictureBox3.Image = null;
            }
            pictureBox3.Image = phaseBmp;
        }

        private void FourierMagnitudePlot()
        {
            if (pictureBox1.Image == null)
            {
                MessageBox.Show("No image loaded.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            Bitmap bmp = new Bitmap(pictureBox1.Image);
            int width = bmp.Width;
            int height = bmp.Height;

            double[,] gray = new double[width, height];
            for (int y = 0; y < height; y++)
                for (int x = 0; x < width; x++)
                {
                    Color c = bmp.GetPixel(x, y);
                    gray[x, y] = (c.R + c.G + c.B) / 3.0;
                }

            Complex[,] data = new Complex[width, height];
            for (int y = 0; y < height; y++)
                for (int x = 0; x < width; x++)
                    data[x, y] = new Complex(gray[x, y], 0);

            FFT2D(data, true);

            // store for inverse
            lastFFTData = data;
            lastWidth = width;
            lastHeight = height;
            hasFFT = true;

            Complex[,] centered = new Complex[width, height];
            int halfW = width / 2, halfH = height / 2;
            for (int y = 0; y < height; y++)
                for (int x = 0; x < width; x++)
                {
                    int cx = (x + halfW) % width;
                    int cy = (y + halfH) % height;
                    centered[cx, cy] = data[x, y];
                }

            double[,] mag = new double[width, height];
            double maxMag = double.MinValue;
            for (int y = 0; y < height; y++)
                for (int x = 0; x < width; x++)
                {
                    double m = centered[x, y].Magnitude;
                    double lm = Math.Log(1 + m);
                    mag[x, y] = lm;
                    if (lm > maxMag) maxMag = lm;
                }

            if (maxMag <= 0) maxMag = 1;

            Bitmap magBmp = new Bitmap(width, height);
            for (int y = 0; y < height; y++)
                for (int x = 0; x < width; x++)
                {
                    int val = (int)(255 * (mag[x, y] / maxMag));
                    val = Math.Clamp(val, 0, 255);
                    magBmp.SetPixel(x, y, Color.FromArgb(val, val, val));
                }

            if (pictureBox4.Image != null)
            {
                pictureBox4.Image.Dispose();
                pictureBox4.Image = null;
            }
            pictureBox4.Image = magBmp;
        }

        private Complex[] PadToPowerOfTwo(Complex[] input)
        {
            int n = input.Length;
            int pow2 = 1;
            while (pow2 < n) pow2 <<= 1;
            if (pow2 == n) return input;

            Complex[] padded = new Complex[pow2];
            Array.Copy(input, padded, n);
            // Remaining elements are already zero (default for Complex)
            return padded;
        }

        // Simple 2D FFT (row-column method)
        private void FFT2D(Complex[,] data, bool forward)
        {
            int width = data.GetLength(0);
            int height = data.GetLength(1);

            // FFT rows
            for (int y = 0; y < height; y++)
            {
                Complex[] row = new Complex[width];
                for (int x = 0; x < width; x++)
                    row[x] = data[x, y];
                row = PadToPowerOfTwo(row);
                FFT1D(row, forward);
                for (int x = 0; x < width; x++)
                    data[x, y] = row[x];
            }

            // FFT columns
            for (int x = 0; x < width; x++)
            {
                Complex[] col = new Complex[height];
                for (int y = 0; y < height; y++)
                    col[y] = data[x, y];
                col = PadToPowerOfTwo(col);
                FFT1D(col, forward);
                for (int y = 0; y < height; y++)
                    data[x, y] = col[y];
            }
        }

        // Simple 1D Cooley-Tukey FFT
        private void FFT1D(Complex[] buffer, bool forward)
        {
            int n = buffer.Length;
            int bits = (int)Math.Log2(n);
            for (int j = 1, i = 0; j < n; j++)
            {
                int bit = n >> 1;
                for (; (i & bit) != 0; bit >>= 1)
                    i ^= bit;
                i ^= bit;

                if (j < i)
                {
                    var temp = buffer[j];
                    buffer[j] = buffer[i];
                    buffer[i] = temp;
                }
            }

            for (int len = 2; len <= n; len <<= 1)
            {
                double ang = 2 * Math.PI / len * (forward ? -1 : 1);
                Complex wlen = new Complex(Math.Cos(ang), Math.Sin(ang));
                for (int i = 0; i < n; i += len)
                {
                    Complex w = Complex.One;
                    for (int j = 0; j < len / 2; j++)
                    {
                        Complex u = buffer[i + j];
                        Complex v = buffer[i + j + len / 2] * w;
                        buffer[i + j] = u + v;
                        buffer[i + j + len / 2] = u - v;
                        w *= wlen;
                    }
                }
            }

            if (!forward)
            {
                for (int i = 0; i < n; i++)
                    buffer[i] /= n;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            FourierMagnitudePlot();
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {

        }
    }
}
