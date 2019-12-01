using System;
using System.IO;

namespace _01b
{
    class Program
    {

        static void Main(string[] args)
        {            
            string text = ReadFile("input.txt");

            int sum = 0;
            foreach(string line in text.Split('\n')) {
                var resultOfCount = Count(int.Parse(line));
                Console.WriteLine(line + " : " +  resultOfCount);
                sum += resultOfCount;
            }

            Console.WriteLine(sum);
        }

        static int Count(int input) {
            //int input = 100756;//3382136;
            int sumOfFuelRequirements = 0;

            while (true)
            {
                input = CountFuelperModule(input);
                if (input <= 0)
                {
                    break;
                }

                sumOfFuelRequirements += input;
            };

            return sumOfFuelRequirements;
        }

        static int CountFuelperModule(int input)
        {
            int result = input / 3 - 2;
            return result;
        }

        static string ReadFile(string fileName)
        {
            var stream = File.OpenRead(fileName);
            var sr = new System.IO.StreamReader(stream);
            var inputText = sr.ReadToEnd();

            return inputText;
        }
    }
}
