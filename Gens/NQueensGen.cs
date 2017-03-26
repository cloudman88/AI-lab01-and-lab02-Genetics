using System.Collections.Generic;
using System.Linq;

namespace Genetics.Gens 
{
    class NQueensGen : Gen
    {
        public List<int> NQueensPos { get; set; }

        public NQueensGen(int n)
        {
            NQueensPos = Enumerable.Repeat(-1, n).ToList();            
        }
    }
}
