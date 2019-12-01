using System;
using System.IO;

namespace _01a
{
    class Program
    {

        static void Main(string[] args)
        {
            string text = ReadFile("input.txt");
            
            int sum= 0;
            foreach(string line in text.Split('\n')) {
                var resultOfCount = CountFuelperModule(line);
                Console.WriteLine(line + " : " +  resultOfCount);
                sum += resultOfCount;
            }

            Console.WriteLine(sum);
        }

        static int CountFuelperModule(string value) {
            int input = int.Parse(value);

            int result = input / 3 - 2;
            return result; 
        }

        static string ReadFile(string fileName) {
            var stream = File.OpenRead(fileName);
            var sr = new System.IO.StreamReader(stream);
            var inputText = sr.ReadToEnd();

            return inputText;
        }
    }
}
