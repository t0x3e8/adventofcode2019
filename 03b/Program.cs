using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace _03b
{
    class Program
    {
        class WirePoint : IEquatable<WirePoint>
        {
            public int X;
            public int Y;
            public int Value;
            public int Steps;

            public override bool Equals(object obj)
            {
                var item = obj as WirePoint;

                if (item == null)
                    return false;

                return this.X.Equals(item.X) && this.Y.Equals(item.Y);
            }

            public bool Equals(WirePoint other)
            {
                if (other == null)
                    return false;

                return this.X.Equals(other.X) && this.Y.Equals(other.Y);
            }

            public override int GetHashCode()
            {
                return this.X.GetHashCode() ^ this.Y.GetHashCode();
            }

            public override string ToString()
            {
                return string.Format("{0}:{1}", this.X, this.Y);
            }
        }
        static void Main(string[] args)
        {
            var inputData = ReadFile("input.txt");
            var points1 = MarkPoints(inputData.Item1, 1);
            var points2 = MarkPoints(inputData.Item2, 2);

            int result = CalculateNearestInterceptions(points1, points2);

            Console.WriteLine(result);
        }

        private static HashSet<WirePoint> MarkPoints(List<string> wireSet, int id)
        {
            int currentX = 0;
            int currentY = 0;
            int count = 1;
            HashSet<WirePoint> points = new HashSet<WirePoint>();

            foreach (string wire in wireSet)
            {
                int vector = int.Parse(wire.Substring(1));
                switch (wire[0])
                {
                    case 'U':
                        for (int i = currentY - 1; i >= currentY - vector; i--)
                        {
                            var w = new WirePoint() { X = currentX, Y = i, Value = id, Steps = count };
                            if (!points.Contains(w))
                            {
                                points.Add(w);
                            }
                            count++;
                        }
                        currentY -= vector;
                        break;
                    case 'D':
                        for (int i = currentY + 1; i <= currentY + vector; i++)
                        {
                            var w = new WirePoint() { X = currentX, Y = i, Value = id, Steps = count };
                            if (!points.Contains(w))
                            {
                                points.Add(w);
                            }
                            count++;
                        }
                        currentY += vector;
                        break;
                    case 'L':
                        for (int i = currentX - 1; i >= currentX - vector; i--)
                        {
                            var w = new WirePoint() { X = i, Y = currentY, Value = id, Steps = count };
                            if (!points.Contains(w))
                            {
                                points.Add(w);
                            }
                            count++;
                        }
                        currentX -= vector;
                        break;
                    case 'R':
                        for (int i = currentX + 1; i <= currentX + vector; i++)
                        {
                            var w = new WirePoint() { X = i, Y = currentY, Value = id, Steps = count };
                            if (!points.Contains(w))
                            {
                                points.Add(w);
                            }
                            count++;
                        }
                        currentX += vector;
                        break;
                }

            }
            return points;
        }

        private static int CalculateNearestInterceptions(HashSet<WirePoint> points1, HashSet<WirePoint> points2)
        {
            var list1 = points1.Intersect(points2).ToList();
            var list2 = points2.Intersect(points1).ToList();
            var zippedSet = new HashSet<Tuple<WirePoint, WirePoint>>();
            foreach (var item1 in list1)
            {
                var item2 = list2.Find(w => w.X == item1.X && w.Y == item1.Y);
                zippedSet.Add(new Tuple<WirePoint, WirePoint>(item1, item2));
            }

            List<int> sumOfSteps = new List<int>();
            List<int> distances = new List<int>();

            foreach (var zippedEntry in zippedSet)
            {
                int sum = zippedEntry.Item1.Steps + zippedEntry.Item2.Steps;
                distances.Add(Math.Abs(zippedEntry.Item1.X) + Math.Abs(zippedEntry.Item2.Y));
                sumOfSteps.Add(sum);
            }

            return sumOfSteps.Min();
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
