using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Genetics.GeneticsAlgorithms;

namespace Genetics.BaldwinEffect
{
    class BinaryGen : Gen
    {
        public String BinaryStr { get; set; }
        public double NewLearnedRate;
        public double CorrectRate;
        public double IncorrectRate;

        public BinaryGen(int size, Random rand)
        {
            List<int> indexes = Enumerable.Range(1, size).ToList();
            StringBuilder sb = new StringBuilder(new string('?', size));
            int numOfOnes = (int) (size*BaldwinEffect.OnesProbability);
            int numOfZeroes = (int) (size*BaldwinEffect.ZeroesProbability);            
            for (int i = 0; i < numOfOnes; i++)
            {
                int tempIndex = rand.Next()%indexes.Count;
                indexes.RemoveAt(tempIndex); //remove index to avoid duplicate indexes for future chars
                sb[tempIndex] = '1';
            }
            for (int i = 0; i < numOfZeroes; i++)
            {
                int tempIndex = rand.Next() % indexes.Count;
                indexes.RemoveAt(tempIndex);
                sb[tempIndex] = '0';
            }
            BinaryStr = sb.ToString();
            IncorrectRate = BaldwinEffect.UnknownProbability;
            CorrectRate = 1 - BaldwinEffect.UnknownProbability;
            NewLearnedRate = 0;
        }

        public BinaryGen(BinaryGen binGen)
        {
            BinaryStr = binGen.BinaryStr;
            IncorrectRate = binGen.IncorrectRate;
            CorrectRate = binGen.CorrectRate;
            NewLearnedRate = binGen.NewLearnedRate;
        }
    }
}
