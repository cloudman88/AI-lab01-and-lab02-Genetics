using System.Collections.Generic;
using System.Linq;
using Genetics.GeneticsAlgorithms;

namespace Genetics.NQueens 
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
