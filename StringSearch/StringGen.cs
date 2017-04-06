using System;
using Genetics.GeneticsAlgorithms;

namespace Genetics.StringSearch
{
    class StringGen : Gen
    {
        public string Str { get; set; }
        public StringGen()
        {
            Str = "";
        }
        public StringGen(Random rand,int trgLenght)
        {
            for (int j = 0; j < trgLenght; j++)
            {
                var randVal = rand.Next();
                Str += Convert.ToChar((randVal % 90) + 32);
            }
        }
    }
}
