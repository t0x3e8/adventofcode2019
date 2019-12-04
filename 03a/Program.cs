using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace _03a
{
    class Program
    {

        class GridStructure
        {
            public int[,] Grid;
            public int CurLeft;
            public int CurUp;
            public int OrgLeft;
            public int OrgUp;
            Action<int, int, int, int, GridStructure, int> marker = (u, d, l, r, grid, id) =>
            {
                int x = grid.CurLeft - l;
                int endX = grid.CurLeft + r;
                int y = grid.CurUp - u;
                int endY = grid.CurUp + d;
                int newCurLeft = grid.CurLeft - l + r;
                int newCurUp = grid.CurUp - u + d;


                for (; y <= endY; y++)
                {
                    for (; x <= endX; x++)
                    {
                        if (grid.Grid[y, x] == 9 || grid.Grid[y, x] == id)
                            ;
                        else if (grid.Grid[y, x] == 0)
                            grid.Grid[y, x] = id;
                        else
                            grid.Grid[y, x] = 5;
                    }
                    if (u > 0 || d > 0)
                        x--;
                }

                grid.CurLeft = newCurLeft;
                grid.CurUp = newCurUp;
            };

            public void CreateGrid(List<string> wireSet1, List<string> wireSet2)
            {
                int maxUp = 0;
                int maxDown = 0;
                int maxLeft = 0;
                int maxRight = 0;

                Func<List<string>, string, int>  maxValue= (wireSet, dir) => {
                    var items = wireSet.FindAll(i => i.StartsWith(dir)).Select(i => int.Parse(i.Substring(1)));
                    return items.Sum();
                };

                maxUp = Math.Max(maxValue(wireSet1, "U"), maxValue(wireSet2, "U"));
                maxDown = Math.Max(maxValue(wireSet1, "D"), maxValue(wireSet2, "D"));
                maxLeft = Math.Max(maxValue(wireSet1, "L"), maxValue(wireSet2, "L"));
                maxRight = Math.Max(maxValue(wireSet1, "R"), maxValue(wireSet2, "R"));

                this.Grid = new int[maxUp + maxDown + 1, maxLeft + maxRight + 1];
                this.CurLeft = this.OrgLeft = maxLeft;
                this.CurUp = this.OrgUp = maxUp;

                this.Grid[this.CurUp, this.CurLeft] = 9;
            }

            public void Move(string direction, int id)
            {
                int vector = int.Parse(direction.Substring(1));
                switch (direction[0])
                {
                    case 'U':
                        marker.Invoke(vector, 0, 0, 0, this, id);
                        break;
                    case 'D':
                        marker.Invoke(0, vector, 0, 0, this, id);
                        break;
                    case 'L':
                        marker.Invoke(0, 0, vector, 0, this, id);
                        break;
                    case 'R':
                        marker.Invoke(0, 0, 0, vector, this, id);
                        break;
                }
            }

            public void Print()
            {
                Console.WriteLine("-----------------------------------------------------------------------------------");
                for (int i = 0; i < this.Grid.GetLength(0); i++)
                {
                    for (int j = 0; j < this.Grid.GetLength(1); j++)
                    {
                        Console.Write(this.Grid[i, j] + "\t");
                    }
                    Console.WriteLine();
                }
            }
        }
        static void Main(string[] args)
        {
            var inputData = ReadFile("input.txt");
            GridStructure grid = new GridStructure();
            grid.CreateGrid(inputData.Item1, inputData.Item2);
            MarkWire(grid, inputData.Item1, 1);
            MarkWire(grid, inputData.Item2, 2);

            int result = CalculateNearestInterceptions(grid);

            Console.WriteLine(result);
        }



        private static void MarkWire(GridStructure grid, List<string> wireSet, int id)
        {
            grid.CurLeft = grid.OrgLeft;
            grid.CurUp = grid.OrgUp;

            foreach (string wireBlock in wireSet)
            {
                grid.Move(wireBlock, id);
            }
        }

        private static int CalculateNearestInterceptions(GridStructure grid)
        {
            List<int> results = new List<int>();

            for (int y = 0; y < grid.Grid.GetLength(0); y++)
            {
                for (int x = 0; x < grid.Grid.GetLength(1); x++)
                {
                    if (grid.Grid[y, x] == 5)
                    {
                        int taxiCapGeometric = Math.Abs(grid.OrgLeft - x) + Math.Abs(grid.OrgUp - y);
                        results.Add(taxiCapGeometric);
                    }
                }
            }

            return results.Min();
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
