using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks;

namespace GrowCut
{
    public enum NeighborhoodSystem
    {
        vonNeumann,
        Moore
    }

    public struct CellDescription
    {
        public CellDescription(double strength, int label)
        {
            this.strength = strength;
            this.label = label;
        }

        // strength of cell
        public double strength;
        // label of cell
        public int label;
    }

    class GrowCut
    {
        private LockBitmap sourceImage;
        private LockBitmap resultImage;
        private CellDescription[,] states;
        private int width;
        private int height;
        private int windowSize = 5;
        public delegate void imageCallback(CellDescription[,] states);

        public GrowCut(Bitmap sourceImage, CellDescription[,] states)
        {
            this.sourceImage = new LockBitmap(sourceImage);
            this.resultImage = new LockBitmap((Bitmap)sourceImage.Clone());
            this.states = states;
            width = sourceImage.Width;
            height = sourceImage.Height;
            lockImages();
        }

        public static CellDescription[,] getCellsMap(Bitmap originalBitmap, Bitmap cellSeeds)
        {
            if (originalBitmap.Size != originalBitmap.Size)
            {
                throw new RankException("Image with label seed has different size");
            }

            CellDescription emptyCell = new CellDescription(strength: 0, label: 0);
            CellDescription[,] result = new CellDescription[originalBitmap.Width, originalBitmap.Height];
            Dictionary<Color, int> labels = new Dictionary<Color, int>();
            Color emptyPixel = Color.FromArgb(0, 0, 0, 0);
            int labelNumber = 0;

            for (int x = 0; x < originalBitmap.Width; x++)
            {
                for (int y = 0; y < originalBitmap.Height; y++)
                {
                    Color currentPixel = cellSeeds.GetPixel(x, y);

                    if (currentPixel == emptyPixel)
                    {
                        result[x, y] = emptyCell;
                    }
                    else if (labels.ContainsKey(cellSeeds.GetPixel(x, y)))
                    {
                        result[x, y] = new CellDescription(strength: 1.0, 
                            label: labels[currentPixel]);
                    }
                    else
                    {
                        labelNumber++;
                        labels.Add(currentPixel, labelNumber);
                        result[x, y] = new CellDescription(strength: 1.0,
                            label: labelNumber);
                    }
                }
            }

            return result;
        }

        public static int[] colorToVector(Color color)
        {
            return new int[3] { color.R, color.G, color.B };
        }


        /// <summary>
        /// 
        /// </summary>
        public void evolution(imageCallback callback)
        {
            int iterations = 500;
            int i = 0;
            int countOfChanges = 1;
            int ws = (windowSize - 1) / 2;

            while (i < iterations && countOfChanges != 0)
            {
                i++;
                CellDescription[,] newStates = (CellDescription[,])states.Clone();

                countOfChanges = 0;
                // Проходимся по всем клеткам
                Parallel.For(0, width, x =>
               {
                   for (int y = 0; y < height; y++)
                   {
                       int[] C_p = colorToVector(sourceImage.GetPixel(x, y));
                       CellDescription S_p = states[x, y];

                       // Соседи атакуют!
                       int startXX = Math.Max(0, x - ws);
                       int startYY = Math.Max(0, y - ws);
                       int endXX = Math.Min(x + ws + 1, width);
                       int endYY = Math.Min(y + ws + 1, height);

                       for (int xx = startXX; xx < endXX; xx++)
                       {
                           for (int yy = startYY; yy < endYY; yy++)
                           {
                               int[] C_q = colorToVector(sourceImage.GetPixel(xx, yy));
                               CellDescription S_q = states[xx, yy];

                               if (g(C_p, C_q) * S_q.strength > S_p.strength)
                               {
                                   newStates[x, y].label = S_q.label;
                                   newStates[x, y].strength = g(C_p, C_q);
                                   countOfChanges++;
                               }
                           }
                       }

                   }
               });
                Console.Out.WriteLine("Iteration : " + i);
                Console.Out.WriteLine("Count of changes : " + countOfChanges);
                states = newStates;
            }
            callback(states);
        }

        private double g(int[] x, int[] y)
        {
            return 1 - lenghtOfVector(subtractionOfVectors(x, y)) / 10;
            //return 1 - lenghtOfVector(subtractionOfVectors(x, y)) / Math.Max(lenghtOfVector(x), lenghtOfVector(y));
        }



        private double lenghtOfVector(int[] inputVector)
        {
            int result = 0;
            foreach (int item in inputVector)
            {
                result += item * item;
            }
            return Math.Sqrt(result);
        }

        private int[] subtractionOfVectors(int[] firstVector, int[] secondVector)
        {
            if (firstVector.Length != secondVector.Length)
            {
                throw new RankException("Vectors have different length");
            }

            int[] result = new int[firstVector.Length];
            for (int i = 0; i < firstVector.Length; i++)
            {
                result[i] = firstVector[i] - secondVector[i];
            }
            return result;
        }

        private void lockImages()
        {
            sourceImage.LockBits();
            resultImage.LockBits();
        }

        private void freeImages()
        {
            sourceImage.UnlockBits();
            resultImage.UnlockBits();
        }
    }
}
