using System;
using System.Collections.Generic;
using System.Linq;

namespace NPIValidator
{
    class Program
    {
        static void Main(string[] args)
        {
            string[] npis = new string[]
            {
                "123456789", "1234567893", "1234567895", "1669643912", "166964391", "1669643915", "165987224", "1659872240", "1659872245",
                "1234", "12345678", "ASDFASDF",
                "123412h45"
            };

            foreach (string npi in npis)
            {
                Console.WriteLine($"npi '{npi}' with checkdigit '{npi.GetNPICheckDigit()}' is valid: {npi.IsValidNPI()}");
            }

            //const string npi9 = "123456789";
            ////step1: double alternate digits, starting from the rightmost digit
            //int[] doubledAlternates = npi9.ToIntArray().Reverse().Where((digit, index) => index % 2 == 0).Select(n => n*2).ToArray();

            ////step2: add constant 24, individual digits of all tokens
            //const int prefixValue = 24;
            //int[] unaffectedDigits = npi9.ToIntArray().Where((digit, index) => index % 2 == 1).ToArray();
            //int[] individualDigits = unaffectedDigits.Concat(doubledAlternates.ToIntArraySplitNumbers()).ToArray();
            //int sumOfDigits = prefixValue + individualDigits.Sum();

            ////step3: subtract from next higher number ending in zero to get check digit
            //int nextHigherNumberEndingWithZero = sumOfDigits.GetNextMultipleOfTen();
            //int checkDigit = nextHigherNumberEndingWithZero - sumOfDigits;
            //string npi = npi9 + checkDigit.ToString();

            ////print out the results
            //Console.WriteLine($"npi9: {string.Join(" - ", npi9.ToCharArray())}");
            //doubledAlternates.Print(true, nameof(doubledAlternates));
            //unaffectedDigits.Print(true, nameof(unaffectedDigits));
            //individualDigits.Print(true, nameof(individualDigits));
            //Console.WriteLine($"sum: {sumOfDigits}");
            //Console.WriteLine($"nextHigherNumberEndingWithZero: {nextHigherNumberEndingWithZero}");
            //Console.WriteLine($"checkDigit: {checkDigit}");
            //Console.WriteLine($"npi: {npi}");

            Console.WriteLine("press the ENTER key to exit");
            Console.Read();
        }

    }

    public static class NPIExtensions
    {
        public static bool IsValidNPI(this string npi)
        {
            return npi.HasValidNPILength() && long.TryParse(npi, out long n) && npi.GetNPICheckDigit() == npi.CalculateNPICheckDigit();
        }

        public static bool HasValidNPILength(this string npi)
        {
            return npi.Length == 10 || npi.Length == 9;
        }

        public static string GetNPICheckDigit(this string npi)
        {
            return npi.Length == 10 ? npi[9..] : npi.Length == 9 ? npi.CalculateNPICheckDigit() : "";
        }

        public static string CalculateNPICheckDigit(this string npi)
        {
            if (npi == null || npi.Length == 0)
            {
                return "";
            }
            string npi9 = npi.Length == 10 ? npi[0..9] : npi.Length == 9 ? npi : "";

            //step1: double alternate digits, starting from the rightmost digit
            int[] doubledAlternates = npi9.ToIntArray().Reverse().Where((digit, index) => index % 2 == 0).Select(n => n * 2).ToArray();

            //step2: add constant 24, individual digits of all tokens
            const int prefixValue = 24;
            int[] unaffectedDigits = npi9.ToIntArray().Where((digit, index) => index % 2 == 1).ToArray();
            int[] individualDigits = unaffectedDigits.Concat(doubledAlternates.ToIntArraySplitDigits()).ToArray();
            int sumOfDigits = prefixValue + individualDigits.Sum();

            //step3: subtract from next higher number ending in zero to get check digit
            int nextHigherNumberEndingWithZero = sumOfDigits.GetNextMultipleOfTen();
            return (nextHigherNumberEndingWithZero - sumOfDigits).ToString();
        }

        public static int[] ToIntArraySplitDigits(this int[] numbers)
        {
            List<int> printableItems = new List<int>();
            foreach (int number in numbers)
            {
                printableItems.AddRange(number.ToString().ToIntArray());
            }
            return printableItems.ToArray();
        }

        public static int[] ToIntArray(this string value)
        {
            return value.ToCharArray().Select(n => (int)char.GetNumericValue(n)).ToArray();
        }

        public static int GetNextMultipleOfTen(this int number)
        {
            return (number + 9) - (number + 9) % 10;
        }

        public static void Print(this int[] numbers, bool inline = false, string collectionName = null)
        {
            if (inline)
            {
                Console.WriteLine($"{collectionName ?? nameof(numbers)}: {string.Join(" - ", numbers)}");
                return;
            }
            Console.WriteLine($"{collectionName ?? nameof(numbers)}:");
            foreach (var item in numbers)
            {
                Console.WriteLine($"{item}");

            }
        }
    }
}
