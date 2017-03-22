using System;
using System.Collections.Generic;

namespace Genetics
{
    class Program
    {
        static void Main(string[] args)
        {
            do
            {
                // Compose a string that consists of three lines.
                //string lines = "";

                GeneticsAlgorithms geneticsAlgorithms = new GeneticsAlgorithms();
                List<StringGen> population = new List<StringGen>();
                List<StringGen> buffer = new List<StringGen>();

                geneticsAlgorithms.init_population(ref population,ref buffer);
                int totalIteration = -1;
                for (int i = 0; i < GeneticsAlgorithms.GaMaxiter; i++)
                {
                    geneticsAlgorithms.calc_fitness(ref population);		// calculate fitness
                 //   geneticsAlgorithms.calc_fitness_with_bonus(ref population); // calculate fitness with bonuses

                    geneticsAlgorithms.sort_by_fitness(ref population);   // sort them
                    var avg = geneticsAlgorithms.calc_avg(population); // calc avg
                    var stdDev = geneticsAlgorithms.calc_std_dev(population, avg); //calc std dev
                    geneticsAlgorithms.print_result_details(population,avg,stdDev);        // print the best one, average and std dev
                    if ((population)[0].Fitness == 0)
                    {
                        totalIteration = i; // save number of iteration
                        break;
                    }

                    geneticsAlgorithms.Mate(population,ref buffer);     // mate the population together
                    geneticsAlgorithms.Swap(ref population,ref buffer);       // swap buffers
                }
                Console.WriteLine("Iterations: "+ totalIteration);

            } while (Console.ReadKey(true).Key != ConsoleKey.Escape);

        }
    }
}