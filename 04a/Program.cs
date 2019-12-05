using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace _04a
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
                var isAlwaysIncrease = IsNumberIncreasing(i);
                if(hasPair && isAlwaysIncrease) 
                    counter++;
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
            bool result = false;
            var numberText = number.ToString();
            for (int i = 0; i <= 9; i++)
            {
                result = numberText.Contains(string.Concat(i.ToString(), i.ToString()));
                if (result)
                    break;
            }

            return result;
        }
    }
}
