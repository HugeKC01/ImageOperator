using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MyFirst
{
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
        }

        private void Form2_Load(object sender, EventArgs e)
        {

        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (pictureBox1.Image == null)
            {
                MessageBox.Show("No image to save.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            using (SaveFileDialog dlg = new SaveFileDialog())
            {
                dlg.Title = "Save image";
                dlg.Filter = "PNG Image|*.png|JPEG Image|*.jpg;*.jpeg|BMP Image|*.bmp|GIF Image|*.gif|TIFF Image|*.tif;*.tiff";
                dlg.AddExtension = true;
                dlg.FileName = "output";
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        // Determine format from extension
                        var ext = System.IO.Path.GetExtension(dlg.FileName).ToLowerInvariant();
                        System.Drawing.Imaging.ImageFormat fmt = System.Drawing.Imaging.ImageFormat.Png;
                        switch (ext)
                        {
                            case ".jpg":
                            case ".jpeg": fmt = System.Drawing.Imaging.ImageFormat.Jpeg; break;
                            case ".bmp": fmt = System.Drawing.Imaging.ImageFormat.Bmp; break;
                            case ".gif": fmt = System.Drawing.Imaging.ImageFormat.Gif; break;
                            case ".tif":
                            case ".tiff": fmt = System.Drawing.Imaging.ImageFormat.Tiff; break;
                        }
                        // Save a copy (avoid locking original bitmap owned by Form1)
                        using (Bitmap copy = new Bitmap(pictureBox1.Image))
                        {
                            copy.Save(dlg.FileName, fmt);
                        }
                        MessageBox.Show("Image saved successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Failed to save image: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (pictureBox1.Image != null)
            {
                pictureBox1.Image.Dispose();
                pictureBox1.Image = null;
            }
            Close();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (pictureBox1.Image != null)
            {
                pictureBox1.Image.Dispose();
                pictureBox1.Image = null;
            }
            Application.Exit();
        }
    }
}
