using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Genetics
{
    public enum CrossoverMethod
    {
        SinglePoint,
        TwoPoint,
        Uniform,
        PMX, //Partially Matched
        OX,  //Ordered
        CX,  //Cycle 
        ER   //Edge recombination
    }
    abstract class GeneticsAlgorithms <T> where T : Gen
    {
       protected readonly Random Rand;
       protected const int GaMaxiter	=	200;		// maximum iterations 16384
       protected const int GaPopSize	=	2048;		// ga population size 2048
       protected const double GaElitRate = 0.10;	// elitism rate
       protected const double GaMutationRate = 0.25;        // mutation rate
       protected const int MaxRand = Int32.MaxValue;  //Max value of random function in C#
       protected const double GaMutation= MaxRand * GaMutationRate; 

        protected GeneticsAlgorithms()
        {
            Rand = new Random();
        }

        public abstract void init_population(ref List<T> population, ref List<T> buffer);
        public abstract void calc_fitness(ref List<T> population);
        public void sort_by_fitness(ref List<T> population)
        {
            population.Sort((s1, s2) => s1.Fitness.CompareTo(s2.Fitness));
        }
        public void Elitism(List<T> population, ref List<T> buffer, int esize)
        {
            for (int i = 0; i < esize; i++)
            {
                buffer[i] = population[i];
            }
        }
        public abstract void Mutate(T member);
        public void Mate(List<T> population,ref List<T> buffer, CrossoverMethod method, bool isTournament = false )
        {
            if (isTournament)
            {                
                for (int i = 0; i < GaPopSize; i++)
                {
                    //select 2 pairs of gens
                    //pair one
                    int i1 = Rand.Next() % (GaPopSize / 2);
                    int i2 = Rand.Next() % (GaPopSize / 2);
                    //pair two
                    int i3 = Rand.Next() % (GaPopSize / 2);
                    int i4 = Rand.Next() % (GaPopSize / 2);
                    //get the best gen out of each pair, by fitness value
                    var gen1 = (population[i1].Fitness < population[i2].Fitness) ? population[i1] : population[i2];
                    var gen2 = (population[i3].Fitness < population[i4].Fitness) ? population[i3] : population[i4];
                    //mate the 2 best gens out of each pair
                    MateByMethod(buffer[i], gen1, gen2, method);
                    
                    if (Rand.Next() < GaMutation) Mutate(buffer[i]);
                }
            }
            else
            {
                int esize = (int)(GaPopSize * GaElitRate);
                Elitism(population, ref buffer, esize);
                //Mate the rest
                for (int i = esize; i < GaPopSize; i++)
                {
                    var i1 = Rand.Next() % (GaPopSize / 2);
                    var i2 = Rand.Next() % (GaPopSize / 2);
                    MateByMethod(buffer[i], population[i1], population[i2], method);
                    if (Rand.Next() < GaMutation) Mutate(buffer[i]);
                }                
            }
        }
        public abstract void MateByMethod(T bufGen, T gen1, T gen2,CrossoverMethod method);
        public void print_result_details(T bestGen, double avg, double stdDev)
        {
            Tuple<string, uint> bestGenDetails = GetBestGenDetails(bestGen);
            Console.WriteLine("Best: " + bestGenDetails.Item1 + " (" + bestGenDetails.Item2 + ")" +
                ", Average: " + string.Format("{0:N2}", avg) +
                ", Standard Deviation: " + string.Format("{0:N2}", stdDev));
        }
        public abstract Tuple<string, uint> GetBestGenDetails(T gen);
        public void Swap(ref List<T> population,ref List<T> buffer)
        {
            List<T> temp = population;
            population = buffer;
            buffer = temp;
        }
        public double calc_avg(List<T> population)
        {
            double avg = 0;
            for (int i = 0; i < GaPopSize; i++)
            {
                avg += (population)[i].Fitness;
            }
            avg /= GaPopSize;
            return avg;
        }
        public double calc_std_dev(List<T> population, double avg)
        {
            double standardDeviation = 0.0;
            int i;          
            for (i = 0; i < GaPopSize; ++i)
                standardDeviation += Math.Pow(population[i].Fitness - avg, 2);
            standardDeviation = Math.Sqrt(standardDeviation / GaPopSize);
            return standardDeviation;
        }
        public virtual void RunAlgorithm(ref List<T> population, ref List<T> buffer, CrossoverMethod method)
        {
            int totalIteration = -1;
            for (int i = 0; i < GaMaxiter; i++)
            {
                calc_fitness(ref population);      // calculate fitness
                //calc_fitness_with_bonus(ref population); // calculate fitness with bonuses
                sort_by_fitness(ref population);   // sort them
                var avg = calc_avg(population); // calc avg
                var stdDev = calc_std_dev(population, avg); //calc std dev
                print_result_details(population[0], avg, stdDev);  // print the best one, average and std dev
                if ((population)[0].Fitness == 0)
                {
                    totalIteration = i; // save number of iteration
                    break;
                }
                Mate(population, ref buffer,method);     // mate the population together
                Swap(ref population, ref buffer);       // swap buffers
            }
            Console.WriteLine("Iterations: " + totalIteration);
        }
    }
}