using System;
using System.Collections.Generic;
using System.IO;

namespace _06b
{
    class Program
    {
        class UniversalObject
        {
            public string Name { get; set; }
            public IList<UniversalObject> ObjectsInOrbits { get; set; }
            public UniversalObject(string name)
            {
                this.Name = name;
                this.ObjectsInOrbits = new List<UniversalObject>();
            }

            public int Count { get; set; }

            public static UniversalObject Create(List<string> inputLines)
            {
                UniversalObject universalObject = CreateRecur(inputLines, "COM", 0);
                return universalObject;
            }

            private static UniversalObject CreateRecur(List<string> inputLines, string objectName, int count)
            {
                UniversalObject universalObject = new UniversalObject(objectName);
                universalObject.Count = count++;
                foreach (var objectsInOrbitLines in inputLines.FindAll(s => s.StartsWith(universalObject.Name + ")")))
                {
                    var words = Parse(objectsInOrbitLines);
                    universalObject.ObjectsInOrbits.Add(CreateRecur(inputLines, words.Item2, count));
                }

                return universalObject;
            }

            private static Tuple<string, string> Parse(string line)
            {
                var words = line.Split(')');
                return new Tuple<string, string>(words[0], words[1]);
            }

            public int CountOrbits()
            {
                int count = this.Count;
                foreach (var objectInOrbit in this.ObjectsInOrbits)
                {
                    count += objectInOrbit.CountOrbits();
                }

                return count;
            }

            public HashSet<UniversalObject> Find(string objectName)
            {
                HashSet<UniversalObject> objectsOnRoute = new HashSet<UniversalObject>();
                if (this.Name == objectName) { 
                    objectsOnRoute.Add(this);
                    return objectsOnRoute;
                }

                foreach (var objectInOrbit in this.ObjectsInOrbits)
                {
                    objectsOnRoute = objectInOrbit.Find(objectName);
                    if (objectsOnRoute != null)
                    {
                        objectsOnRoute.Add(this);
                        return objectsOnRoute;
                    }
                }

                return null;
            }
        }

        static void Main(string[] args)
        {
            var linesInput = ReadFile("input.txt");
            UniversalObject universalObject = UniversalObject.Create(linesInput);
            var youOrbit = universalObject.Find("YOU");
            var sanOrbit = universalObject.Find("SAN");
            youOrbit.SymmetricExceptWith(sanOrbit);

            
            int result = youOrbit.Count - 2;

            Console.WriteLine(result);
        }

        static List<string> ReadFile(string fileName)
        {
            var stream = File.OpenRead(fileName);
            var sr = new System.IO.StreamReader(stream);
            List<string> lines = new List<string>();
            while (!sr.EndOfStream)
            {
                lines.Add(sr.ReadLine());
            }

            return lines;
        }
    }
}
