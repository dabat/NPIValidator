using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace NPIValidator
{
    class Program
    {
        static void Main(string[] args)
        {
            string[] good_npis = new string[]
            {
                "123456789", "1234567893", "166964391", "1669643912", "165987224", "1659872240"
            };
            
            string[] bad_npis = new string[]
            {
                "1234567895", "1669643915", "1659872245", "1234", "12345678", "ASDFASDF", "123412h45"
            };
            
            Console.WriteLine("good npis:");
            foreach (string npi in good_npis)
            {
                Console.WriteLine($"npi: '{npi}' checkdigit: '{npi.GetNPICheckDigit()}' calculated checkdigit: '{npi.CalculateNPICheckDigit()}' is valid: {npi.IsNPI()}");
            }
            Console.WriteLine("");
            
            Console.WriteLine("bad npis:");
            foreach (string npi in bad_npis)
            {
                Console.WriteLine($"npi: '{npi}' checkdigit: '{npi.GetNPICheckDigit()}' calculated checkdigit: '{npi.CalculateNPICheckDigit()}' is valid: {npi.IsNPI()}");
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

            Console.WriteLine("press any key to exit");
            Console.ReadKey();
        }

    }

    public static class NPIExtensions
    {
        public static bool IsNPI(this string npi)
        {
            return npi.HasValidNPILength() && long.TryParse(npi, out long n) && npi.GetNPICheckDigit() == npi.CalculateNPICheckDigit();
        }

        public static bool HasValidNPILength(this string npi)
        {
            return !string.IsNullOrWhiteSpace(npi) && (npi.Length == 10 || npi.Length == 9);
        }

        public static string GetNPICheckDigit(this string npi)
        {
            if (!string.IsNullOrWhiteSpace(npi))
            {
                return npi.Length == 10 ? npi[9..] : npi.Length == 9 ? npi.CalculateNPICheckDigit() : "";
            }
            else
            {
                return "";
            }
        }

        public static string CalculateNPICheckDigit(this string npi)
        {
            if (string.IsNullOrWhiteSpace(npi))
            {
                return "";
            }
            string npi9 = npi.Length == 10 ? npi[0..9] : npi.Length == 9 ? npi : "";

            //step1: double alternate digits, starting from the rightmost digit
            int[] doubledAlternates = npi9.ToIntArray().Reverse().GetNth(2).Select(n => n * 2).ToArray();
            
            Debug.WriteLine("two ways to get the alternating elements:");
            Debug.WriteLine($"npi9.ToIntArray().Reverse().Where((digit, index) => index % 2 == 0).Select(n => n * 2).ToArray() => {string.Join(",",npi9.ToIntArray().Reverse().Where((digit, index) => index % 2 == 0).Select(n => n * 2).ToArray())}");
            Debug.WriteLine($"npi9.ToIntArray().Reverse().GetNth(2).Select(n => n * 2).ToArray() => {string.Join(",", npi9.ToIntArray().Reverse().GetNth(2).Select(n => n * 2).ToArray())}");

            //step2: add constant 24 and the individual digits of all tokens
            int[] unaffectedDigits = npi9.ToIntArray().Where((digit, index) => index % 2 == 1).ToArray();
            int sumOfDigits = 24 + doubledAlternates.Select(n => n.SumDigits()).Sum() + unaffectedDigits.Sum();

            //step2.5: if the sum is a number ending in zero, then the check digit is zero
            if (sumOfDigits / 10 == 0) return "0";

            //step3: subtract from next higher number ending in zero to get check digit
            int nextHigherNumberEndingWithZero = sumOfDigits.GetNextMultipleOfTen();
            return (nextHigherNumberEndingWithZero - sumOfDigits).ToString();
        }

        public static bool IsNCPDP(this string id)
        {
            // https://sso.ncpdp.org/HelpResource/Download?strFileName=FAQs_Indy.pdf&strExtension=.pdf&strPage=Resources
            return !string.IsNullOrWhiteSpace(id) && int.TryParse(id, out int n) && id.Length == 7;
        }

        public static int SumDigits(this int number)
        {
            if (number < 10)
            {
                return number;
            }
            else
            {
                return (number % 10) + (number / 10);
            }
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

        public static IEnumerable<T> GetNth<T>(this IEnumerable<T> list, int itemOrdinal)
        {
            if (itemOrdinal <= 0) throw new ArgumentOutOfRangeException(nameof(itemOrdinal), itemOrdinal, null);
            int index = itemOrdinal;
            foreach (T item in list)
            {
                if (++index < itemOrdinal)
                {
                    continue;
                }
                index = 0;
                yield return item;
            }
        }

        public static IEnumerable<T> GetNth<T>(this IReadOnlyList<T> list, int itemOrdinal, int startAt = 1)
        {
            if (itemOrdinal <= 0) throw new ArgumentOutOfRangeException(nameof(itemOrdinal), itemOrdinal, null);
            if (startAt < 1) throw new ArgumentOutOfRangeException(nameof(startAt), startAt, null);
            for (var index = startAt - 1; index < list.Count; index += itemOrdinal)
            {
                yield return list[index];
            }
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

        public static void Print<T>(this IEnumerable<T> list, char delimiter = ',')
        {
            Console.WriteLine($"list: {string.Join(delimiter, list)}");
        }
    }
}
