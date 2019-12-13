using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;

namespace _13b
{
    enum OpCodes
    {
        OP_ADD = 1,
        OP_MUL = 2,
        OP_IN = 3,
        OP_OUT = 4,
        OP_JIT = 5,
        OP_JIF = 6,
        OP_LTH = 7,
        OP_EQU = 8,
        OP_RBS = 9
    }

    class VM
    {
        private bool isSettingRead = true;
        private long? initialSignal;
        private long defaultSetting;
        public string ID { get; set; }
        private long referenceBase = 0;

        public bool VerboseMode { get; set; }

        public VM(long? initialSignal, long defaultSetting, string ID)
        {
            this.initialSignal = initialSignal;
            this.defaultSetting = defaultSetting;
            this.ID = ID;
        }

        private int OperateCurrentPosition(List<long> inputData, int curPos, Func<long, long, long> testOperation)
        {
            var val1 = this.ReadValue(inputData, curPos, 1);
            var val2 = this.ReadValue(inputData, curPos, 2);

            return (int)testOperation(val1, val2);
        }

        private void PrintToConsole(string msg)
        {
            if (this.VerboseMode)
            {
                Console.WriteLine(msg);
            }
        }
        private List<long> Operate(List<long> inputData, int curPos, Func<long, long, long> mathOperation)
        {
            var val1 = this.ReadValue(inputData, curPos, 1);
            var val2 = this.ReadValue(inputData, curPos, 2);
            var outputAddress = ReadAddress(inputData, curPos, 3);

            if (inputData.Count <= outputAddress)
                inputData.AddRange(new long[outputAddress - inputData.Count + 1]);

            outputAddress = (outputAddress < 0) ? 0 : outputAddress;
            inputData[(int)outputAddress] = mathOperation(val1, val2);

            return inputData;
        }

        private List<long> Operate(List<long> inputData, int curPos, Func<long> mathOperation)
        {
            var opCode = ToArray((int)inputData[curPos]);
            var outputAddress = this.ReadAddress(inputData, curPos, 1);

            if (inputData.Count <= outputAddress)
                inputData.AddRange(new long[outputAddress - inputData.Count + 1]);

            inputData[(int)outputAddress] = mathOperation();

            return inputData;
        }

        private int ReadAddress(List<long> inputData, int curPos, int opArgumentPosition)
        {
            var opCode = ToArray((int)inputData[curPos]);
            int shifted = opArgumentPosition + 1;
            var address = curPos + opArgumentPosition;

            if (opCode.Length > shifted && opCode[shifted] == 1)
                return address;
            else if (opCode.Length > shifted && opCode[shifted] == 2)
                return (int)(referenceBase + inputData[address]);
            else
                return (inputData.Count <= address) ? 0 : (int)inputData[address];
        }

        private long ReadValue(List<long> inputData, int curPos, int opArgumentPosition)
        {
            int address = this.ReadAddress(inputData, curPos, opArgumentPosition);
            return (inputData.Count < address) ? 0 : inputData[address];
        }

        private static int[] ToArray(int number)
        {
            List<int> opCode = new List<int>();
            while (number > 0)
            {
                opCode.Add(number % 10);
                number = number / 10;
            }

            return opCode.ToArray();
        }

