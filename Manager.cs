using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;

namespace Genetics
{
    class Manager
    {
        private CrossoverMethod _crossoverMethod;
        private MutationOperator _mutationOperator;
        private int _n;
        private bool isTournament;
        private Stopwatch _sw;

        public Manager()
        {
            _sw = new Stopwatch();
        }

        public void Run()
        {
            print_options();
                int input = get_input();                
                switch (input)
                {
                    case 1:
                        run_string_search();
                        break;
                    case 2:
                        run_n_queens();
                        break;
                    case 3:
                        run_minimal_conflits();
                        break;
                    case 4:
                        run_bin_packing_ga();
                        break;
                    case 5:
                        run_bin_packing_first_fit();
                        break;
                    default:
                        Console.WriteLine("please enter a number between 1 to 5");
                        break;
                }
        }

        private void print_options()
        {
            Console.WriteLine("Please choose algorithm by it's number: ");
            Console.WriteLine("1.String search");
            Console.WriteLine("2.N-Queens using Genetics Algorithms");
            Console.WriteLine("3.N-Queens using Minimal Conflits");
            Console.WriteLine("4.Bin Packing using Genetics Algorithms");
            Console.WriteLine("5.Bin Packing - First Fit");
        }
        private void run_string_search()
        {
           choose_crossover_method(false);
            set_isTournamnet();
            bool isBonus = set_isBonus();

            StringSearch stringSearch = new StringSearch(_crossoverMethod, isTournament);
            do
            {
                _sw.Start();
                stringSearch.init_population();
                stringSearch.run_algorithm(isBonus);
                _sw.Stop();
                Console.WriteLine("Elapsed={0}", _sw.Elapsed);
                _sw.Reset();
                Console.WriteLine("press any key to run again or escapse to exit");
            } while (Console.ReadKey(true).Key != ConsoleKey.Escape);
        }

        private void run_n_queens()
        {
            choose_crossover_method();
            choose_mutations_operator();
            set_value_for_N();
            set_isTournamnet();
            NQueens nQueens = new NQueens(_n, _mutationOperator, _crossoverMethod, isTournament);
            do
            {
                nQueens.init_population();
                nQueens.run_algorithm();
                Console.WriteLine("press any key to run again or escapse to exit");
            } while (Console.ReadKey(true).Key != ConsoleKey.Escape);
        }
        private void run_minimal_conflits()
        {
            set_value_for_N();
            MinConflitsAlgorithm mca = new MinConflitsAlgorithm(_n);
            do
            {
                mca.init_current_state();
                mca.run_algorithm();
                Console.WriteLine("press any key to run again or escapse to exit");
            } while (Console.ReadKey(true).Key != ConsoleKey.Escape);
        }
        private void run_bin_packing_ga()
        {
            choose_crossover_method();
            choose_mutations_operator();
            set_isTournamnet();
            List<int> volumes = new List<int>() { 3, 1, 6, 4, 5, 2 };
            string str = "";
            foreach (var volume in volumes)
            {
                str += volume + " ";
            }
            int containerSize = 7;
            Console.WriteLine("numbers to be set in minimum bins :"+ str);
            Console.WriteLine("container size :"+containerSize);

            BinPackingGenetics bpg = new BinPackingGenetics(volumes, containerSize,_mutationOperator,_crossoverMethod , isTournament);
            do
            {
                bpg.init_population();
                bpg.run_algorithm();
                Console.WriteLine("press any key to run again or escapse to exit");
            } while (Console.ReadKey(true).Key != ConsoleKey.Escape);
        }
        private void run_bin_packing_first_fit()
        {
            List<int> volumes = new List<int>() { 3, 1, 6, 4, 5, 2 };
            string str = "";
            foreach (var volume in volumes)
            {
                str += volume + " ";
            }
            int containerSize = 7;
            Console.WriteLine("numbers to be set in minimum bins :" + str);
            Console.WriteLine("container size :" + containerSize);

            BinPackingAlgorithm bpa = new BinPackingAlgorithm(volumes, containerSize);
            bpa.run_first_fit_algo();
            Console.WriteLine("press any key to run again or escapse to exit");
        }
        private int get_input()
        {
            bool validInput = true;
            int input = 0;
            do
            {
                try
                {
                    input = Convert.ToInt32(Console.ReadLine());
                    Console.WriteLine("");
                }
                catch (Exception e)
                {
                    validInput = false;
                    Console.WriteLine("please enter a number");
                }

            } while (!validInput);
            return input;
        }
        private void choose_crossover_method(bool isOrdered = true)
        {
            Console.WriteLine("Please Choose CrossOver Method :");
            var methodsList = Enum.GetValues(typeof(CrossoverMethod)).Cast<CrossoverMethod>().ToList();
            if (isOrdered == true)
            {
                for (int i = 0; i < 3; i++)
                {
                    methodsList.RemoveAt(0);                    
                }
            }
            else
            {
                for (int i = 0; i < 4; i++)
                {
                    methodsList.Remove(methodsList.Last());
                }
            }
            for (int i = 0; i < methodsList.Count; i++)
            {
                Console.WriteLine((i + 1) + ". " + methodsList[i]);
            }
            int input = 0;
            do
            {
                input = get_input();

            } while (input <= 0 || input > methodsList.Count);
            _crossoverMethod = methodsList[input - 1];
        }
        private void choose_mutations_operator()
        {
            Console.WriteLine("Please Choose Mutation Operator :");
            var mutationList = Enum.GetValues(typeof(MutationOperator)).Cast<MutationOperator>().ToList();
            for (int i = 0; i < mutationList.Count; i++)
            {
                Console.WriteLine((i + 1) + ". " + mutationList[i]);
            }
            int input = 0;
            do
            {
                input = get_input();

            } while (input <= 0 || input > mutationList.Count);
            _mutationOperator = mutationList[input];
        }
        private void set_value_for_N()
        {
            Console.WriteLine("Please enter value for variabele N: ");
            int input = 0;
            do
            {
                input = get_input();

            } while (input <= 0);
            _n = input;
        }
        private void set_isTournamnet()
        {
            Console.WriteLine("Please set if Selection method: ");
            Console.WriteLine("1. Tournament");
            Console.WriteLine("2. Truncation ");
            int input = 0;
            do
            {
                input = get_input();

            } while (input <= 0 || input >2);
            isTournament = (input==1);
        }
        private bool set_isBonus()
        {
            Console.WriteLine("Please choose Huristic method: ");
            Console.WriteLine("1. with Bonus for bullseye");
            Console.WriteLine("2. no bonus ");
            int input = 0;
            do
            {
                input = get_input();

            } while (input <= 0 || input > 2);
            return (input == 1);
        }
    }
}
