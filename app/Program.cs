using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace app
{
    class NominalCount
    {
        public int Nominal { get; init; }

        public int Count { get; set; }

        public int UsedInExchange { get; set; }
    }
    class Program
    {
        private const string FileName = @"../../../nominals.txt";
        private static NominalCount[] GetNominalsCount()
        {
            IEnumerable<string> lines = File.ReadLines(FileName);
            return lines.Select(l =>
            {
                var splitted = l.Split(',');
                return new NominalCount
                {
                    Nominal = int.Parse(splitted[0]),
                    Count = int.Parse(splitted[1]),
                };
            }).OrderByDescending(x => x.Nominal).ToArray();
        }

        static void Main(string[] args)
        {
            var nominalsCounts = GetNominalsCount();
            Console.Write("Enter amount: ");
            var input = Console.ReadLine();
            while(input != "exit")
            {
                var amount = int.Parse(input);

                var successfull = ProcessExchange(amount, nominalsCounts);
                if(!successfull)
                {
                    Console.WriteLine("No Change.");
                }
                else
                {
                    foreach(var nominal in nominalsCounts.Where(x => x.UsedInExchange > 0))
                    {
                        if (nominal.UsedInExchange > 0)
                        {
                            Console.WriteLine($"{nominal.Nominal}:{nominal.UsedInExchange}");
                            nominal.Count -= nominal.UsedInExchange;
                            nominal.UsedInExchange = 0;
                        }
                    }
                    nominalsCounts = nominalsCounts.Where(x => x.Count > 0).ToArray();
                }

                Console.Write("Enter amount: ");
                input = Console.ReadLine();
            }

            Console.Write("End...");
        }

        public static bool ProcessExchange(int amount, NominalCount[] source)
        {
            int[][] coinsUsed = new int[amount + 1][];
            for (int i = 0; i <= amount; ++i)
            {
                coinsUsed[i] = new int[source.Length];
            }

            int[] minCoins = new int[amount + 1];
            for (int i = 1; i <= amount; ++i)
            {
                minCoins[i] = int.MaxValue - 1;
            }

            int[] limits = source.Select(x => x.Count).ToArray();

            for (int i = 0; i < source.Length; ++i)
            {
                while (limits[i] > 0)
                {
                    for (int j = amount; j >= 0; --j)
                    {
                        int currAmount = j + source[i].Nominal;
                        if (currAmount <= amount)
                        {
                            if (minCoins[currAmount] > minCoins[j] + 1)
                            {
                                minCoins[currAmount] = minCoins[j] + 1;

                                coinsUsed[j].CopyTo(coinsUsed[currAmount], 0);
                                coinsUsed[currAmount][i] += 1;
                            }
                        }
                    }

                    limits[i] -= 1;
                }
            }

            if (minCoins[amount] == int.MaxValue - 1)
            {
                return false;
            }

            var change = coinsUsed[amount];

            var nominalCounts = new List<NominalCount>();
            for (int i = 0; i< change.Length; i++)
            {
                var usedNominal = source[i];
                usedNominal.UsedInExchange = change[i];
            }
            return true;
        }

    }
}
