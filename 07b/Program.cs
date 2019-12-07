using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace _07b
{
    class Program
    {
        static void Main(string[] args)
        {
            HashSet<int> results = new HashSet<int>();
            var inputParameters = GetPermutations(new int[] { 5, 6, 7, 8, 9 }, 5).ToArray();
            Tuple<int, int> programOutput;
            var inputParameter = new int[] { 1, 0, 4, 3, 2 };

            // foreach (var inputParameter in inputParameters)
            {
                programOutput = RunProgram(ReadFile("input.txt"), inputParameter);
                // results.Add(programOutput);
            }


            // Console.WriteLine("highest is: " + results.Max());
            Console.WriteLine("The last is: " + programOutput.Item2);
        }

        static IEnumerable<IEnumerable<T>> GetPermutations<T>(IEnumerable<T> list, int length)
        {
            if (length == 1) return list.Select(t => new T[] { t });
            return GetPermutations(list, length - 1)
                .SelectMany(t => list.Where(o => !t.Contains(o)),
                    (t1, t2) => t1.Concat(new T[] { t2 }));
        }

        private static Tuple<int, int> RunProgram(List<int> inputData, IEnumerable<int> inputParameters)
        {
            int totalOutput = 0;
            int lastOutput = 0;
            var parameters = inputParameters.ToArray();
            Queue<AmplifierVM> amplifiers = new Queue<AmplifierVM>();
            amplifiers.Enqueue(new AmplifierVM(0, parameters[0], 0));
            amplifiers.Enqueue(new AmplifierVM(0, parameters[1], 1));
            amplifiers.Enqueue(new AmplifierVM(0, parameters[2], 2));
            amplifiers.Enqueue(new AmplifierVM(0, parameters[3], 3));
            amplifiers.Enqueue(new AmplifierVM(0, parameters[4], 4));
            Queue<int> outputs = new Queue<int>();

            while (amplifiers.Count() > 0)
            {
                var vm = amplifiers.Dequeue();
                int? tempInput = (outputs.Count() > 0) ? outputs.Dequeue() as int? : 0;
                var amplifierOutput = vm.RunCode(inputData, tempInput, (output) =>
                {
                    outputs.Enqueue(output);
                });
                if (amplifierOutput.IsHalted)
                {
                    amplifiers.Enqueue(vm);
                }
                else if (amplifierOutput.IsFinished)
                {
                    totalOutput += amplifierOutput.Output.Value;
                    lastOutput = amplifierOutput.Output.Value;
                }
            }

            return new Tuple<int, int>(totalOutput, lastOutput);
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

    public class VMOutput
    {
        public int? Output { get; set; }
        public int Status { get; set; }
        public bool IsFinished { get { return this.Status == 1; } }
        public bool IsHalted { get { return this.Status == 3; } }
        public VMOutput(int? output, int status)
        {
            this.Output = output;
            this.Status = status;
        }
    }

    public class AmplifierVM
    {
        const int OP_ADD = 1;
        const int OP_MUL = 2;
        const int OP_IN = 3;
        const int OP_OUT = 4;
        const int OP_JIT = 5;
        const int OP_JIF = 6;
        const int OP_LTH = 7;
        const int OP_EQU = 8;
        private int? initialSignal;
        private int defaultSetting;
        private bool isSettingReadFlag = false;
        private int ID;
        private int preservedPositionOfExecution = 0;
        private bool preservedCompletnessOfExecution = false;
        private int preservedResultOfExecution = 0;
        public AmplifierVM(int? initialSignal, int defaultSetting, int ID)
        {
            this.initialSignal = initialSignal;
            this.defaultSetting = defaultSetting;
            this.ID = ID;
        }

        public VMOutput RunCode(List<int> inputData, int? ioSignal, Action<int> outputCallback)
        {
            int curPos = this.preservedPositionOfExecution;
            bool done = this.preservedCompletnessOfExecution;
            int result = this.preservedResultOfExecution;

            Console.WriteLine($"{this.ID} START, curPost: {curPos}");

            while (!done)
            {
                if (inputData[curPos] != 0 && (inputData[curPos] % 99) == 0)
                {
                    done = true;
                    Console.WriteLine($"{this.ID} STOP");
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
                        int parameter = 0;

                        if (this.initialSignal.HasValue)
                        {
                            Console.WriteLine($"{this.ID} using initial signal: {this.initialSignal.Value}");
                            parameter = this.initialSignal.Value;
                            this.initialSignal = null;
                        }
                        else if (!this.isSettingReadFlag)
                        {
                            Console.WriteLine($"{this.ID} using settings: {this.defaultSetting}");
                            parameter = this.defaultSetting;
                            this.isSettingReadFlag = true;
                        }
                        else if (ioSignal.HasValue)
                        {
                            Console.WriteLine($"{this.ID} previous amplifier signal: {ioSignal}");
                            parameter = ioSignal.Value;
                            ioSignal = null;
                        }
                        else
                        {
                            Console.WriteLine($"{this.ID} HALT, cutPos: {curPos}");
                            this.preservedCompletnessOfExecution = done;
                            this.preservedPositionOfExecution = curPos;
                            this.preservedResultOfExecution = result;

                            return new VMOutput(null, 3);

                        }

                        inputData[inputData[curPos + 1]] = parameter;
                        curPos += 2;
                        break;
                    case OP_OUT:
                        result = inputData[inputData[curPos + 1]];
                        outputCallback.Invoke(result);
                        Console.WriteLine($"{this.ID} OUTPUT {result}");
                        curPos += 2;
                        break;
                    default:
                        curPos += 1;
                        break;
                }

                if (curPos >= inputData.Count)
                    done = true;
            }

            return new VMOutput(result, 1);
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

        public override string ToString()
        {
            return this.ID.ToString();
        }
    }
}
