using System;
using Genetics.BinPacking;
using Genetics.GeneticsAlgorithms;
using Genetics.NQueens;

namespace Genetics
{
    class Program
    {
        static void Main(string[] args)
        {
            Manager man = new Manager();
            do
            {
                man.Run();
            } while (Console.ReadKey(true).Key != ConsoleKey.Escape);
        }
    }
}