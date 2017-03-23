using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Genetics 
{
    class NQueensGen : Gen
    {
        private int[] _nQueensPos; //every value represnt a queen raw position, this prevent 2 queens or more in the same column
        public int[] NQueensPos
        {
            get { return _nQueensPos; }
            set { _nQueensPos = value; }
        }

        public NQueensGen(int n)
        {
            _nQueensPos = new int[n];
            _fitness = 0;
        }
    }
}
