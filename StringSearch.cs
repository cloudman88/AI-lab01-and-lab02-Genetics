using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Genetics
{
    class StringSearch : GeneticsAlgorithms<StringGen>
    {
        private const string StrTarget = "Hello world!";
        private const double Bonus = 0.95;
        private const double BigBonus = 0.90;
        private const double MaxBonus = 0.50;

        public StringSearch() { }
        public override void init_population(ref List<StringGen> population, ref List<StringGen> buffer)
        {
            int targetLength = StrTarget.Length;
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
        public override void calc_fitness(ref List<StringGen> population)
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
        public void calc_fitness_with_bonus(ref List<StringGen> population)
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
        
        //public override void Elitism(List<StringGen> population, ref List<StringGen> buffer, int esize)
        //{
        //    for (int i = 0; i < esize; i++)
        //    {
        //        buffer[i] = population[i];
        //        //buffer[i].Str = population[i].Str;
        //        //buffer[i].Fitness = population[i].Fitness;
        //    }
        //}
        public override void Mutate(StringGen member)
        {           
            int targetLenght = StrTarget.Length;
            int ipos = Rand.Next() % targetLenght;
            int delta = (Rand.Next() % 90) + 32;
            StringBuilder sb = new StringBuilder(member.Str);
            int charVal = (int)member.Str[ipos];
            sb[ipos] = Convert.ToChar((charVal + delta) % 122);
            member.Str = sb.ToString();
        }
        public override void MateByMethod(StringGen bufGen, StringGen gen1, StringGen gen2,
                                            CrossoverMethod method)
        {
            int targetLenght = StrTarget.Length;
            int spos = Rand.Next() % targetLenght;
            int spos2 = Rand.Next() % (targetLenght - spos) + spos;
            switch (method)
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
        public override Tuple<string, uint> GetBestGenDetails(StringGen gen)
        {
            return new Tuple<string, uint>(gen.Str, gen.Fitness);
        }
    }
}
