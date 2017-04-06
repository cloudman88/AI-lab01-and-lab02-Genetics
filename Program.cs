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
                StringSearch.StringSearch sSearch = new StringSearch.StringSearch(CrossoverMethod.SinglePoint, SelectionMethod.Truncation);
                sSearch.init_population();
                sSearch.run_algorithm();
                //BinPacking.BinPackingGenetics bpg = new BinPackingGenetics(4, MutationOperator.Displacement, CrossoverMethod.CX, SelectionMethod.Truncation);
                //bpg.init_population();
                //bpg.run_algorithm();

                //man.Run();
            } while (Console.ReadKey(true).Key != ConsoleKey.Escape);
        }
    }
}