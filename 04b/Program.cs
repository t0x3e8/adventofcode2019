using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace _04b
{
    class Program
    {
        static void Main(string[] args)
        {
            var inputFrom = 193651;
            var inputTo = 649729;
            var counter = 0;
            for (int i = inputFrom; i < inputTo; i++)
            {
                var hasPair = HasNumberPair(i);
                if (hasPair)
                {
                    var isAlwaysIncrease = IsNumberIncreasing(i);
                    if (isAlwaysIncrease)
                        counter++;
                }
            }

            Console.WriteLine(counter);
        }

        static bool IsNumberIncreasing(int number)
        {
            bool result = false;
            var numberText = number.ToString();
            for (int i = 1; i < 6; i++)
            {
                int prev = (int)Char.GetNumericValue(numberText[i - 1]);
                int curr = (int)Char.GetNumericValue(numberText[i]);
                result = curr >= prev;
                if (!result)
                    break;
            }

            return result;
        }
        static bool HasNumberPair(int number)
        {
            var numberText = number.ToString();
            bool[] results = new bool[10];
            for (int i = 0; i <= 9; i++)
            {
                results[i] = numberText.Contains(string.Concat(i.ToString(), i.ToString()));
                if (results[i])
                {
                    bool result3 = numberText.Contains(string.Concat(i.ToString(), i.ToString(), i.ToString()));
                    bool result4 = numberText.Contains(string.Concat(i.ToString(), i.ToString(), i.ToString(), i.ToString()));
                    bool result5 = numberText.Contains(string.Concat(i.ToString(), i.ToString(), i.ToString(), i.ToString(), i.ToString()));
                    bool result6 = numberText.Contains(string.Concat(i.ToString(), i.ToString(), i.ToString(), i.ToString(), i.ToString(), i.ToString()));
                    results[i] = !(result3 || result4 || result5 || result6);
                }
            }

            bool result = results.Any((element) => element == true);

            return result;
        }
    }
}
