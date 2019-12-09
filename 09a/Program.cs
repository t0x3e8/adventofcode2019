using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace _09a
{
    class Program
    {
        static void Main(string[] args)
        {
            VM optComputer = new VM(1, 0, "A", null);
            var output = optComputer.RunCode(ReadFile("input.txt"), null);

            Console.WriteLine(output.Output);
        }

        static List<long> ReadFile(string fileName)
        {
            var stream = File.OpenRead(fileName);
            var sr = new System.IO.StreamReader(stream);
            var inputText = sr.ReadToEnd();
            var list = inputText.Split(',').Select(long.Parse).ToList<long>();

            return list;
        }
    }

    public class VMOutput
    {
        public long? Output { get; set; }
        public int Status { get; set; }
        public bool IsFinished { get { return this.Status == 1; } }
        public bool IsHalted { get { return this.Status == 3; } }
        public VMOutput(long? output, int status)
        {
            this.Output = output;
            this.Status = status;
        }
    }

    public class VM
    {
        public List<long> Outputs = new List<long>();
        public string InputAmplifierID { get; set; }
        const int OP_ADD = 1;
        const int OP_MUL = 2;
        const int OP_IN = 3;
        const int OP_OUT = 4;
        const int OP_JIT = 5;
        const int OP_JIF = 6;
        const int OP_LTH = 7;
        const int OP_EQU = 8;
        const int OP_RBS = 9;
        private int? initialSignal;
        private int defaultSetting;
        private bool isSettingReadFlag = false;
        public string ID { get; set; }
        private int preservedPositionOfExecution = 0;
        private bool preservedCompletnessOfExecution = false;
        private long preservedResultOfExecution = 0;
        private int preservedSingalsCounterOfExecution = 0;
        private List<long> preservedInputData;
        private static long referenceBase = 0;

        public VM(int? initialSignal, int defaultSetting, string ID, string inputAmplifierID)
        {
            this.initialSignal = initialSignal;
            this.defaultSetting = defaultSetting;
            this.ID = ID;
            this.InputAmplifierID = inputAmplifierID;
        }

        public VMOutput RunCode(List<long> inputData, List<long> ioSignals)
        {
            int curPos = this.preservedPositionOfExecution;
            int signalsCounter = this.preservedSingalsCounterOfExecution;
            bool done = this.preservedCompletnessOfExecution;
            long result = this.preservedResultOfExecution;
            if (this.preservedInputData != null)
            {
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
                        curPos = (int)OperateCurrentPosition(inputData, curPos, (inVal, outVal) =>
                        {
                            return (inVal != 0) ? outVal : curPos += 3;
                        });
                        break;
                    case OP_JIF:
                        curPos = (int)OperateCurrentPosition(inputData, curPos, (inVal, outVal) =>
                        {
                            return (inVal == 0) ? outVal : curPos += 3;
                        });
                        break;
                    case OP_LTH:
                        inputData = Operate(inputData, curPos, (v1, v2) => { return (v1 < v2) ? 1 : 0; });
                        curPos += 4;
                        break;
                    case OP_IN:
                        long parameter = 0;

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
                            this.preservedInputData = new List<long>(inputData);

                            return new VMOutput(null, 3);
                        }

                        inputData[(int)inputData[curPos + 1]] = parameter;
                        curPos += 2;
                        break;
                    case OP_OUT:
                        result = ReadValue(inputData, curPos, 1);
                        this.Outputs.Add(result);
                        Console.WriteLine($"{this.ID} OUTPUT {result}");
                        curPos += 2;
                        break;
                    case OP_RBS:
                        referenceBase = ReadValue(inputData, curPos, 1);
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

        static long OperateCurrentPosition(List<long> inputData, int curPos, Func<long, long, long> test)
        {
            var opCode = ToArray(inputData[curPos]);

            var val1 = ReadValue(inputData, curPos, 1);
            var val2 = ReadValue(inputData, curPos, 2);

            return test(val1, val2);
        }

        static List<long> Operate(List<long> inputData, int curPos, Func<long, long, long> test)
        {
            var opCode = ToArray(inputData[curPos]);
            var val1 = ReadValue(inputData, curPos, 1);
            var val2 = ReadValue(inputData, curPos, 2);
            var outputAddress = inputData[curPos + 3];
            if (inputData.Count <= outputAddress)
                for (int i = inputData.Count; i <= outputAddress; i++)
                    inputData.Add(0);

            inputData[(int)outputAddress] = test(val1, val2);

            return inputData;
        }

        static long ReadValue(List<long> inputData, int curPos, int paramPosition)
        {
            var opCode = ToArray(inputData[curPos]);
            long val = 0;
            var address = inputData[curPos + paramPosition];
            int shifted = paramPosition + 1;

            if (opCode.Length >= shifted && opCode[shifted] == 1)
                val = inputData[curPos + paramPosition];
            else if (opCode.Length >= shifted && opCode[shifted] == 2)
                val = inputData[(int)referenceBase];
            else
            {
                if (inputData.Count < address)
                    val = 0;
                else
                    val = inputData[(int)address];
            }

            return val;
        }

        static int[] ToArray(long number)
        {
            List<int> opCode = new List<int>();
            while (number > 0)
            {
                opCode.Add((int)number % 10);
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
