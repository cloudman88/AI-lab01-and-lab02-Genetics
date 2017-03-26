using System;

namespace Genetics.Gens
{
    class StringGen : Gen
    {
        public string Str { get; set; }
        public StringGen()
        {
            Str = "";
        }
        public StringGen(String str,uint fitness = 0)
        {
            Str = str;
            Fitness = fitness;
        }
    }
}
