using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace _02b
{
    class Program
    {
        const int ADD = 1;
        const int MUL = 2;
        const int STP = 99;

        static void Main(string[] args)
        {
            var inputData = ReadFile("input.txt");
            var copyOfInputData = inputData.ToList<int>();
            int result = 0;

            for (int i = 0; i < 100; i++)
            {
                for (int j = 0; j < 100; j++)
                {
                    // reset live
                    inputData = copyOfInputData.ToList();
                    inputData[1] = i;
                    inputData[2] = j;

                    inputData = ProcessData(inputData);

                    if (inputData[0] == 19690720)
                    {
                        result = 100 * i + j;
                        break;
                    }
                }
            }


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
                        inputData = Operate(inputData, curPos);
                        curPos += 4;
                        break;
                    case MUL:
                        inputData = Operate(inputData, curPos);
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

        static List<int> Operate(List<int> inputData, int curPos)
        {
            int firstValuePos = inputData[curPos + 1];
            int secondValuePos = inputData[curPos + 2];
            int resultValueOutputPos = inputData[curPos + 3];
            int result = 0;
            if (inputData[curPos] == ADD)
                result = inputData[firstValuePos] + inputData[secondValuePos];
            else if (inputData[curPos] == MUL)
                result = inputData[firstValuePos] * inputData[secondValuePos];

            inputData[resultValueOutputPos] = result;
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
