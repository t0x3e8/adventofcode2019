﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace _03b
{
    class Program
    {
        struct WirePoint
        {
            public int X;
            public int Y;
            public int Value;

            public int Count;
            public string ID { get { return this.X + ":" + this.Y; } }
        }
        static void Main(string[] args)
        {
            var inputData = ReadFile("input.txt");
            HashSet<WirePoint> points = new HashSet<WirePoint>();
            MarkPoints(inputData.Item1, points, 1);
            MarkPoints(inputData.Item2, points, 2);

            int result = CalculateNearestInterceptions(points);

            Console.WriteLine(result);
        }

        private static void MarkPoints(List<string> wireSet, HashSet<WirePoint> points, int id)
        {
            int currentX = 0;
            int currentY = 0;
            int count = 0;
            foreach (string wire in wireSet)
            {
                int vector = int.Parse(wire.Substring(1));
                switch (wire[0])
                {
                    case 'U':
                        for (int i = currentY - 1; i >= currentY - vector; i--)
                        {
                            points.Add(new WirePoint() { X = currentX, Y = i, Value = id, Count = count });
                            count++;
                        }
                        currentY -= vector;
                        break;
                    case 'D':
                        for (int i = currentY + 1; i <= currentY + vector; i++)
                        {
                            points.Add(new WirePoint() { X = currentX, Y = i, Value = id, Count = count });
                            count++;
                        }
                        currentY += vector;
                        break;
                    case 'L':
                        for (int i = currentX - 1; i >= currentX - vector; i--)
                        {
                            points.Add(new WirePoint() { X = i, Y = currentY, Value = id, Count = count });
                            count++;
                        }
                        currentX -= vector;
                        break;
                    case 'R':
                        for (int i = currentX + 1; i <= currentX + vector; i++)
                        {
                            points.Add(new WirePoint() { X = i, Y = currentY, Value = id, Count = count });
                            count++;
                        }
                        currentX += vector;
                        break;
                }
            }
        }

        private static int CalculateNearestInterceptions(HashSet<WirePoint> points)
        {
            var query = from x in points
                        group x by new { x.ID, x.X, x.Y } into groupped
                        where groupped.Count() > 1
                        select groupped.Key;

            List<int> results = new List<int>();
            foreach (var overlapping in query)
            {
                int taxiCap = Math.Abs(overlapping.X) + Math.Abs(overlapping.Y);
                if (taxiCap != 0)
                    results.Add(taxiCap);
            }

            int result = results.Min();

            return result;
        }

        static Tuple<List<string>, List<string>> ReadFile(string fileName)
        {
            var stream = File.OpenRead(fileName);
            var sr = new System.IO.StreamReader(stream);
            List<string> wireSet1 = sr.ReadLine().Split(',').Select(x => x.Trim()).ToList();
            List<string> wireSet2 = sr.ReadLine().Split(',').Select(x => x.Trim()).ToList();

            return new Tuple<List<string>, List<string>>(wireSet1, wireSet2);
        }
    }
}