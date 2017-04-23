using System;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Genetics.GeneticsAlgorithms;

namespace Genetics.StringSearch
{
    class StringSearch : GeneticsAlgorithms<StringGen>
    {
        private const string StrTarget = "Hello world!";
        private const double Bonus = 0.95;
        private const double BigBonus = 0.90;
        private const double MaxBonus = 0.50;
        private bool _isBonus;

        public bool IsBonus
        {
            get { return _isBonus; }
            set { _isBonus = value; }
        }

        public StringSearch(CrossoverMethod method, SelectionMethod selectionMethod) : base(method, selectionMethod)
        {
            _isBonus = false;
            MaxFitness = (uint)(90*StrTarget.Length);
            Alpha = (int)MaxFitness/2;
        }
        
        public override void init_population()
        {
            int targetLength = StrTarget.Length;
            Population.Clear();
            Buffer.Clear();
            for (int i = 0; i < GaPopSize; i++)
            {
                StringGen citizen = new StringGen(Rand, targetLength);
                //for (int j = 0; j < targetLength; j++)
                //{
                //    var randVal = Rand.Next();
                //    citizen.Str += Convert.ToChar((randVal % 90) + 32);
                //}

                Population.Add(citizen);
                Buffer.Add(new StringGen());
            }
        }

        protected override void calc_fitness()
        {
            string target = StrTarget;
            int targetLenght = target.Length;
            if (IsBonus == false)
            {
                for (int i = 0; i < GaPopSize; i++)
                {
                    uint fitness = 0;
                    for (int j = 0; j < targetLenght; j++)
                    {

                        fitness += (uint) Math.Abs(Population[i].Str[j] - target[j]);
                    }
                    Population[i].Fitness = fitness;
                }
            }
            else
            {
                for (int i = 0; i < GaPopSize; i++)
                {
                    uint fitness = 0;
                    uint countBigBonus = 0;
                    for (int j = 0; j < targetLenght; j++)
                    {
                        var grade = (uint)Math.Abs(Population[i].Str[j] - target[j]);
                        if (grade == 0) countBigBonus++;
                        fitness += grade;
                    }

                    uint countBonus = 0;
                    string tempTarget = target;

                    for (int j = 0; j < targetLenght; j++)
                    {
                        if (tempTarget.Contains(Population[i].Str[j]))
                        {
                            countBonus++;
                            // replace the matching char with char out of chars range to avoid duplicate counting of the same char
                            var index = tempTarget.IndexOf(Population[i].Str[j]);
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
                    Population[i].Fitness = fitness;
                }
            }
        }
        //private void calc_fitness_with_bonus()
        //{
        //    string target = StrTarget;
        //    int targetLenght = target.Length;
        //    for (int i = 0; i < GaPopSize; i++)
        //    {
        //        uint fitness = 0;
        //        uint countBigBonus = 0;
        //        for (int j = 0; j < targetLenght; j++)
        //        {
        //            var grade = (uint)Math.Abs(population[i].Str[j] - target[j]);
        //            if (grade == 0) countBigBonus++;
        //            fitness += grade;
        //        }

        //        uint countBonus = 0;
        //        string tempTarget = target;

        //        for (int j = 0; j < targetLenght; j++)
        //        {
        //            if (tempTarget.Contains(population[i].Str[j]))
        //            {
        //                countBonus++;
        //                // replace the matching char with char out of chars range to avoid duplicate counting of the same char
        //                var index = tempTarget.IndexOf(population[i].Str[j]);
        //                StringBuilder sb = new StringBuilder(tempTarget);
        //                sb[index] = Convert.ToChar(127);
        //                tempTarget = sb.ToString();
        //            }
        //        }
        //        if (countBonus != 0) // if there is at least one correct char in the wrong place, remove the percectly positioned char from this counter to avoid duplicity 
        //            countBonus -= countBigBonus;
        //        if (countBonus != 0 && fitness > 1) // skip bonus for fitness =1 ,to avoid fitness =0 because of bonus when str isn't matching to target
        //        {
        //            fitness = (uint)(fitness * Math.Max(Math.Pow(Bonus, countBonus), MaxBonus));
        //        }
        //        if (countBigBonus != 0 && fitness > 1) // skip bonus for fitness =1 ,to avoid fitness =0 because of bonus when str isn't matching to target
        //        {
        //            fitness = (uint)(fitness * Math.Max(Math.Pow(BigBonus, countBigBonus), MaxBonus));
        //        }
        //        population[i].Fitness = fitness;
        //    }
        //}        
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
            switch (CrosMethod)
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

        protected override StringGen get_new_gen()
        {
            return new StringGen(Rand,StrTarget.Length);
        }

        protected override int calc_distance(StringGen strGen1, StringGen strGen2)
        {
            //Levenshtein distance
            int[,] distances = new int[StrTarget.Length+1,StrTarget.Length+1];
            string str1 = strGen1.Str;
            string str2 = strGen2.Str;
            for (int i = 0; i < StrTarget.Length+1; i++)
            {
                distances[i, 0] = i;
                distances[0, i ] = i;
            }
            for (int i = 1; i < StrTarget.Length+1; i++)
            {
                for (int j = 1; j < StrTarget.Length+1; j++)
                {
                    int substitutionCost = (str1[i - 1] == str2[j - 1]) ? 0 : Math.Abs(str1[i - 1] - str2[j - 1]);
                    distances[i, j] = Math.Min(distances[i - 1, j] + 1, // deletion
                        Math.Min(distances[i, j - 1] + 1,              // insertion
                        distances[i - 1, j - 1] + substitutionCost)); // substitution
                }
            }                
            return distances[StrTarget.Length, StrTarget.Length];
        }
    }
}