        public long RunCode(List<long> inputData, Func<int> inputCallback, Action<int> outputCallback)
        {
            int curPos = 0;
            bool done = false;
            long result = 0;

            this.PrintToConsole($"{this.ID} START, curPost: {curPos}");

            while (!done)
            {
                if (inputData[curPos] != 0 && (inputData[curPos] % 99) == 0)
                {
                    done = true;
                    this.PrintToConsole($"{this.ID} STOP");
                    break;
                }

                switch (inputData[curPos] % 10)
                {
                    case (int)OpCodes.OP_ADD:
                        inputData = Operate(inputData, curPos, (v1, v2) =>
                        {
                            long addResult = v1 + v2;
                            this.PrintToConsole($"{this.ID} ADD, {v1} + {v2} = {addResult}");
                            return addResult;
                        });
                        curPos += 4;
                        break;
                    case (int)OpCodes.OP_MUL:
                        inputData = Operate(inputData, curPos, (v1, v2) =>
                        {
                            long mulResult = v1 * v2;
                            this.PrintToConsole($"{this.ID} MUL, {v1} * {v2} = {mulResult}");
                            return mulResult;
                        });
                        curPos += 4;
                        break;
                    case (int)OpCodes.OP_EQU:
                        inputData = Operate(inputData, curPos, (v1, v2) =>
                        {
                            int equResult = v1 == v2 ? 1 : 0;
                            this.PrintToConsole($"{this.ID} EQU, {v1} == {v2} = {equResult}");
                            return equResult;
                        });
                        curPos += 4;
                        break;
                    case (int)OpCodes.OP_JIT:
                        curPos = OperateCurrentPosition(inputData, curPos, (inVal, outVal) =>
                        {
                            return (inVal != 0) ? outVal : curPos += 3;
                        });

                        this.PrintToConsole($"{this.ID} JIT {curPos}");
                        break;
                    case (int)OpCodes.OP_JIF:
                        curPos = OperateCurrentPosition(inputData, curPos, (inVal, outVal) =>
                        {
                            return (inVal == 0) ? outVal : curPos += 3;
                        });
                        this.PrintToConsole($"{this.ID} JIF {curPos}");
                        break;
                    case (int)OpCodes.OP_LTH:
                        inputData = Operate(inputData, curPos, (v1, v2) =>
                        {
                            int lthResult = (v1 < v2) ? 1 : 0;

                            this.PrintToConsole($"{this.ID} LTH {v1}<{v2}= {lthResult}");
                            return lthResult;
                        });
                        curPos += 4;
                        break;
                    case (int)OpCodes.OP_IN:
                        long parameter = 0;

                        if (!this.isSettingRead)
                        {
                            this.PrintToConsole($"{this.ID} using settings: {this.defaultSetting}");
                            parameter = this.defaultSetting;
                            this.isSettingRead = true;
                        }
                        else if (this.initialSignal.HasValue)
                        {
                            this.PrintToConsole($"{this.ID} using initial signal: {this.initialSignal.Value}");
                            parameter = this.initialSignal.Value;
                            this.initialSignal = null;
                        }
                        else
                        {
                            parameter = inputCallback.Invoke();
                            this.PrintToConsole($"{this.ID} using PainingRobot input {parameter}");
                        }

                        inputData = Operate(inputData, curPos, () =>
                        {
                            this.PrintToConsole($"{this.ID} INput {parameter} ");
                            return parameter;
                        });
                        curPos += 2;
                        break;
                    case (int)OpCodes.OP_OUT:
                        result = ReadValue(inputData, curPos, 1);
                        outputCallback.Invoke((int)result);
                        this.PrintToConsole($"{this.ID} OUTPUT {result}");
                        curPos += 2;
                        break;
                    case (int)OpCodes.OP_RBS:
                        this.referenceBase += ReadValue(inputData, curPos, 1);
                        this.PrintToConsole($"{this.ID} RBS new referenceBase {referenceBase}");
                        curPos += 2;
                        break;
                }

                if (curPos >= inputData.Count)
                    done = true;
            }

            return result;
        }
    }

    class Tile
    {
        public int X { get; set; }
        public int Y { get; set; }
        public int TileID { get; set; }
    }

    class Arcade
    {
        private List<int> outputs = new List<int>();
        public HashSet<Tile> Tiles { get; private set; }

        private VM vm;
        public Arcade()
        {
            this.vm = new VM(null, 0, "");
            this.Tiles = new System.Collections.Generic.HashSet<Tile>();
        }

        public void Run()
        {
            var inputData = ReadFile("input.txt");
            this.vm.RunCode(inputData, this.BrainInput, this.BrainOutput);
        }


        private int BrainInput()
        {
            var pad = this.Tiles.Where(t => t.TileID == 3).First();
            var ball = this.Tiles.Where(t => t.TileID == 4).FirstOrDefault();
            int joystick = (ball != null)? (pad.X < ball.X) ? 1 : (pad.X > ball.X) ? -1 : 0 : 0;
            Console.WriteLine($"Ball: {ball.X}:{ball.Y}, Pad: {pad.X}:{pad.Y}, Joystick moves: {joystick}");

            return joystick;
        }

        private void BrainOutput(int output)
        {
            this.outputs.Add(output);
            if (this.outputs.Count == 3)
            {
                if (outputs[0] == -1 && outputs[1] == 0) {
                    Console.WriteLine($"Score: {this.outputs[2]}");
                }
                else
                {
                    var tile = this.Tiles.Where(t => t.X == outputs[0] && t.Y == outputs[1]).FirstOrDefault();
                    if (tile == null)
                        this.Tiles.Add(new Tile() { X = this.outputs[0], Y = this.outputs[1], TileID = this.outputs[2] });
                    else
                    {
                        tile.X = outputs[0];
                        tile.Y = outputs[1];
                        tile.TileID = outputs[2];
                    }
                }

                this.outputs.Clear();
            }
        }


        private List<long> ReadFile(string fileName)
        {
            var stream = File.OpenRead(fileName);
            var sr = new System.IO.StreamReader(stream);
            var inputText = sr.ReadToEnd();
            var list = inputText.Split(',').Select(long.Parse).ToList<long>();

            return list;
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Arcade arcade = new Arcade();
            arcade.Run();
            var tilesOnScreen = arcade.Tiles.Where(t => t.TileID == 2).Count();
            Console.WriteLine($"Tiles on the screen: {tilesOnScreen}");
        }
    }
}