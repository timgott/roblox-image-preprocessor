using System;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;

namespace RobloxImagePreprocessor
{
    class Program
    {
        static void ProcessTransparentPixel(Bitmap image, int x, int y)
        {
            int sumR = 0;
            int sumG = 0;
            int sumB = 0;
            int count = 0;

            // Check Neighbors
            var bounds = new Rectangle(0, 0, image.Width, image.Height);

            for (int nx = -1; nx <= 1; nx++)
                for (int ny = -1; ny <= 1; ny++)
                {
                    if (nx == 0 && ny == 0)
                        continue;
                    if (!bounds.Contains(x + nx, y + ny))
                        continue;

                    Color color = image.GetPixel(x + nx, y + ny);
                    if (color.A > 0)
                    {
                        sumR += color.R;
                        sumG += color.G;
                        sumB += color.B;
                        count++;
                    }
                }



            if (count > 0)
            {
                Color color = Color.FromArgb(0, (byte)(sumR / count), (byte)(sumG / count), (byte)(sumB / count));
                image.SetPixel(x, y, color);
            }
                
        }

        static void Watermark(Stream stream)
        {
            var data = System.Text.Encoding.ASCII.GetBytes("Roblox Image Preprocessor");
            stream.Write(data, 0, data.Length);
        }

        static int Main(string[] args)
        {
            if (args.Length != 1)
            {
                Console.Error.WriteLine("Wrong number of arguments, 1 expected");
                return 1;
            }

            string path = args[0];
            Bitmap image;
            using (var stream = new FileStream(path, FileMode.Open, FileAccess.Read))
            {
                image = new Bitmap(stream);
            }

            for (int x = 0; x < image.Width; x++)
                for (int y = 0; y < image.Height; y++)
                {
                    Color color = image.GetPixel(x, y);
                    if (color.A == 0)
                    {
                        ProcessTransparentPixel(image, x, y);
                    }
                }

            using (var stream = new FileStream(path, FileMode.Truncate, FileAccess.Write))
            {
                image.Save(stream, ImageFormat.Png);
                Watermark(stream);
            }
            
            return 0;
        }
    }
}

