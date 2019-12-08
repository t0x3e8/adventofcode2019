using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace _08b
{
    class Program
    {
        static void Main(string[] args)
        {
            int imgWidth = 25;
            int imgHeight = 6;
            var images = ReadFile("input.txt", imgWidth, imgHeight);
            var workingImage = CreateImage(images, imgWidth, imgHeight);
            Print(workingImage, imgWidth, imgHeight);

            Console.WriteLine($"");
        }

        private static char[] CreateImage(List<char[]> images, int w, int h)
        {
            char[] image = new char[w * h];

            for (int y = 0; y < h; y++)
            {
                for (int x = 0; x < w; x++)
                {
                    int loc = x + (y * h);
                    var allPixels = images.Select(img => img[loc]).ToArray();
                    image[loc] = DeterminePixelColor(allPixels);
                }
            }

            return image;
        }

        private static void Print(char[] image, int w, int h)
        {
            for (int y = 0; y < h; y++)
            {
                for (int x = 0; x < w; x++)
                    Console.Write(image[x + (y * h)]);

                Console.WriteLine();
            }
        }
        private static char DeterminePixelColor(char[] pixels)
        {
            for (int i = 0; i < pixels.Length; i++)
                if (pixels[i] != '2')
                    return pixels[i] == '0' ? '\u2617' : '\u2616';

            return 'E';
        }

        static List<char[]> ReadFile(string fileName, int w, int h)
        {
            var stream = File.OpenRead(fileName);
            var sr = new System.IO.StreamReader(stream);

            List<char[]> images = new List<char[]>();
            int size = w * h;
            char[] image = new char[size];

            while (!sr.EndOfStream)
            {
                sr.ReadBlock(image, 0, size);

                images.Add(image.ToArray());
                image = new char[size];
            }

            return images;
        }
    }
}
