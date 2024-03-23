using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Tesseract;

namespace Screenshot
{
    public partial class Save : Form
    {
        Bitmap bmp;

        public Save(Int32 x, Int32 y, Int32 w, Int32 h, Size s)
        {
            InitializeComponent();
            Rectangle rect = new Rectangle(x, y, w, h);
            bmp = new Bitmap(rect.Width, rect.Height, PixelFormat.Format32bppArgb);

            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.CopyFromScreen(rect.Left, rect.Top, 0, 0, s, CopyPixelOperation.SourceCopy);
            }

            // Save to specified directory
            string directory = @"C:\Users\Asmak\Desktop";
            if (Directory.Exists(directory))
            {
                try
                {
                    bmp.Save(Path.Combine(directory, "screen.jpeg"), System.Drawing.Imaging.ImageFormat.Jpeg);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error saving screenshot: " + ex.Message);
                }
            }
            else
            {
                MessageBox.Show("Directory does not exist: " + directory);
            }

            // Save to temporary folder
            string imagePath = Path.Combine(Path.GetTempPath(), "screenshot.png");
            try
            {
                bmp.Save(imagePath, System.Drawing.Imaging.ImageFormat.Png);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error saving screenshot: " + ex.Message);
            }

            pbCapture.Image = bmp;

            var path = @"C:\Program Files\Tesseract-OCR\tessdata";
            try
            {
                using (var engine = new TesseractEngine(path, "eng", EngineMode.Default))
                {
                    using (var img = PixConverter.ToPix(bmp))
                    {
                        using (var page = engine.Process(img))
                        {
                            string text = page.GetText();
                            Clipboard.SetText(text);
                        }
                    }
                }
            }
            catch (TesseractException te)
            {
                MessageBox.Show($"An error occurred while initializing the Tesseract OCR engine: {te.Message}", "Tesseract Error");
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.CheckPathExists = true;
            sfd.FileName = "Capture";
            sfd.Filter = "PNG Image(*.png)|*.png|JPG Image(*.jpg)|*.jpg|BMP Image(*.bmp)|*.bmp";
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                pbCapture.Image.Save(sfd.FileName);
            }
        }

        private void btnHome_Click(object sender, EventArgs e)
        {
            frmHome home = new frmHome();
            this.Hide();
            home.Show();
        }
    }
}
