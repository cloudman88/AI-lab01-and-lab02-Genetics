using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Genetics
{
    public enum CrossoverMehod
    {
        SinglePoint,
        TwoPoint,
        Uniform
    }
    abstract class GeneticsAlgorithms
    {
        private readonly Random _rand;
        public const int GaMaxiter	=	200;		// maximum iterations 16384
        private const int GaPopSize	=	5000;		// ga population size 2048
        private const double GaElitRate = 0.10;	// elitism rate
        private const double GaMutationRate = 0.25;        // mutation rate
        private const int MaxRand = Int32.MaxValue;  //Max value of random function in C#
        private const double GaMutation= MaxRand * GaMutationRate; 
        private const string GaTarget = "Hello world!";
        private const double Bonus = 0.95;
        private const double BigBonus = 0.90;
        private const double MaxBonus = 0.50;

        public GeneticsAlgorithms()
        {
            _rand = new Random();
        }
        public void init_population(ref List<StringGen> population, ref List<StringGen> buffer )
        {
            int targetLength = GaTarget.Length;
            for (int i = 0; i < GaPopSize; i++)
            {
                StringGen citizen = new StringGen();
                int ra;
                for (int j = 0; j < targetLength; j++)
                {
                    ra = _rand.Next();
                    citizen.Str += Convert.ToChar((ra % 90) + 32);
                }

                population.Add(citizen);
                buffer.Add(new StringGen());
            }
        }
        public void calc_fitness(ref List<StringGen> population)
        {
            string target = GaTarget;
            int targetLenght = target.Length;

            for (int i = 0; i < GaPopSize; i++)
            {
                uint fitness = 0;
                for (int j = 0; j < targetLenght; j++)
                {
                    fitness += (uint)Math.Abs(population[i].Str[j] - target[j]);
                }
                population[i].Fitness=fitness;
            }
        }
        public void calc_fitness_with_bonus(ref List<StringGen> population)
        {
            string target = GaTarget;
            int targetLenght = target.Length;
            for (int i = 0; i < GaPopSize; i++)
            {
                uint fitness = 0;
                uint countBigBonus = 0;
                for (int j = 0; j < targetLenght; j++)
                {
                    var grade = (uint)Math.Abs(population[i].Str[j] - target[j]);
                    if (grade == 0) countBigBonus++;
                    fitness += grade;
                }

                uint countBonus = 0;
                string tempTarget = target;

                for (int j = 0; j < targetLenght; j++)
                {
                    if (tempTarget.Contains(population[i].Str[j]))
                    {
                        countBonus++;
                        // replace the matching char with char out of chars range to avoid duplicate counting of the same char
                        var index = tempTarget.IndexOf(population[i].Str[j]);
                        StringBuilder sb = new StringBuilder(tempTarget);
                        sb[index] = Convert.ToChar(127);
                        tempTarget = sb.ToString();
                    }
                }
                if (countBonus!=0) // if there is at least one correct char in the wrong place, remove the percectly positioned char from this counter to avoid duplicity 
                    countBonus -= countBigBonus;
                if (countBonus != 0 && fitness>1) // skip bonus for fitness =1 ,to avoid fitness =0 because of bonus when str isn't matching to target
                {
                    fitness = (uint)(fitness * Math.Max(Math.Pow(Bonus, countBonus), MaxBonus));
                }
                if (countBigBonus != 0 && fitness > 1) // skip bonus for fitness =1 ,to avoid fitness =0 because of bonus when str isn't matching to target
                {
                    fitness = (uint)(fitness * Math.Max(Math.Pow(BigBonus, countBigBonus), MaxBonus));
                }
                population[i].Fitness=fitness;
            }
        }
        public void sort_by_fitness(ref List<StringGen> population)
        {
            population.Sort((s1, s2) => s1.Fitness.CompareTo(s2.Fitness));
            // qsort(population.begin(), population.end(), fitness_sort);
        }
        public void Elitism(List<StringGen> population,ref List<StringGen> buffer, int esize)
        {
            for (int i = 0; i < esize; i++)
            {
                buffer[i].Str = population[i].Str;
                buffer[i].Fitness = population[i].Fitness;
            }
        }
        public void Mutate(StringGen member)
        {
            int targetLenght = GaTarget.Length;
            int ipos = _rand.Next() % targetLenght;
            int delta = (_rand.Next() % 90) + 32;

            StringBuilder sb = new StringBuilder(member.Str);           
            int charVal = (int)member.Str[ipos];
            sb[ipos] = Convert.ToChar((charVal + delta) % 122);
            member.Str = sb.ToString();
        }
        public void Mate(List<StringGen> population,ref List<StringGen> buffer, CrossoverMehod method = CrossoverMehod.SinglePoint, bool isTournament = false )
        {
            if (isTournament)
            {                
                for (int i = 0; i < GaPopSize; i++)
                {
                    //select 2 pairs of gens
                    //pair one
                    int i1 = _rand.Next() % (GaPopSize / 2);
                    int i2 = _rand.Next() % (GaPopSize / 2);
                    //pair two
                    int i3 = _rand.Next() % (GaPopSize / 2);
                    int i4 = _rand.Next() % (GaPopSize / 2);
                    //get the best gen out of each pair, by fitness value
                    var gaRec1 = (population[i1].Fitness < population[i2].Fitness) ? population[i1] : population[i2];
                    var gaRec2 = (population[i3].Fitness < population[i4].Fitness) ? population[i3] : population[i4];
                    //mate the 2 best gens out of each pair
                    MateByMethod(buffer[i], gaRec1, gaRec2, method);
                    
                    //int spos = _rand.Next() % targetLenght;
                    //int spos2 = _rand.Next() % (targetLenght - spos) + spos;
                    //switch (method)
                    //{
                    //        case CrossoverMehod.SinglePoint:
                    //               buffer[i].Str = gaRec1.Str.Substring(0, spos) + gaRec2.Str.Substring(spos, targetLenght - spos);
                    //        break;
                    //        case CrossoverMehod.TwoPoint:                                   
                    //               buffer[i].Str = gaRec1.Str.Substring(0, spos) + gaRec2.Str.Substring(spos, spos2 - spos) + gaRec1.Str.Substring(spos2, targetLenght - spos2);
                    //        break;
                    //        case CrossoverMehod.Uniform:
                    //                StringBuilder sb = new StringBuilder(GaTarget);
                    //                for (int j = 0; j < targetLenght; j++)
                    //                {                                    
                    //                    // randomlly choose char from either gens    
                    //                    int genToChoose = _rand.Next()%2;                                        
                    //                    sb[j] = (genToChoose == 0) ? gaRec1.Str[j] : gaRec2.Str[j];
                    //                }
                    //                buffer[i].Str = sb.ToString();
                    //        break;
                    //}

                    if (_rand.Next() < GaMutation) Mutate(buffer[i]);
                }
            }
            else
            {
                int esize = (int)(GaPopSize * GaElitRate);
                Elitism(population, ref buffer, esize);
                //Mate the rest
                for (int i = esize; i < GaPopSize; i++)
                {
                    var i1 = _rand.Next() % (GaPopSize / 2);
                    var i2 = _rand.Next() % (GaPopSize / 2);
                    MateByMethod(buffer[i], population[i1], population[i2], method);
                    //int spos = _rand.Next() % targetLenght;
                    //buffer[i].Str = population[i1].Str.Substring(0, spos) +
                    //                population[i2].Str.Substring(spos, targetLenght - spos);
                    if (_rand.Next() < GaMutation) Mutate(buffer[i]);
                }                
            }
        }

        public void MateByMethod(StringGen bufGaRecord, StringGen gaRec1, StringGen gaRec2, CrossoverMehod method = CrossoverMehod.SinglePoint)
        {
            int targetLenght = GaTarget.Length;
            int spos = _rand.Next() % targetLenght;
            int spos2 = _rand.Next() % (targetLenght - spos) + spos;
            switch (method)
            {
                case CrossoverMehod.SinglePoint:
                    bufGaRecord.Str = gaRec1.Str.Substring(0, spos) + gaRec2.Str.Substring(spos, targetLenght - spos);
                    break;
                case CrossoverMehod.TwoPoint:
                    bufGaRecord.Str = gaRec1.Str.Substring(0, spos) + gaRec2.Str.Substring(spos, spos2 - spos) + gaRec1.Str.Substring(spos2, targetLenght - spos2);
                    break;
                case CrossoverMehod.Uniform:
                    StringBuilder sb = new StringBuilder(GaTarget);
                    for (int j = 0; j < targetLenght; j++)
                    {
                        // randomlly choose char from either gens    
                        int genToChoose = _rand.Next() % 2;
                        sb[j] = (genToChoose == 0) ? gaRec1.Str[j] : gaRec2.Str[j];
                    }
                    bufGaRecord.Str = sb.ToString();
                    break;
            }
        }
        public void print_result_details(List<StringGen> gav, double avg, double stdDev)
        {
            Console.WriteLine("Best: " + gav[0].Str + " (" + gav[0].Fitness + ")" +
                ", Average: " + string.Format("{0:N2}", avg) +
                ", Standard Deviation: " + string.Format("{0:N2}", stdDev));
        }

        //public String print_result_details(List<GaRecord> gav, double avg, double stdDev)
        //{
        //    string str = "Best: " + gav[0].Str + " (" + gav[0].Fitness + ")" +
        //                 ", Average: " + string.Format("{0:N2}", avg) +
        //                 ", Standard Deviation: " + string.Format("{0:N2}", stdDev);
        //    Console.WriteLine(str);
        //    return str;
        //}
        public void Swap(ref List<StringGen> population,ref List<StringGen> buffer)
        {
             List<StringGen> temp = population;
            population = buffer;
            buffer = temp;
        }
        public double calc_avg(List<StringGen> population)
        {
            double avg = 0;
            for (int i = 0; i < GaPopSize; i++)
            {
                avg += (population)[i].Fitness;
            }
            avg /= GaPopSize;
            return avg;
        }
        public double calc_std_dev(List<StringGen> population, double avg)
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