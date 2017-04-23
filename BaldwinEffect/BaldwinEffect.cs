using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Genetics.BinPacking;
using Genetics.GeneticsAlgorithms;

namespace Genetics.BaldwinEffect
{
    class BaldwinEffect : GeneticsAlgorithms<BinaryGen>
    {
        public const double OnesProbability = 0.25;
        public const double ZeroesProbability = 0.25;
        public const double UnknownProbability = 0.5;

        private const String TargetString = "01100010100111001000";
        private const int NumOfLocalSearches = 1000;
        private readonly Random _rand;
        private bool _targetWasFound;

        public BaldwinEffect(CrossoverMethod crossMethod, SelectionMethod selectionMethod) : base(crossMethod, selectionMethod)
        {
            _rand = new Random();
            _targetWasFound = false;
        }

        public override void init_population()
        {
            Population = new List<BinaryGen>();
            Buffer = new List<BinaryGen>();
            for (int i = 0; i < GaPopSize; i++)
            {
                BinaryGen binaryGen = new BinaryGen(TargetString.Length, _rand);

                Population.Add(binaryGen);
                Buffer.Add(binaryGen);
            }
        }
        protected override void calc_fitness()
        {
            for (int i = 0; i < GaPopSize; i++)
            {
                local_search(Population[i]);
            }
        }
        private void local_search(BinaryGen binaryGen)
        {
            int j; // number of attempts
            var tempCorrect = binaryGen.CorrectRate;
            BinaryGen prototype = new BinaryGen(binaryGen);
            for ( j = 0; j < NumOfLocalSearches; j++)
            {
                BinaryGen tempCopy = new BinaryGen(prototype);
                string result = FillingUnfixedChars(tempCopy);
                if (result.Equals(TargetString))
                {
                    //binaryGen.Fitness = (uint) (1 + (19*n/NumOfLocalSearches));
                    _targetWasFound = true;
                    break;
                }
                if (tempCorrect < tempCopy.CorrectRate) //save the best result so far
                {
                    tempCorrect = binaryGen.CorrectRate;
                    binaryGen = new BinaryGen(tempCopy);
                    binaryGen.BinaryStr = String.Copy(result);
                }
            }
            //var n = NumOfLocalSearches - j;
            binaryGen.Fitness = (uint)(1 + (19 * j / NumOfLocalSearches));
        }
        private string FillingUnfixedChars(BinaryGen binaryGen)
        {
            string binaryStr = binaryGen.BinaryStr;
            StringBuilder sb = new StringBuilder(TargetString);
            int unknownCount = 0;
            int newCorrectCount = 0;
            int incorrectCount = 0;
            for (int i = 0; i < binaryStr.Length; i++)
            {
                if (binaryStr[i].Equals('?'))
                {
                    unknownCount++;
                    int temp = _rand.Next()%2;
                    if (temp == 0) sb[i] = '0';
                    else if (temp == 1) sb[i] = '1';
                    else sb[i] = '?';
                    //sb[i] = (temp == 0) ? '0' : '1';
                    if (!sb[i].Equals('?') && sb[i].Equals(TargetString[i]))
                    {
                        newCorrectCount++;
                    }
                    else if (!sb[i].Equals('?') && !sb[i].Equals(TargetString[i]))
                    {
                        incorrectCount++;
                    }
                }
                else
                {
                    sb[i] = binaryStr[i];
                }
            }
            if (unknownCount != 0)
            {
                //var correctBefore = TargetString.Length - unknownCount;
                binaryGen.CorrectRate = (newCorrectCount ) /(double) (TargetString.Length- unknownCount);
                binaryGen.IncorrectRate = incorrectCount/(double)TargetString.Length;
                binaryGen.NewLearnedRate = newCorrectCount/(double) unknownCount;
            }
            return sb.ToString();
        }
        private void print_matching_result(int index,BinaryGen binaryGen, double avgCor,double avgInc,double avgNew)
        {
            double correctPosRateAvg = avgCor;
            double incorrectPosRateAvg = avgInc;
            double newLearnedRateAvg = avgNew;

            Console.WriteLine("#"+(index+1)+" "+ binaryGen.BinaryStr + ", "+
                "cor.Avg: "+ string.Format("{0:N2}", correctPosRateAvg) +
                " ,incor.Avg: "+ string.Format("{0:N2}", incorrectPosRateAvg) +
                ", new.Avg: "+ string.Format("{0:N2}", newLearnedRateAvg));
        }

