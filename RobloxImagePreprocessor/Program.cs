using System;
using System.IO;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Processing.Processors;

namespace RobloxImagePreprocessor
{
    class Program
    {
        static Rgba32 ProcessTransparentPixel(Image<Rgba32> image, int x, int y)
        {
            int sumR = 0;
            int sumG = 0;
            int sumB = 0;
            int count = 0;

            // Check Neighbors
            var bounds = image.Bounds();

            for (int nx = -1; nx <= 1; nx++)
                for (int ny = -1; ny <= 1; ny++)
                {
                    if (nx == 0 && ny == 0)
                        continue;
                    if (!bounds.Contains(x + nx, y + ny))
                        continue;

                    Rgba32 color = image[x + nx, y + ny];
                    if (color.A > 0)
                    {
                        sumR += color.R;
                        sumG += color.G;
                        sumB += color.B;
                        count++;
                    }
                }

            if (count > 0)
                return new Rgba32((byte)(sumR / count), (byte)(sumG / count), (byte)(sumB / count), 0);
            else
                return new Rgba32(0, 0, 0, 0);
        }

        static int Main(string[] args)
        {
            if (args.Length != 1)
            {
                Console.Error.WriteLine("Wrong number of arguments, 1 expected");
                return 1;
            }

            string path = args[0];
            using (var image = Image.Load<Rgba32>(path))
            {
                for (int x = 0; x < image.Width; x++)
                    for (int y = 0; y < image.Height; y++)
                    {
                        if (image[x, y].A == 0)
                        {
                            image[x, y] = ProcessTransparentPixel(image, x, y);
                        }
                    }

                image.Save(path);
            }
            
            return 0;
        }
    }
}
