using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using GrowCut;

namespace GrowCut
{
    public partial class Form1 : Form
    {
        private bool isDrawing;
        private bool isPositive;
        private int startX;
        private int startY;
        private int endX;
        private int endY;

        public Form1()
        {
            InitializeComponent();
            drawBox.BackColor = Color.Transparent;
            drawBox.Parent = pictureBox;
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();

            //openFileDialog1.InitialDirectory = "c:\\";
            openFileDialog1.Filter = "Image Files(*.BMP;*.JPG;*.GIF)|*.BMP;*.JPG;*.GIF|All files (*.*)|*.*";
            openFileDialog1.FilterIndex = 2;
            openFileDialog1.RestoreDirectory = true;

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                pictureBox.Image = new Bitmap(openFileDialog1.FileName);
                drawBox.Image = new Bitmap(pictureBox.Image.Width, pictureBox.Image.Height);
                
            }
        }

        private void drawBox_MouseDown(object sender, MouseEventArgs e)
        {
            if (drawBox.Image != null)
            {
                if (e.Button == MouseButtons.Left)
                {
                    isPositive = true;
                } else if (e.Button == MouseButtons.Right)
                {
                    isPositive = false;
                }
                isDrawing = true;
                startX = e.X;
                startY = e.Y;
            }
        }

        private void drawBox_MouseMove(object sender, MouseEventArgs e)
        {
            if (isDrawing)
            {
                Graphics graphics = Graphics.FromImage(drawBox.Image);
                if (isPositive)
                {
                    graphics.DrawLine(new Pen(Color.Red, 11), startX, startY, e.X, e.Y);
                    graphics.FillEllipse(Brushes.Red, e.X - 5, e.Y - 5, 11, 11);
                } else
                {
                    graphics.DrawLine(new Pen(Color.Blue, 11), startX, startY, e.X, e.Y);
                    graphics.FillEllipse(Brushes.Blue, e.X - 5, e.Y - 5, 11, 11);

                }
                graphics.Save();
                drawBox.Invalidate();
                startX = e.X;
                startY = e.Y;
            }
        }

        private void drawBox_MouseUp(object sender, MouseEventArgs e)
        {
            isDrawing = false;
        }

        private void updatePictureWithCells(CellDescription[,] cells)
        {
            //Bitmap image = (Bitmap)pictureBox.Image.Clone();
            //Parallel.For(0, image.Width, x =>
            //{
            //    for (int y = 0; y < image.Height; y++)
            //    {
            //        if (cells[x, y].label == 1)
            //        {
            //            image.SetPixel(x, y, Color.Blue);
            //        } else
            //        {
            //            image.SetPixel(x, y, Color.Red);
            //        }
            //    }
            //});

            Bitmap resultImage = (Bitmap)pictureBox.Image.Clone();
            for (int x = 0; x < resultImage.Width; x++)
            {
                {
                    for (int y = 0; y < resultImage.Height; y++)
                    {
                        int[] currentPixel = GrowCut.colorToVector(resultImage.GetPixel(x, y));
                        if (cells[x, y].label == 1)
                        {
                            int blue = Math.Min(255, currentPixel[2] + 100);
                            resultImage.SetPixel(x, y, Color.FromArgb(currentPixel[0], currentPixel[1], blue));
                        }
                        else
                        {
                            int red = Math.Min(255, currentPixel[0] + 100);
                            resultImage.SetPixel(x, y, Color.FromArgb(red, currentPixel[1], currentPixel[2]));
                        }
                    }
                }
                pictureBox.Image = resultImage;
            }
        }

        private void testToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CellDescription[,] test = GrowCut.getCellsMap((Bitmap)pictureBox.Image, (Bitmap)drawBox.Image);
            GrowCut growCut = new GrowCut((Bitmap)pictureBox.Image.Clone(), test);
            growCut.evolution(updatePictureWithCells);
            
        }
    }
}
