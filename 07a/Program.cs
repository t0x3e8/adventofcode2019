using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace _07a
{
    class Program
    {
        const int OP_ADD = 1;
        const int OP_MUL = 2;
        const int OP_IN = 3;
        const int OP_OUT = 4;
        const int OP_JIT = 5;
        const int OP_JIF = 6;
        const int OP_LTH = 7;
        const int OP_EQU = 8;

        static void Main(string[] args)
        {
            HashSet<int> results = new HashSet<int>();
            var inputParameters = GetPermutations(new int[] { 0, 1, 2, 3, 4 }, 5).ToArray();
            foreach (var inputParameter in inputParameters)
            {
                int result = RunProgram(ReadFile("input.txt"), inputParameter);
                Console.WriteLine(string.Concat(inputParameter) + ": " + result);
                results.Add(result);
            }


            Console.WriteLine("highest is: " + results.Max());
        }

static IEnumerable<IEnumerable<T>>
    GetPermutations<T>(IEnumerable<T> list, int length)
{
    if (length == 1) return list.Select(t => new T[] { t });
    return GetPermutations(list, length - 1)
        .SelectMany(t => list.Where(o => !t.Contains(o)),
            (t1, t2) => t1.Concat(new T[] { t2 }));
}

        private static int RunProgram(List<int> inputData, IEnumerable<int> inputParameters)
        {
            int output = 0;
            var parameters = inputParameters.ToArray();

            for (int i = 0; i < 5; i++)
            {
                output = RunCode(inputData, parameters[i], output);
            }

            return output;
        }

        private static int RunCode(List<int> inputData, int inputParam1, int inputParam2)
        {
            int curPos = 0;
            bool done = false;
            int result = 0;
            while (!done)
            {
                if (inputData[curPos] != 0 && (inputData[curPos] % 99) == 0)
                {
                    done = true;
                    break;
                }

                switch (inputData[curPos] % 10)
                {
                    case OP_ADD:
                        inputData = Operate(inputData, curPos, (v1, v2) => { return v1 + v2; });
                        curPos += 4;
                        break;
                    case OP_MUL:
                        inputData = Operate(inputData, curPos, (v1, v2) => { return v1 * v2; });
                        curPos += 4;
                        break;
                    case OP_EQU:
                        inputData = Operate(inputData, curPos, (v1, v2) => { return (v1 == v2) ? 1 : 0; });
                        curPos += 4;
                        break;
                    case OP_JIT:
                        curPos = OperateCurrentPosition(inputData, curPos, (inVal, outVal) =>
                        {
                            return (inVal != 0) ? outVal : curPos += 3;
                        });
                        break;
                    case OP_JIF:
                        curPos = OperateCurrentPosition(inputData, curPos, (inVal, outVal) =>
                        {
                            return (inVal == 0) ? outVal : curPos += 3;
                        });
                        break;
                    case OP_LTH:
                        inputData = Operate(inputData, curPos, (v1, v2) => { return (v1 < v2) ? 1 : 0; });
                        curPos += 4;
                        break;
                    case OP_IN:
                        // Console.WriteLine("Provide input number: ");
                        if (inputParam1 == -1)
                            inputParam1 = inputParam2;
                        inputData[inputData[curPos + 1]] = inputParam1;
                        inputParam1 = -1;
                        curPos += 2;
                        break;
                    case OP_OUT:
                        // Console.WriteLine("Output: " + inputData[inputData[curPos + 1]]);
                        result = inputData[inputData[curPos + 1]];
                        curPos += 2;
                        break;
                    default:
                        curPos += 1;
                        break;
                }

                if (curPos >= inputData.Count)
                    done = true;
            }

            return result;
        }

        static int OperateCurrentPosition(List<int> inputData, int curPos, Func<int, int, int> test)
        {
            var opCode = ToArray(inputData[curPos]);

            var val2 = 0;
            var address2 = inputData[curPos + 2];
            var val1 = 0;
            var address1 = inputData[curPos + 1];

            if (opCode.Length >= 3 && opCode[2] == 1)
                val1 = inputData[curPos + 1];
            else
                val1 = inputData[address1];
            if (opCode.Length >= 4 && opCode[3] == 1)
                val2 = inputData[curPos + 2];
            else
                val2 = inputData[address2];

            return test(val1, val2);
        }

        static List<int> Operate(List<int> inputData, int curPos, Func<int, int, int> func)
        {
            var opCode = ToArray(inputData[curPos]);
            var val1 = 0;
            var address1 = inputData[curPos + 1];

            var val2 = 0;
            var address2 = inputData[curPos + 2];
            var outputAddress = inputData[curPos + 3];

            if (opCode.Length >= 3 && opCode[2] == 1)
                val1 = inputData[curPos + 1];
            else
                val1 = inputData[address1];

            if (opCode.Length >= 4 && opCode[3] == 1)
                val2 = inputData[curPos + 2];
            else
                val2 = inputData[address2];
            inputData[outputAddress] = func(val1, val2);

            return inputData;
        }

        static int[] ToArray(int number)
        {
            List<int> opCode = new List<int>();
            while (number > 0)
            {
                opCode.Add(number % 10);
                number = number / 10;
            }

            return opCode.ToArray();
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
