using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace GrowCut
{
    public partial class DrawControll : UserControl
    {
        private int startX;
        private int startY;
        private int endX;
        private int endY;
        private bool isDrawing;
        private bool isPositiveMode;
        private Bitmap image;

        public Bitmap getImage()
        {
            return (Bitmap)imageBox.Image;
        }

        public void setImage(Bitmap image)
        {
            imageBox.Image = image;
            drawBox.Image = new Bitmap(image.Width, image.Height);
        }

        public DrawControll()
        {
            InitializeComponent();
            drawBox.BackColor = Color.Transparent;
            drawBox.Parent = imageBox;
            System.Console.Out.WriteLine(imageBox.Parent.Size);
        }


        #region draw
        private void drawBox_MouseUp(object sender, MouseEventArgs e)
        {
        }

        private void drawBox_MouseMove(object sender, MouseEventArgs e)
        {
            if (isDrawing)
            {
                endX = e.X;
                endY = e.Y;
            }
        }

        private void drawBox_Paint(object sender, PaintEventArgs e)
        {
            if (isDrawing)
            {
                if (isPositiveMode) {
                    System.Console.Out.WriteLine("Positive");
                    e.Graphics.DrawEllipse(new Pen(Color.Blue, 2), endX, endY, 10, 10);
                } else
                {
                    System.Console.Out.WriteLine("Negative");
                    e.Graphics.DrawEllipse(new Pen(Color.Blue, 2), endX, endY, 10, 10);
                }
                e.Graphics.Save();
                drawBox.Invalidate();
            }
        }

        private void drawBox_MouseDown(object sender, MouseEventArgs e)
        {
            startX = e.X;
            startY = e.Y;
            if (e.Button == MouseButtons.Left)
            {
                isPositiveMode = true;
            } else if (e.Button == MouseButtons.Right)
            {
                isPositiveMode = false;
            }
            isDrawing = true;
        }
        #endregion

    }
}
