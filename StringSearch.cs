using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Genetics.Gens;

namespace Genetics
{
    class StringSearch : GeneticsAlgorithms<StringGen>
    {
        private const string StrTarget = "Hello world!";
        private const double Bonus = 0.95;
        private const double BigBonus = 0.90;
        private const double MaxBonus = 0.50;

        public StringSearch(CrossoverMethod method,bool isTour) : base(method,isTour)
        {
        }

        public virtual void run_algorithm(bool isBonus)
        {
            int totalIteration = -1;
            for (int i = 0; i < GaMaxiter; i++)
            {
                if (isBonus == true) calc_fitness_with_bonus(); // calculate fitness with bonuses
                else    calc_fitness();      // calculate fitness
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
        public override void init_population()
        {
            int targetLength = StrTarget.Length;
            population.Clear();
            buffer.Clear();
            for (int i = 0; i < GaPopSize; i++)
            {
                StringGen citizen = new StringGen();
                for (int j = 0; j < targetLength; j++)
                {
                    var randVal = Rand.Next();
                    citizen.Str += Convert.ToChar((randVal % 90) + 32);
                }

                population.Add(citizen);
                buffer.Add(new StringGen());
            }
        }

        protected override void calc_fitness()
        {
            string target = StrTarget;
            int targetLenght = target.Length;

            for (int i = 0; i < GaPopSize; i++)
            {
                uint fitness = 0;
                for (int j = 0; j < targetLenght; j++)
                {

                    fitness += (uint)Math.Abs(population[i].Str[j] - target[j]);
                }
                population[i].Fitness = fitness;
            }
        }
        private void calc_fitness_with_bonus()
        {
            string target = StrTarget;
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
                if (countBonus != 0) // if there is at least one correct char in the wrong place, remove the percectly positioned char from this counter to avoid duplicity 
                    countBonus -= countBigBonus;
                if (countBonus != 0 && fitness > 1) // skip bonus for fitness =1 ,to avoid fitness =0 because of bonus when str isn't matching to target
                {
                    fitness = (uint)(fitness * Math.Max(Math.Pow(Bonus, countBonus), MaxBonus));
                }
                if (countBigBonus != 0 && fitness > 1) // skip bonus for fitness =1 ,to avoid fitness =0 because of bonus when str isn't matching to target
                {
                    fitness = (uint)(fitness * Math.Max(Math.Pow(BigBonus, countBigBonus), MaxBonus));
                }
                population[i].Fitness = fitness;
            }
        }        
        protected override void Mutate(StringGen member)
        {           
            int targetLenght = StrTarget.Length;
            int ipos = Rand.Next() % targetLenght;
            int delta = (Rand.Next() % 90) + 32;
            StringBuilder sb = new StringBuilder(member.Str);
            int charVal = (int)member.Str[ipos];
            sb[ipos] = Convert.ToChar((charVal + delta) % 122);
            member.Str = sb.ToString();
        }
        protected override void mate_by_method(StringGen bufGen, StringGen gen1, StringGen gen2)
        {
            int targetLenght = StrTarget.Length;
            int spos = Rand.Next() % targetLenght;
            int spos2 = Rand.Next() % (targetLenght - spos) + spos;
            switch (crossoverMethod)
            {
                case CrossoverMethod.SinglePoint:
                    bufGen.Str = gen1.Str.Substring(0, spos) + gen2.Str.Substring(spos, targetLenght - spos);
                    break;
                case CrossoverMethod.TwoPoint:
                    bufGen.Str = gen1.Str.Substring(0, spos) + gen2.Str.Substring(spos, spos2 - spos) + gen1.Str.Substring(spos2, targetLenght - spos2);
                    break;
                case CrossoverMethod.Uniform:
                    StringBuilder sb = new StringBuilder(StrTarget);
                    for (int j = 0; j < targetLenght; j++)
                    {
                        // randomlly choose char from either gens    
                        int genToChoose = Rand.Next() % 2;
                        sb[j] = (genToChoose == 0) ? gen1.Str[j] : gen2.Str[j];
                    }
                    bufGen.Str = sb.ToString();
                    break;
            }
        }
        protected override Tuple<string, uint> get_best_gen_details(StringGen gen)
        {
            return new Tuple<string, uint>(gen.Str, gen.Fitness);
        }
    }
}
