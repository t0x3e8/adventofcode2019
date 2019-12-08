using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace _08a
{
    class Program
    {
        static void Main(string[] args)
        {
            int imgWidth = 25;
            int imgHeight = 6;
            var layers = ReadFile("input.txt", imgWidth, imgHeight);
            var workingLayer = FindLayerByLeastDigits(layers, '0');
            int ones = CountDigits(workingLayer, '1');
            int twos = CountDigits(workingLayer, '2');

            Console.WriteLine($"Checksum is: {ones * twos}");
        }

        private static HashSet<char[]> FindLayerByLeastDigits(List<HashSet<char[]>> layers, char searchDigit)
        {
            int max = 9999;
            HashSet<char[]> foundLayer = new HashSet<char[]>();
            foreach (var layer in layers)
            {
                int count = CountDigits(layer, searchDigit);
                if (count <= max)
                {
                    foundLayer = layer;
                    max = count;                }
            }

            return foundLayer;
        }

        private static int CountDigits(HashSet<char[]> layer, char searchDigit)
        {
            int count = layer.Select(row => row.Count(c => c == searchDigit)).Sum();
            return count;
        }

        static List<HashSet<char[]>> ReadFile(string fileName, int imgWidth, int imgHeight)
        {
            var stream = File.OpenRead(fileName);
            var sr = new System.IO.StreamReader(stream);

            List<HashSet<char[]>> layers = new List<HashSet<char[]>>();
            HashSet<char[]> image = new HashSet<char[]>();

            char[] buffer = new char[imgWidth];

            while (!sr.EndOfStream)
            {
                sr.ReadBlock(buffer, 0, imgWidth);
                image.Add(buffer.ToArray());

                if (image.Count == imgHeight)
                {
                    layers.Add(image);
                    image = new HashSet<char[]>();
                }
            }

            return layers;
        }
    }
}
