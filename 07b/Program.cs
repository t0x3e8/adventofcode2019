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

            foreach (var inputParameter in inputParameters)
            {
                var programOutput = RunProgram(ReadFile("input.txt"), inputParameter);
                results.Add(programOutput.Item1);
            }


            Console.WriteLine($"The total MAX is: {results.Max()}");
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
            Dictionary<string, AmplifierVM> amps = new Dictionary<string, AmplifierVM>();
            amps.Add("A", new AmplifierVM(0, parameters[0], "A", "E"));
            amps.Add("B", new AmplifierVM(null, parameters[1], "B", "A"));
            amps.Add("C", new AmplifierVM(null, parameters[2], "C", "B"));
            amps.Add("D", new AmplifierVM(null, parameters[3], "D", "C"));
            amps.Add("E", new AmplifierVM(null, parameters[4], "E", "D"));
            IEnumerator<KeyValuePair<string, AmplifierVM>> ampsEnumerator = amps.GetEnumerator();

            while (true)
            {
                bool okNext = ampsEnumerator.MoveNext();
                if (!okNext) {
                    ampsEnumerator.Reset();
                    ampsEnumerator.MoveNext();
                }
                var ampVM = ampsEnumerator.Current.Value;
                var amplifierOutput = ampVM.RunCode(new List<int>(inputData), amps[ampVM.InputAmplifierID].Outputs);
                if (ampVM.ID == "E" && amplifierOutput.IsFinished)
                {
                    totalOutput += amplifierOutput.Output.Value;
                    lastOutput = amplifierOutput.Output.Value;
                    break;
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
        public List<int> Outputs = new List<int>();
        public string InputAmplifierID { get; set; }
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
        public string ID {get;set;}
        private int preservedPositionOfExecution = 0;
        private bool preservedCompletnessOfExecution = false;
        private int preservedResultOfExecution = 0;
        private int preservedSingalsCounterOfExecution = 0;
        private List<int> preservedInputData;
        public AmplifierVM(int? initialSignal, int defaultSetting, string ID, string inputAmplifierID)
        {
            this.initialSignal = initialSignal;
            this.defaultSetting = defaultSetting;
            this.ID = ID;
            this.InputAmplifierID = inputAmplifierID;
        }

        public VMOutput RunCode(List<int> inputData, List<int> ioSignals)
        {
            int curPos = this.preservedPositionOfExecution;
            int signalsCounter = this.preservedSingalsCounterOfExecution;
            bool done = this.preservedCompletnessOfExecution;
            int result = this.preservedResultOfExecution;
            if (this.preservedInputData != null) {
                inputData = this.preservedInputData;
            }

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

                        if (!this.isSettingReadFlag)
                        {
                            Console.WriteLine($"{this.ID} using settings: {this.defaultSetting}");
                            parameter = this.defaultSetting;
                            this.isSettingReadFlag = true;
                        }
                        else if (this.initialSignal.HasValue)
                        {
                            Console.WriteLine($"{this.ID} using initial signal: {this.initialSignal.Value}");
                            parameter = this.initialSignal.Value;
                            this.initialSignal = null;
                        }
                        else if (signalsCounter < ioSignals.Count)
                        {
                            parameter = ioSignals[signalsCounter];
                            Console.WriteLine($"{this.ID} using previous amplifier signal: {parameter}");
                            signalsCounter++;
                        }
                        else
                        {
                            Console.WriteLine($"{this.ID} HALT, cutPos: {curPos}");
                            this.preservedCompletnessOfExecution = done;
                            this.preservedPositionOfExecution = curPos;
                            this.preservedResultOfExecution = result;
                            this.preservedSingalsCounterOfExecution = signalsCounter;
                            this.preservedInputData = new List<int>(inputData);

                            return new VMOutput(null, 3);
                        }

                        inputData[inputData[curPos + 1]] = parameter;
                        curPos += 2;
                        break;
                    case OP_OUT:
                        result = inputData[inputData[curPos + 1]];
                        this.Outputs.Add(result);
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
            return this.ID;
        }
    }
}
