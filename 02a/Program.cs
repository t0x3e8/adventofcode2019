using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace _01b
{
    class Program
    {
        const int ADD = 1;
        const int MUL = 2;
        const int STP = 99;

        static void Main(string[] args)
        {
            var inputData = ReadFile("input.txt");

            inputData = ProcessData(inputData);

            int result = inputData[0];

            Console.WriteLine(result);
        }

        private static List<int> ProcessData(List<int> inputData)
        {
            int curPos = 0;
            bool done = false;

            while (!done)
            {
                switch (inputData[curPos])
                {
                    case ADD:
                        inputData = Operate(inputData, curPos, ADD);
                        curPos += 4;
                        break;
                    case MUL:
                        inputData = Operate(inputData, curPos, MUL);
                        curPos += 4;
                        break;
                        case STP: 
                    done = true;
                        break;
                    default:
                        curPos += 1;
                        break;
                }

                if (curPos >= inputData.Count)
                    done = true;
            }

            return inputData;
        }

        static List<int> Operate(List<int> inputData, int curPos, int oper)
        {
            int firstValuePos = inputData[curPos + 1];
            int secondValuePos = inputData[curPos + 2];
            int resultValueOutputPos = inputData[curPos + 3];
            if (oper == ADD)
                inputData[resultValueOutputPos] = inputData[firstValuePos] + inputData[secondValuePos];
            else if (oper == MUL)
                inputData[resultValueOutputPos] = inputData[firstValuePos] * inputData[secondValuePos];
            return inputData;
        }
        static List<int> ReadFile(string fileName)
        {
            var stream = File.OpenRead(fileName);
            var sr = new System.IO.StreamReader(stream);
            var inputText = sr.ReadToEnd();
            var list = inputText.Split(',').Select(int.Parse).ToList<int>();

            return list;
        }
    }
}
