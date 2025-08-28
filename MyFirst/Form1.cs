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

            // Show greyscale image in outputForm's PictureBox
            if (outputForm.Controls["pictureBox1"] is PictureBox outputPictureBox)
            {
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
    }
}
