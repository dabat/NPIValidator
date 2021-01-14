using System;
using System.Collections.Generic;
using System.Linq;

/*
 * https://www.cms.gov/Regulations-and-Guidance/Administrative-Simplification/NationalProvIdentStand/Downloads/NPIcheckdigit.pdf
 * Luhn Formula for Modulus 10 “double-add-double” Check Digit
 * The Luhn check digit formula is calculated as follows: 
 * 1.Double the value of alternate digits beginning with the rightmost digit.
 * 2.Add the individual digits of the products resulting from step 1 to the unaffected digits from the original number.
 * 3.Subtract the total obtained in step 2 from the next higher number ending in zero.  This is the check digit.
 *   If the total obtained in step 2 is a number ending in zero, the check digit is zero.
 * 
 * Example of Check Digit Calculation for NPI used without Prefix
 * Assume the 9-position identifier part of the NPI is 123456789.  
 * Using the Luhn formula on the identifier portion, the check digit is calculated as follows:
 * NPI without check digit:
 * 1     2     3     4     5     6     7     8     9 
 * 
 * Step 1: Double the value of alternate digits, beginning with the rightmost digit.
 * 2            6          10          14          18 
 * 
 * Step 2:  Add constant 24, to account for the 80840 prefix that would be present on a card issuer identifier, 
 * plus the individual digits of products of doubling, plus unaffected digits.
 * 24 + 2 + 2 + 6 + 4 + 1 + 0 + 6 + 1 + 4 + 8 + 1 + 8 = 67 
 * 
 * Step 3:  Subtract from next higher number ending in zero. 
 * 70 – 67 = 3 
 * Check digit = 3 
 * NPI with check digit = 1234567893 
*/

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
