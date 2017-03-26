using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Genetics.Gens;

namespace Genetics
{
    public enum CrossoverMethod
    {
        // these are for String Search only (non-ordered chromosomes)
        SinglePoint,
        TwoPoint,
        Uniform,
        // these are for Nqueens,bin packing problem only (ordered chromosomes)
        PMX, //Partially Matched
        OX,  //Ordered
        CX,  //Cycle 
        ER   //Edge recombination
    }
    abstract class GeneticsAlgorithms <T> where T : Gen
    {
        protected readonly Random Rand;
        protected const int GaMaxiter	=	3000;		// maximum iterations 16384
        protected const int GaPopSize	= 2048;		// ga population size 2048
        protected const double GaElitRate = 0.1;	    // elitism rate
        protected const double GaMutationRate = 0.25;    // mutation rate
        protected const int MaxRand = Int32.MaxValue;    //Max value of random function in C#
        protected const double GaMutation= MaxRand * GaMutationRate;
        protected List<T> population;
        protected List<T> buffer;
        protected CrossoverMethod crossoverMethod;
        protected bool isTournament;

        protected GeneticsAlgorithms(CrossoverMethod method, bool isTour)
        {
            Rand = new Random();
            population = new List<T>();
            buffer = new List<T>();
            crossoverMethod = method;
            isTournament = isTour;
        }

        public abstract void init_population();
        public virtual void run_algorithm()
        {
            int totalIteration = -1;
            for (int i = 0; i < GaMaxiter; i++)
            {
                calc_fitness();      // calculate fitness
                sort_by_fitness();   // sort them
                var avg = calc_avg(); // calc avg
                var stdDev = calc_std_dev(avg); //calc std dev
                print_result_details(population[0], avg, stdDev);  // print the best one, average and std dev
                if ((population)[0].Fitness == 0)
                {
                    totalIteration = i; // save number of iteration
                    break;
                }
                Mate();     // mate the population together
                swap_population_with_buffer();       // swap buffers
            }
            Console.WriteLine("Iterations: " + totalIteration);
        }

        protected abstract void calc_fitness();
        protected void sort_by_fitness()
        {
            population.Sort((s1, s2) => s1.Fitness.CompareTo(s2.Fitness));
        }
        protected void Elitism(int esize)
        {
            for (int i = 0; i < esize; i++)
            {
                buffer[i] = population[i];
            }
        }
        protected abstract void Mutate(T member);
        protected void Mate()
        {
            int esize = (int)(GaPopSize * GaElitRate);
            Elitism(esize);
            //Mate the rest
            if (isTournament)
            {                
                for (int i = esize; i < GaPopSize; i++)
                {
                    //select 2 pairs of gens
                    //pair one
                    int i1 = Rand.Next() % GaPopSize;
                    int i2 = Rand.Next() % GaPopSize;
                    //pair two
                    int i3 = Rand.Next() % GaPopSize;
                    int i4 = Rand.Next() % GaPopSize;
                    //get the best gen out of each pair, by fitness value
                    var gen1 = (population[i1].Fitness < population[i2].Fitness) ? population[i1] : population[i2];
                    var gen2 = (population[i3].Fitness < population[i4].Fitness) ? population[i3] : population[i4];
                    //mate the 2 best gens out of each pair
                    mate_by_method(buffer[i], gen1, gen2);
                    
                    if (Rand.Next() < GaMutation) Mutate(buffer[i]);
                }
            }
            else // TRUNCATION 
            {                
                for (int i = esize; i < GaPopSize; i++)
                {
                    var i1 = Rand.Next() % (GaPopSize / 2);
                    var i2 = Rand.Next() % (GaPopSize / 2);
                    mate_by_method(buffer[i], population[i1], population[i2]);
                    if (Rand.Next() < GaMutation) Mutate(buffer[i]);
                }                
            }
        }
        protected abstract void mate_by_method(T bufGen, T gen1, T gen2);
        protected void print_result_details(T bestGen, double avg, double stdDev)
        {
            Tuple<string, uint> bestGenDetails = get_best_gen_details(bestGen);
            Console.WriteLine("Best: " + bestGenDetails.Item1 + " (" + bestGenDetails.Item2 + ")" +
                ", Average: " + string.Format("{0:N2}", avg) +
                ", Standard Deviation: " + string.Format("{0:N2}", stdDev));
        }
        protected abstract Tuple<string, uint> get_best_gen_details(T gen);
        protected void swap_population_with_buffer()
        {
            List<T> temp = population;
            population = buffer;
            buffer = temp;
        }
        protected double calc_avg()
        {
            double avg = 0;
            for (int i = 0; i < GaPopSize; i++)
            {
                avg += (population)[i].Fitness;
            }
            avg /= GaPopSize;
            return avg;
        }
        protected double calc_std_dev(double avg)
        {
            double standardDeviation = 0.0;
            int i;          
            for (i = 0; i < GaPopSize; ++i)
                standardDeviation += Math.Pow(population[i].Fitness - avg, 2);
            standardDeviation = Math.Sqrt(standardDeviation / GaPopSize);
            return standardDeviation;
        }
    }
}