        protected override void Mutate(BinaryGen member)
        {
            int targetLenght = TargetString.Length;
            int ipos = Rand.Next() % targetLenght;
            int delta = (Rand.Next() % 90) + 32;
            StringBuilder sb = new StringBuilder(member.BinaryStr);
            int charVal = (int)member.BinaryStr[ipos];
            sb[ipos] = Convert.ToChar((charVal + delta) % 122);
            member.BinaryStr = sb.ToString();
        }

        protected override void mate_by_method(BinaryGen bufGen, BinaryGen gen1, BinaryGen gen2)
        {
            int targetLenght = TargetString.Length;
            int spos = Rand.Next() % targetLenght;
            int spos2 = Rand.Next() % (targetLenght - spos) + spos;
            switch (CrosMethod)
            {
                case CrossoverMethod.SinglePoint:
                    bufGen.BinaryStr = gen1.BinaryStr.Substring(0, spos) + gen2.BinaryStr.Substring(spos, targetLenght - spos);
                    break;
                case CrossoverMethod.TwoPoint:
                    bufGen.BinaryStr = gen1.BinaryStr.Substring(0, spos) + gen2.BinaryStr.Substring(spos, spos2 - spos) + gen1.BinaryStr.Substring(spos2, targetLenght - spos2);
                    break;
                case CrossoverMethod.Uniform:
                    StringBuilder sb = new StringBuilder(TargetString);
                    for (int j = 0; j < targetLenght; j++)
                    {
                        // randomlly choose char from either gens    
                        int genToChoose = Rand.Next() % 2;
                        sb[j] = (genToChoose == 0) ? gen1.BinaryStr[j] : gen2.BinaryStr[j];
                    }
                    bufGen.BinaryStr = sb.ToString();
                    break;
            }
        }

        protected override Tuple<string, uint> get_best_gen_details(BinaryGen gen)
        {
            return new Tuple<string, uint>(gen.BinaryStr, gen.Fitness);

        }

        protected override BinaryGen get_new_gen()
        {
            throw new NotImplementedException();
        }

        protected override int calc_distance(BinaryGen gen1, BinaryGen gen2)
        {
            throw new NotImplementedException();
        }

        public override void run_algorithm()
        {
            int totalIteration = -1;
            for (int i = 0; i < GaMaxiter; i++)
            {
                calc_fitness();      // calculate fitness
                sort_by_fitness();   // sort them                        
                double avgCor = calc_correct_avg();
                double avgInc = calc_incorrect_avg();
                double avgNew = calc_new_learned_avg();
                print_matching_result(i, Population[0], avgCor, avgInc, avgNew);

                if (_targetWasFound == true)
                {
                    totalIteration = i + 1; // save number of iteration                                                           
                    break;
                }
                Mate();     // mate the population together
                swap_population_with_buffer();       // swap buffers
            }
            if (totalIteration == GaMaxiter)
            {
                Console.WriteLine("Failed to find solution in " + totalIteration + " iterations.");
            }
            else
            {
                Console.WriteLine("Iterations: " + totalIteration);
            }
        }

        private double calc_new_learned_avg()
        {
            double res = 0;
            for (int i = 0; i < GaPopSize; i++)
            {
                res += Population[i].NewLearnedRate;
            }
            return (res / GaPopSize);
        }

        private double calc_incorrect_avg()
        {
            double res = 0;
            for (int i = 0; i < GaPopSize; i++)
            {
                res += Population[i].IncorrectRate;
            }
            return (res / GaPopSize);
        }

        private double calc_correct_avg()
        {
            double res = 0;
            for (int i = 0; i < GaPopSize; i++)
            {
                res += Population[i].CorrectRate;
            }
            return (res/GaPopSize);
        }
    }
}
