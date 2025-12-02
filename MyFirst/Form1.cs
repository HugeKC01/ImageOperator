namespace MyFirst
{
    public partial class Form1 : Form
    {
        Form2 outputForm = new Form2();
        public Form1()
        {
            InitializeComponent();
        }


        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            pictureBox1.Image = null;
        }

        private void imageToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void loadImageToolStripMenuItem_Click(object sender, EventArgs e)
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

                        // Ensure SizeMode is Zoom
                        pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;

                        // Get the working area of the primary screen (excluding taskbar)
                        Rectangle workingArea = Screen.PrimaryScreen.WorkingArea;
                        int imgWidth = img.Width;
                        int imgHeight = img.Height;
                        int maxWidth = workingArea.Width;
                        int maxHeight = workingArea.Height;

                        // Limit PictureBox size and reduce by 10%
                        int pbWidth = (int)(Math.Min(imgWidth, maxWidth) * 0.9);
                        int pbHeight = (int)(Math.Min(imgHeight, maxHeight) * 0.9);
                        pictureBox1.Width = pbWidth;
                        pictureBox1.Height = pbHeight;

                        // Calculate the required form size to fit the PictureBox and menu
                        int menuHeight = menuStrip1?.Height ?? 0;
                        int formWidth = pbWidth + (this.Width - this.ClientSize.Width);
                        int formHeight = pbHeight + menuHeight + (this.Height - this.ClientSize.Height);

                        this.Size = new Size(formWidth, formHeight);
                        EnsureOutputForm();
                        outputForm.Size = this.Size;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error loading image: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void greyscaleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (pictureBox1.Image == null)
            {
                MessageBox.Show("No image loaded.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Create a greyscale copy of the image
            Bitmap original = new Bitmap(pictureBox1.Image);
            Bitmap greyscale = new Bitmap(original.Width, original.Height);

            for (int y = 0; y < original.Height; y++)
            {
                for (int x = 0; x < original.Width; x++)
                {
                    Color pixelColor = original.GetPixel(x, y);
                    int grey = (int)(pixelColor.R * 0.299 + pixelColor.G * 0.587 + pixelColor.B * 0.114);
                    Color greyColor = Color.FromArgb(grey, grey, grey);
                    greyscale.SetPixel(x, y, greyColor);
                }
            }

            EnsureOutputForm();
            if (outputForm.Controls["pictureBox1"] is PictureBox outputPictureBox)
            {
                if (outputPictureBox.Image != null)
                {
                    outputPictureBox.Image.Dispose();
                    outputPictureBox.Image = null;
                }
                outputPictureBox.SizeMode = PictureBoxSizeMode.Zoom;
                outputPictureBox.Image = greyscale;
            }
            else
            {
                MessageBox.Show("Output PictureBox not found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            outputForm.Size = this.Size;
            // Optionally, show the output form if not visible
            if (!outputForm.Visible)
            {
                outputForm.Show();
            }
        }

        private void exitToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Dispose the image in pictureBox1 if it exists
            if (pictureBox1.Image != null)
            {
                pictureBox1.Image.Dispose();
                pictureBox1.Image = null;
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void sobelToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (pictureBox1.Image == null)
            {
                MessageBox.Show("No image loaded.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            Console.WriteLine("Sobel filter started.");

            // Convert to greyscale first
            Bitmap original = new Bitmap(pictureBox1.Image);
            Bitmap greyscale = new Bitmap(original.Width, original.Height);

            for (int y = 0; y < original.Height; y++)
            {
                for (int x = 0; x < original.Width; x++)
                {
                    Color pixelColor = original.GetPixel(x, y);
                    int grey = (int)(pixelColor.R * 0.299 + pixelColor.G * 0.587 + pixelColor.B * 0.114);
                    greyscale.SetPixel(x, y, Color.FromArgb(grey, grey, grey));
                }
                if (y % 50 == 0) Console.WriteLine($"Greyscale row {y} processed.");
            }

            // Sobel kernels
            int[,] gx = new int[3, 3] {
                { -1, 0, 1 },
                { -2, 0, 2 },
                { -1, 0, 1 }
            };
            int[,] gy = new int[3, 3] {
                { -1, -2, -1 },
                {  0,  0,  0 },
                {  1,  2,  1 }
            };

            Bitmap sobel = new Bitmap(original.Width, original.Height);

            for (int y = 1; y < original.Height - 1; y++)
            {
                for (int x = 1; x < original.Width - 1; x++)
                {
                    int sumX = 0;
                    int sumY = 0;

                    // Apply Sobel kernels
                    for (int ky = -1; ky <= 1; ky++)
                    {
                        for (int kx = -1; kx <= 1; kx++)
                        {
                            int pixel = greyscale.GetPixel(x + kx, y + ky).R;
                            sumX += gx[ky + 1, kx + 1] * pixel;
                            sumY += gy[ky + 1, kx + 1] * pixel;
                        }
                    }

                    int magnitude = (int)Math.Min(255, Math.Sqrt(sumX * sumX + sumY * sumY));
                    sobel.SetPixel(x, y, Color.FromArgb(magnitude, magnitude, magnitude));
                }
                if (y % 50 == 0) Console.WriteLine($"Sobel row {y} processed.");
            }

            Console.WriteLine("Sobel filter finished.");

            // Show Sobel image in outputForm's PictureBox
            EnsureOutputForm();
            if (outputForm.Controls["pictureBox1"] is PictureBox outputPictureBox)
            {
                if (outputPictureBox.Image != null)
                {
                    outputPictureBox.Image.Dispose();
                    outputPictureBox.Image = null;
                }
                outputPictureBox.SizeMode = PictureBoxSizeMode.Zoom;
                outputPictureBox.Image = sobel;
            }
            else
            {
                MessageBox.Show("Output PictureBox not found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            outputForm.Size = this.Size;
            if (!outputForm.Visible)
            {
                outputForm.Show();
            }
        }

        private void sepiaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (pictureBox1.Image == null)
            {
                MessageBox.Show("No image loaded.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Create a sepia copy of the image
            Bitmap original = new Bitmap(pictureBox1.Image);
            Bitmap sepia = new Bitmap(original.Width, original.Height);

            for (int y = 0; y < original.Height; y++)
            {
                for (int x = 0; x < original.Width; x++)
                {
                    Color pixelColor = original.GetPixel(x, y);

                    int tr = (int)(0.393 * pixelColor.R + 0.769 * pixelColor.G + 0.189 * pixelColor.B);
                    int tg = (int)(0.349 * pixelColor.R + 0.686 * pixelColor.G + 0.168 * pixelColor.B);
                    int tb = (int)(0.272 * pixelColor.R + 0.534 * pixelColor.G + 0.131 * pixelColor.B);

                    tr = Math.Min(255, tr);
                    tg = Math.Min(255, tg);
                    tb = Math.Min(255, tb);

                    sepia.SetPixel(x, y, Color.FromArgb(tr, tg, tb));
                }
            }

            // Show sepia image in outputForm's PictureBox
            EnsureOutputForm();
            if (outputForm.Controls["pictureBox1"] is PictureBox outputPictureBox)
            {
                if (outputPictureBox.Image != null)
                {
                    outputPictureBox.Image.Dispose();
                    outputPictureBox.Image = null;
                }
                outputPictureBox.SizeMode = PictureBoxSizeMode.Zoom;
                outputPictureBox.Image = sepia;
            }
            else
            {
                MessageBox.Show("Output PictureBox not found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            outputForm.Size = this.Size;
            if (!outputForm.Visible)
            {
                outputForm.Show();
            }
        }

        private void x3ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ApplyAverageFilter(3);
        }

        private void x5ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ApplyAverageFilter(5);
        }

        private void x10ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ApplyAverageFilter(10);
        }

        private void ApplyAverageFilter(int kernelSize)
        {
            if (pictureBox1.Image == null)
            {
                MessageBox.Show("No image loaded.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            Bitmap original = new Bitmap(pictureBox1.Image);
            Bitmap result = new Bitmap(original.Width, original.Height);

            int offset = kernelSize / 2;

            for (int y = offset; y < original.Height - offset; y++)
            {
                for (int x = offset; x < original.Width - offset; x++)
                {
                    int sumR = 0, sumG = 0, sumB = 0;
                    int count = 0;

                    for (int ky = -offset; ky <= offset; ky++)
                    {
                        for (int kx = -offset; kx <= offset; kx++)
                        {
                            Color pixel = original.GetPixel(x + kx, y + ky);
                            sumR += pixel.R;
                            sumG += pixel.G;
                            sumB += pixel.B;
                            count++;
                        }
                    }

                    int avgR = sumR / count;
                    int avgG = sumG / count;
                    int avgB = sumB / count;

                    result.SetPixel(x, y, Color.FromArgb(avgR, avgG, avgB));
                }
            }

            // Show result in outputForm's PictureBox
            EnsureOutputForm();
            if (outputForm.Controls["pictureBox1"] is PictureBox outputPictureBox)
            {
                if (outputPictureBox.Image != null)
                {
                    outputPictureBox.Image.Dispose();
                    outputPictureBox.Image = null;
                }
                outputPictureBox.SizeMode = PictureBoxSizeMode.Zoom;
                outputPictureBox.Image = result;
            }
            else
            {
                MessageBox.Show("Output PictureBox not found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            outputForm.Size = this.Size;
            if (!outputForm.Visible)
            {
                outputForm.Show();
            }
        }

        private void prewitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (pictureBox1.Image == null)
            {
                MessageBox.Show("No image loaded.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Convert to greyscale first
            Bitmap original = new Bitmap(pictureBox1.Image);
            Bitmap greyscale = new Bitmap(original.Width, original.Height);

            for (int y = 0; y < original.Height; y++)
            {
                for (int x = 0; x < original.Width; x++)
                {
                    Color pixelColor = original.GetPixel(x, y);
                    int grey = (int)(pixelColor.R * 0.299 + pixelColor.G * 0.587 + pixelColor.B * 0.114);
                    greyscale.SetPixel(x, y, Color.FromArgb(grey, grey, grey));
                }
            }

            // Prewitt kernels
            int[,] gx = new int[3, 3] {
                { -1, 0, 1 },
                { -1, 0, 1 },
                { -1, 0, 1 }
            };
            int[,] gy = new int[3, 3] {
                { -1, -1, -1 },
                {  0,  0,  0 },
                {  1,  1,  1 }
            };

            Bitmap prewitt = new Bitmap(original.Width, original.Height);

            for (int y = 1; y < original.Height - 1; y++)
            {
                for (int x = 1; x < original.Width - 1; x++)
                {
                    int sumX = 0;
                    int sumY = 0;

                    // Apply Prewitt kernels
                    for (int ky = -1; ky <= 1; ky++)
                    {
                        for (int kx = -1; kx <= 1; kx++)
                        {
                            int pixel = greyscale.GetPixel(x + kx, y + ky).R;
                            sumX += gx[ky + 1, kx + 1] * pixel;
                            sumY += gy[ky + 1, kx + 1] * pixel;
                        }
                    }

                    int magnitude = (int)Math.Min(255, Math.Sqrt(sumX * sumX + sumY * sumY));
                    prewitt.SetPixel(x, y, Color.FromArgb(magnitude, magnitude, magnitude));
                }
            }

            // Show Prewitt image in outputForm's PictureBox
            EnsureOutputForm();
            if (outputForm.Controls["pictureBox1"] is PictureBox outputPictureBox)
            {
                if (outputPictureBox.Image != null)
                {
                    outputPictureBox.Image.Dispose();
                    outputPictureBox.Image = null;
                }
                outputPictureBox.SizeMode = PictureBoxSizeMode.Zoom;
                outputPictureBox.Image = prewitt;
            }
            else
            {
                MessageBox.Show("Output PictureBox not found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            outputForm.Size = this.Size;
            if (!outputForm.Visible)
            {
                outputForm.Show();
            }
        }

        private void channelToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void redToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ShowColorChannel('R');
        }

        private void greenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ShowColorChannel('G');
        }

        private void blueToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ShowColorChannel('B');
        }

        private void ShowColorChannel(char channel)
        {
            if (pictureBox1.Image == null)
            {
                MessageBox.Show("No image loaded.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            Bitmap original = new Bitmap(pictureBox1.Image);
            Bitmap channelImage = new Bitmap(original.Width, original.Height);

            for (int y = 0; y < original.Height; y++)
            {
                for (int x = 0; x < original.Width; x++)
                {
                    Color pixel = original.GetPixel(x, y);
                    int r = (channel == 'R') ? pixel.R : 0;
                    int g = (channel == 'G') ? pixel.G : 0;
                    int b = (channel == 'B') ? pixel.B : 0;
                    channelImage.SetPixel(x, y, Color.FromArgb(r, g, b));
                }
            }

            EnsureOutputForm();
            if (outputForm.Controls["pictureBox1"] is PictureBox outputPictureBox)
            {
                if (outputPictureBox.Image != null)
                {
                    outputPictureBox.Image.Dispose();
                    outputPictureBox.Image = null;
                }
                outputPictureBox.SizeMode = PictureBoxSizeMode.Zoom;
                outputPictureBox.Image = channelImage;
            }
            else
            {
                MessageBox.Show("Output PictureBox not found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            outputForm.Size = this.Size;
            if (!outputForm.Visible)
            {
                outputForm.Show();
            }
        }

        private void cyanToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ShowCMYKChannel('C');
        }

        private void magentaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ShowCMYKChannel('M');
        }

        private void yellowToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ShowCMYKChannel('Y');
        }

        private void blackToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            ShowCMYKChannel('K');
        }

        // Helper for CMYK channels
        private void ShowCMYKChannel(char channel)
        {
            if (pictureBox1.Image == null)
            {
                MessageBox.Show("No image loaded.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            Bitmap original = new Bitmap(pictureBox1.Image);
            Bitmap cmykImage = new Bitmap(original.Width, original.Height);

            for (int y = 0; y < original.Height; y++)
            {
                for (int x = 0; x < original.Width; x++)
                {
                    Color pixel = original.GetPixel(x, y);
                    float r = pixel.R / 255f;
                    float g = pixel.G / 255f;
                    float b = pixel.B / 255f;

                    float k = 1 - Math.Max(r, Math.Max(g, b));
                    float c = (1 - r - k) / (1 - k + 1e-6f);
                    float m = (1 - g - k) / (1 - k + 1e-6f);
                    float yC = (1 - b - k) / (1 - k + 1e-6f);

                    int value = 0;
                    switch (channel)
                    {
                        case 'C': value = (int)(c * 255); break;
                        case 'M': value = (int)(m * 255); break;
                        case 'Y': value = (int)(yC * 255); break;
                        case 'K': value = (int)(k * 255); break;
                    }
                    value = Math.Clamp(value, 0, 255);
                    cmykImage.SetPixel(x, y, Color.FromArgb(value, value, value));
                }
            }

            ShowOutputImage(cmykImage);
        }

        private void blackToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (pictureBox1.Image == null)
            {
                MessageBox.Show("No image loaded.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            Bitmap original = new Bitmap(pictureBox1.Image);
            Bitmap alphaImage = new Bitmap(original.Width, original.Height);

            for (int y = 0; y < original.Height; y++)
            {
                for (int x = 0; x < original.Width; x++)
                {
                    Color pixel = original.GetPixel(x, y);
                    int a = pixel.A;
                    alphaImage.SetPixel(x, y, Color.FromArgb(255, a, a, a));
                }
            }

            ShowOutputImage(alphaImage);
        }

        // Helper to show image in outputForm
        private void ShowOutputImage(Bitmap image)
        {
            if (outputForm.Controls["pictureBox1"] is PictureBox outputPictureBox)
            {
                if (outputPictureBox.Image != null)
                {
                    outputPictureBox.Image.Dispose();
                    outputPictureBox.Image = null;
                }
                outputPictureBox.SizeMode = PictureBoxSizeMode.Zoom;
                outputPictureBox.Image = image;
            }
            else
            {
                MessageBox.Show("Output PictureBox not found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            outputForm.Size = this.Size;
            if (!outputForm.Visible)
            {
                outputForm.Show();
            }
        }

        // Creates a new instance of Form2 if the previous one was disposed.
        private void EnsureOutputForm()
        {
            if (outputForm == null || outputForm.IsDisposed)
            {
                outputForm = new Form2();
            }
        }
    }
}
