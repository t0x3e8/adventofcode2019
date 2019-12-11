using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;

namespace _11a
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
    // Output: 12077198

    enum Direction
    {
        Left,
        Right,
        Up,
        Down
    }

    class VM
    {
        private bool isSettingRead = false;
        private long? initialSignal;
        private long defaultSetting;
        public string ID { get; set; }
        private long referenceBase = 0;

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
            return inputData[address];
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
                    case (int)OpCodes.OP_ADD:
                        inputData = Operate(inputData, curPos, (v1, v2) =>
                        {
                            long addResult = v1 + v2;
                            Console.WriteLine($"{this.ID} ADD, {v1} + {v2} = {addResult}");
                            return addResult;
                        });
                        curPos += 4;
                        break;
                    case (int)OpCodes.OP_MUL:
                        inputData = Operate(inputData, curPos, (v1, v2) =>
                        {
                            long mulResult = v1 * v2;
                            Console.WriteLine($"{this.ID} MUL, {v1} * {v2} = {mulResult}");
                            return mulResult;
                        });
                        curPos += 4;
                        break;
                    case (int)OpCodes.OP_EQU:
                        inputData = Operate(inputData, curPos, (v1, v2) =>
                        {
                            int equResult = v1 == v2 ? 1 : 0;
                            Console.WriteLine($"{this.ID} EQU, {v1} == {v2} = {equResult}");
                            return equResult;
                        });
                        curPos += 4;
                        break;
                    case (int)OpCodes.OP_JIT:
                        curPos = OperateCurrentPosition(inputData, curPos, (inVal, outVal) =>
                        {
                            return (inVal != 0) ? outVal : curPos += 3;
                        });

                        Console.WriteLine($"{this.ID} JIT {curPos}");
                        break;
                    case (int)OpCodes.OP_JIF:
                        curPos = OperateCurrentPosition(inputData, curPos, (inVal, outVal) =>
                        {
                            return (inVal == 0) ? outVal : curPos += 3;
                        });
                        Console.WriteLine($"{this.ID} JIF {curPos}");
                        break;
                    case (int)OpCodes.OP_LTH:
                        inputData = Operate(inputData, curPos, (v1, v2) =>
                        {
                            int lthResult = (v1 < v2) ? 1 : 0;

                            Console.WriteLine($"{this.ID} LTH {v1}<{v2}= {lthResult}");
                            return lthResult;
                        });
                        curPos += 4;
                        break;
                    case (int)OpCodes.OP_IN:
                        long parameter = 0;

                        if (!this.isSettingRead)
                        {
                            Console.WriteLine($"{this.ID} using settings: {this.defaultSetting}");
                            parameter = this.defaultSetting;
                            this.isSettingRead = true;
                        }
                        else if (this.initialSignal.HasValue)
                        {
                            Console.WriteLine($"{this.ID} using initial signal: {this.initialSignal.Value}");
                            parameter = this.initialSignal.Value;
                            this.initialSignal = null;
                        }
                        else
                        {
                            parameter = inputCallback.Invoke();
                            Console.WriteLine($"{this.ID} using PainingRobot input {parameter}");
                        }

                        inputData = Operate(inputData, curPos, () =>
                        {
                            Console.WriteLine($"{this.ID} INput {parameter} ");
                            return parameter;
                        });
                        curPos += 2;
                        break;
                    case (int)OpCodes.OP_OUT:
                        result = ReadValue(inputData, curPos, 1);
                        outputCallback.Invoke((int)result);
                        Console.WriteLine($"{this.ID} OUTPUT {result}");
                        curPos += 2;
                        break;
                    case (int)OpCodes.OP_RBS:
                        this.referenceBase += ReadValue(inputData, curPos, 1);
                        Console.WriteLine($"{this.ID} RBS new referenceBase {referenceBase}");
                        curPos += 2;
                        break;
                }

                if (curPos >= inputData.Count)
                    done = true;
            }

            return result;
        }
    }

    class PaintingRobot
    {
        private VM brain;
        private int xPosition = 0;
        private int yPosition = 0;
        private Direction cursorDirection = Direction.Up;
        private Dictionary<Point, int> points;
        private int? panelColor = null;

        public PaintingRobot()
        {
            this.brain = new VM(null, 0, "Brain");
        }

        private void Move(Direction newDirection)
        {
            if (this.cursorDirection == Direction.Up)
            {
                // y does not change
                xPosition += (newDirection == Direction.Left) ? -1 : 1;
                this.cursorDirection = newDirection;
            }
            else if (this.cursorDirection == Direction.Left)
            {
                // x does not change
                yPosition += (newDirection == Direction.Left) ? 1 : -1;
                this.cursorDirection = (newDirection == Direction.Left) ? Direction.Down : Direction.Up;
            }
            else if (this.cursorDirection == Direction.Down)
            {
                // y does not change
                xPosition += (newDirection == Direction.Left) ? 1 : -1;
                this.cursorDirection = newDirection;
            }
            else if (this.cursorDirection == Direction.Right)
            {
                // x does not change
                yPosition += (newDirection == Direction.Left) ? -1 : 1;
                this.cursorDirection = (newDirection == Direction.Left) ? Direction.Up : Direction.Down;
            }
        }

        public void Run()
        {
            this.points = new Dictionary<Point, int>();
            var inputData = ReadFile("input.txt");
            this.brain.RunCode(inputData, this.BrainInput, this.BrainOutput);
        }
        private int BrainInput()
        {
            int panelColor = this.points.Where(kv => kv.Key.Equals(new Point(this.xPosition, this.yPosition))).Select(kv => kv.Value).FirstOrDefault();
            return panelColor; 
        }

        private void BrainOutput(int output)
        {
          if(this.panelColor == null) {
            // paint panel
            this.panelColor = output;
            this.points.Add(new Point(this.xPosition, this.yPosition), this.panelColor.Value);
          } 
          else {
            // move robot
            this.Move(output == 0 ? Direction.Left : Direction.Right);
            this.panelColor = null;
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
            PaintingRobot robot = new PaintingRobot();
            robot.Run();
        }
    }
}
