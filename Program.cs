using System;
using System.Collections.Generic;
using System.Linq;

namespace Genetics
{
    class Program
    {
        static void Main(string[] args)
        {
            do
            {
                //StringSearch stringSearch = new StringSearch();
                //List<StringGen> population = new List<StringGen>();
                //List<StringGen> buffer = new List<StringGen>();

                //stringSearch.init_population(ref population,ref buffer);
                //stringSearch.RunAlgorithm(ref population,ref buffer);

                //List<int> x = new List<int>(5);
                //var a = Enumerable.Repeat(0, 5).ToList();

                NQueens nQueens = new NQueens(12,MutationOperator.SimpleInversion);
                List<NQueensGen> population = new List<NQueensGen>();
                List<NQueensGen> buffer = new List<NQueensGen>();

                nQueens.init_population(ref population, ref buffer);
                nQueens.RunAlgorithm(ref population, ref buffer,CrossoverMethod.ER);
            } while (Console.ReadKey(true).Key != ConsoleKey.Escape);

        }
    }
}